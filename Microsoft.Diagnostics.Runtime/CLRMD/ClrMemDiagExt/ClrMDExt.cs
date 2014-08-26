using Microsoft.Diagnostics.Runtime;
using System.Linq;

namespace Microsoft.Diagnostics.RuntimeExt
{
    public static class ClrMemDiagExtensions
    {
        public static dynamic GetDynamicObject(this ClrHeap heap, ulong addr)
        {
            var type = heap.GetObjectType(addr);
            if (type == null)
                return null;

            return new ClrObject(heap, type, addr);
        }

        public static dynamic GetDynamicClass(this ClrHeap heap, string typeName)
        {
            ClrType type = (from t in heap.EnumerateTypes()
                               where t != null && t.Name == typeName
                               select t).FirstOrDefault();

            if (type == null)
                return null;

            return new ClrDynamicClass(heap, type);
        }
    }
}