using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.RuntimeExt
{
    public class StaticVariableValueWrapper : DynamicObject
    {
        ClrHeap m_heap;
        ClrStaticField m_field;

        public StaticVariableValueWrapper(ClrHeap heap, ClrStaticField field)
        {
            m_heap = heap;
            m_field = field;
        }

        public dynamic GetValue(ClrAppDomain appDomain)
        {
            if (m_field.IsPrimitive())
            {
                object value = m_field.GetValue(appDomain);
                if (value != null)
                    return new ClrPrimitiveValue(value, m_field.ElementType);
            }
            else if (m_field.IsValueClass())
            {
                ulong addr = m_field.GetAddress(appDomain);
                if (addr != 0)
                    return new ClrObject(m_heap, m_field.Type, addr, true);
            }
            else if (m_field.ElementType == ClrElementType.String)
            {
                ulong addr = m_field.GetAddress(appDomain);
                if (m_heap.GetRuntime().ReadPointer(addr, out addr))
                    return new ClrObject(m_heap, m_field.Type, addr);
            }
            else
            {
                object value = m_field.GetValue(appDomain);
                if (value != null)
                    return new ClrObject(m_heap, m_field.Type, (ulong)value);
            }

            return new ClrNullValue(m_heap);
        }
    }

    public class ClrDynamicClass : DynamicObject
    {
        ClrHeap m_heap;
        ClrType m_type;

        public ClrDynamicClass(ClrHeap heap, ClrType type)
        {
            if (heap == null)
                throw new ArgumentNullException("heap");

            if (type == null)
                throw new ArgumentNullException("type");

            m_heap = heap;
            m_type = type;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type == typeof(ClrType))
            {
                result = m_type;
                return true;
            }

            return base.TryConvert(binder, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (GetStaticField(m_heap, m_type, binder, out result))
                return true;

            throw new InvalidOperationException(string.Format("Type '{0}' does not contain a static '{1}' field.", m_type.Name, binder.Name));
        }

        internal static bool GetStaticField(ClrHeap heap, ClrType type, GetMemberBinder binder, out object result)
        {
            result = null;
            bool success = false;
            ClrStaticField field = null;

            StringComparison compare = binder.IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            foreach (var inst in type.StaticFields)
            {
                if (inst.Name.Equals(binder.Name, compare))
                {
                    field = inst;
                    break;
                }
            }

            if (field != null)
            {
                result = new StaticVariableValueWrapper(heap, field);
                success = true;
            }

            return success;
        }
    }
}
