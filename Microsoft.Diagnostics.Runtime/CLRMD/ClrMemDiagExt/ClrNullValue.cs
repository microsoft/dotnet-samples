using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Microsoft.Diagnostics.RuntimeExt
{
    public class ClrNullValue : DynamicObject
    {
        static ClrType s_free;

        public ClrNullValue(ClrHeap heap)
        {
            foreach (var type in heap.EnumerateTypes())
            {
                s_free = type;
                break;
            }
        }

        public bool IsNull()
        {
            return true;
        }

        public ulong GetValue()
        {
            return 0;
        }

        public int GetLength()
        {
            return 0;
        }

        public ClrType GetHeapType()
        {
            return s_free;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = this;
            return true;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            return GetDefaultNullValue(binder.Type, out result);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return new string[] { };
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this;
            return true;
        }

        public static bool GetDefaultNullValue(Type type, out object result)
        {
            result = null;
            if (!type.IsValueType)
                return true;
            
            if (type.IsPrimitive && type.IsPublic)
            {
                try
                {
                    result = Activator.CreateInstance(type);
                    return true;
                }
                catch
                {
                }
            }

            return false;
        }
    }

}
