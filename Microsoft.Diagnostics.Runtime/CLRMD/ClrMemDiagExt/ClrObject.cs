using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Microsoft.Diagnostics.RuntimeExt
{
    public class ClrObject : DynamicObject
    {
        ulong m_addr;
        bool m_inner;
        ClrType m_type;
        ClrHeap m_heap;
        int m_len = -1;
        public ClrObject(ClrHeap heap, ClrType type, ulong addr, bool inner)
        {
            if (heap == null)
                throw new ArgumentNullException("heap");

            m_addr = addr;
            m_inner = inner;
            m_heap = heap;

            // For interior pointers (structs inside other objects), we simply have to trust the caller
            // gave us the right thing.
            m_type = inner ? type : heap.GetObjectType(addr);
        }

        public ClrObject(ClrHeap heap, ClrType type, ulong addr)
        {
            if (heap == null)
                throw new ArgumentNullException("heap");

            m_addr = addr;
            m_heap = heap;


            if (addr != 0)
            {
                var gcType = heap.GetObjectType(addr);
                if (gcType != null)
                    type = gcType;
            }

            m_type = type;
        }

        public override string ToString()
        {
            if (IsNull())
                return "{null}";

            return string.Format(@"[{0:X} {1}]", m_addr, m_type.Name);
        }

        public bool IsNull()
        {
            return m_addr == 0 || m_type == null;
        }

        public ulong GetValue()
        {
            return m_addr;
        }

        public ClrType GetHeapType()
        {
            return m_type;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (IsNull())
            {
                result = new ClrNullValue(m_heap);
                return true;
            }

            if (m_type.IsArray)
            {
                // Populate length for bounds check.
                if (m_len == -1)
                    m_len = m_type.GetArrayLength(m_addr);

                int index = GetIndexFromObjects(indexes);
                return GetArrayValue(m_type, m_addr, index, out result);
            }

            // Dictionary
            if (IsDictionary())
            {
                if (indexes.Length != 1)
                    throw new ArgumentException("Only one index is allowed for Dictionary indexing.");

                dynamic dict = ((dynamic)this);
                dynamic entries = dict.entries;
                ClrInstanceField key = ((ClrType)entries).ArrayComponentType.GetFieldByName("key");

                // Two cases:  The key is an object or string, denoted by "System.__Canon", or it's a primitive.
                //             otherwise we don't support it.
                var keyType = key.Type;
                if (keyType.IsObjectReference)
                {
                    Debug.Assert(keyType.Name == "System.__Canon");
                    if (indexes[0].GetType() == typeof(string))
                    {
                        string index = (string)indexes[0];
                        int len = dict.count;
                        for (int i = 0; i < len; ++i)
                        {
                            if ((string)entries[i].key == index)
                            {
                                result = entries[i].value;
                                return true;
                            }
                        }

                        throw new KeyNotFoundException();
                    }
                    else
                    {
                        ulong addr;
                        if (indexes[0].GetType() == typeof(long))
                            addr = (ulong)(long)indexes[0];
                        else if (indexes[0].GetType() == typeof(ulong))
                            addr = (ulong)indexes[0];
                        else
                            addr = (ulong)(dynamic)indexes[0];

                        int len = dict.count;
                        for (int i = 0; i < len; ++i)
                        {
                            if ((ulong)entries[i].key == addr)
                            {
                                result = entries[i].value;
                                return true;
                            }
                        }
                    }
                }
                else if (keyType.IsPrimitive)
                {
                    object index = indexes[0];
                    if (index is ClrPrimitiveValue)
                        index = ((ClrPrimitiveValue)index).GetValue();
                    Type type = index.GetType();
                    int len = dict.count;
                    for (int i = 0; i < len; ++i)
                    {
                        ClrPrimitiveValue value = entries[i].key;
                        if (value.GetValue().Equals(index))
                        {
                            result = entries[i].value;
                            return true;
                        }
                    }

                    throw new KeyNotFoundException();
                }
            }

            if (IsList())
            {
                int index = GetIndexFromObjects(indexes);
                var itemsField = m_type.GetFieldByName("_items");
                ulong addr = (ulong)itemsField.GetValue(m_addr);

                // Populate length for bounds check.
                if (m_len == -1)
                {
                    var sizeField = m_type.GetFieldByName("_size");
                    m_len = (int)sizeField.GetValue(m_addr);
                }

                // If type is null, then we've hit a dac bug.  Attempt to work around it,
                // but we'll have to give up if getting the object type directly doesn't work.
                ClrType type = itemsField.Type;
                if (type == null || type.ArrayComponentType == null)
                {
                    type = m_heap.GetObjectType(addr);
                    if (type == null || type.ArrayComponentType == null)
                    {
                        result = new ClrNullValue(m_heap);
                        return true;
                    }
                }

                return GetArrayValue(type, addr, index, out result);
            }

            throw new InvalidOperationException(string.Format("Object of type '{0}' is not indexable.", m_type.Name));
        }

        private static int GetIndexFromObjects(object[] indexes)
        {
            // Validate input.
            if (indexes.Length != 1)
                throw new ArgumentException("Only one integer index is allowed for array indexing.");

            int index = -1;
            if (indexes[0] is int)
                index = (int)indexes[0];
            else if (indexes[0] is uint)
                index = (int)(uint)indexes[0];
            else
                throw new ArgumentException("Array index must be integer.");
            return index;
        }

        private bool GetArrayValue(ClrType type, ulong addr, int index, out object result)
        {
            var componentType = type.ArrayComponentType;

            // componentType being null is a dac bug which should only happen when we have an array of
            // value types, where we have never *actually* constructed one of the types.  If there are
            // other dac bugs which cause this, we unfortunately cannot work around it.
            if (addr == 0 || componentType == null)
            {
                result = new ClrNullValue(m_heap);
                return true;
            }

            if (index < 0 || index >= m_len)
                throw new IndexOutOfRangeException();

            // Now construct the value based on the element type.
            if (componentType.ElementType == ClrElementType.Struct)
            {
                addr = type.GetArrayElementAddress(addr, index);
                result = new ClrObject(m_heap, componentType, addr, true);
                return true;
            }
            else if (componentType.IsObjectReference)
            {
                addr = type.GetArrayElementAddress(addr, index);
                if (!m_heap.GetRuntime().ReadPointer(addr, out addr) || addr == 0)
                {
                    result = new ClrNullValue(m_heap);
                    return true;
                }
                else
                {
                    result = new ClrObject(m_heap, componentType, addr);
                    return true;
                }
            }
            else if (componentType.IsPrimitive)
            {
                result = new ClrPrimitiveValue(type.GetArrayElementValue(addr, index), componentType.ElementType);
                return true;
            }

            result = null;
            return false;
        }

        private bool IsDictionary()
        {
            return !m_type.IsArray && m_type.Name.StartsWith("System.Collections.Generic.Dictionary<") && m_type.Name.EndsWith(">");
        }

        private bool IsList()
        {
            return !m_type.IsArray && m_type.Name.StartsWith("System.Collections.Generic.List<") && m_type.Name.EndsWith(">");
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            if (m_type == null)
                return new string[] { };

            return from f in m_type.Fields select f.Name;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (m_type == null)
                return ClrNullValue.GetDefaultNullValue(binder.Type, out result);

            if (binder.Type == typeof(ClrType))
            {
                // Special case:  The caller is requesting the GCHeapType of what we are inspecting.
                result = m_type;
                return true;
            }

            if (binder.Type.IsPrimitive && m_type.IsPrimitive && m_type.HasSimpleValue)
            {
                result = Convert.ChangeType(m_type.GetValue(m_addr), binder.Type);
                return true;
            }

            if (binder.Type == typeof(ulong))
            {
                result = m_addr;
                return true;
            }

            if (binder.Type == typeof(long))
            {
                result = (long)m_addr;
                return true;
            }

            if (binder.Type == typeof(string))
            {
                if (m_type.IsString)
                    result = m_type.GetValue(m_addr);
                else
                    result = ToString();
                return true;
            }


            result = null;
            return false;
        }

        public int GetLength()
        {
            if (m_type == null)
                return -1;

            if (m_len != -1)
                return m_len;

            if (m_type.IsArray)
            {
                m_len = m_type.GetArrayLength(m_addr);
                return m_len;
            }

            if (IsDictionary())
            {
                var countField = m_type.GetFieldByName("count");
                m_len = (int)countField.GetValue(m_addr);
                return m_len;
            }

            if (IsList())
            {
                var sizeField = m_type.GetFieldByName("_size");
                m_len = (int)sizeField.GetValue(m_addr);
                return m_len;
            }

            throw new InvalidOperationException("Object does not have a length associated with it.");
        }

        public ClrType GetDictionaryKeyType()
        {
            dynamic entries = ((dynamic)this).entries;
            ClrInstanceField key = ((ClrType)entries).ArrayComponentType.GetFieldByName("key");

            if (key == null)
                return null;

            return key.Type;
        }

        public ClrType GetDictionaryValueType()
        {
            dynamic entries = ((dynamic)this).entries;
            ClrInstanceField value = ((ClrType)entries).ArrayComponentType.GetFieldByName("value");

            if (value == null)
                return null;

            return value.Type;
        }

        public IList<Tuple<dynamic, dynamic>> GetDictionaryItems()
        {
            if (m_type == null || !IsDictionary())
                throw new InvalidOperationException("Can only call GetDictionaryItems on a System.Collections.Generic.Dictionary object.");

            dynamic dict = this;
            dynamic entries = dict.entries;

            if (entries.IsNull())
                return new Tuple<dynamic, dynamic>[] { };

            int len = dict.count;
            var items = new Tuple<dynamic, dynamic>[len];
            for (int i = 0; i < len; ++i)
                items[i] = new Tuple<dynamic, dynamic>(entries[i].key, entries[i].value);

            return items;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (IsNull())
            {
                result = new ClrNullValue(m_heap);
                return true;
            }

            ClrInstanceField field = null;
            if (binder.IgnoreCase)
            {
                foreach (var inst in m_type.Fields)
                {
                    if (inst.Name.Equals(binder.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        field = inst;
                        break;
                    }
                }
            }
            else
            {
                field = m_type.GetFieldByName(binder.Name);
            }

            if (field == null)
            {
                if (ClrDynamicClass.GetStaticField(m_heap, m_type, binder, out result))
                    return true;

                throw new InvalidOperationException(string.Format("Type '{0}' does not contain a '{1}' field.", m_type.Name, binder.Name));
            }

            if (field.IsPrimitive())
            {
                object value = field.GetValue(m_addr, m_inner);
                if (value == null)
                    result = new ClrNullValue(m_heap);
                else
                    result = new ClrPrimitiveValue(value, field.ElementType);

                return true;
            }
            else if (field.IsValueClass())
            {
                ulong addr = field.GetAddress(m_addr, m_inner);
                result = new ClrObject(m_heap, field.Type, addr, true);
                return true;
            }
            else if (field.ElementType == ClrElementType.String)
            {
                ulong addr = field.GetAddress(m_addr, m_inner);
                if (!m_heap.GetRuntime().ReadPointer(addr, out addr))
                {
                    result = new ClrNullValue(m_heap);
                    return true;
                }

                result = new ClrObject(m_heap, field.Type, addr);
                return true;
            }
            else
            {
                object value = field.GetValue(m_addr, m_inner);
                if (value == null)
                    result = new ClrNullValue(m_heap);
                else
                    result = new ClrObject(m_heap, field.Type, (ulong)value);

                return true;
            }
        }


        private bool IsStringDict()
        {
            return m_type.Name.Length > 52 && m_type.Name.Substring(38, 13) == "System.String";
        }
    }
}
