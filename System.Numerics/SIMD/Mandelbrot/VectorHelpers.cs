using System;
using System.Numerics;

namespace Algorithms
{
    public static class VectorHelper
    {
        // Helper to construct a vector from a lambda that takes an index. It's not efficient, but I
        // think it's prettier and more succint than the corresponding for loop.
        // Don't use it on a hot code path (i.e. inside a loop)
        public static Vector<T> Create<T>(Func<int, T> creator) where T : struct
        {
            T[] data = new T[Vector<T>.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = creator(i);
            return new Vector<T>(data);
        }

        // Helper to invoke a function for each element of the vector. This is NOT fast. I just like
        // the way it looks better than a for loop. I expect that if this were provided for the core
        // SIMD API, people might get the wrong idea about performance of this method.
        // i.e. Don't use it somewhere that performance truly matters
        public static void ForEach<T>(this Vector<T> vec, Action<T, int> op) where T : struct
        {
            for (int i = 0; i < Vector<T>.Length; i++)
                op(vec[i], i);
        }
    }
}