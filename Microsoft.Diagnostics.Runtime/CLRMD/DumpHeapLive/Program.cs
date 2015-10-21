// Please go to the ClrMD project page on github for full source and to report issues:
//    https://github.com/Microsoft/clrmd

// This example shows how to determine live objects which live on the heap.  "GetLiveObjects"
// does a walk of all objects on the heap which are reachable from roots and places it into
// an ObjectSet.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Diagnostics.Runtime;

namespace DumpHeapLive
{
    class Entry
    {
        public string Name;
        public int Count;
        public ulong Size;
    }

    class Program
    {
        static void Main(string[] args)
        {
            bool stat, live;
            string dump, dac;
            if (!TryParseArgs(args, out dump, out dac, out stat, out live))
            {
                Usage();
                Environment.Exit(1);
            }

            try
            {
                ClrRuntime runtime = CreateRuntime(dump, dac);
                ClrHeap heap = runtime.GetHeap();

                ObjectSet liveObjs = null;
                if (live)
                    liveObjs = GetLiveObjects(heap);

                Dictionary<ClrType, Entry> stats = new Dictionary<ClrType, Entry>();

                if (!stat)
                    Console.WriteLine("{0,16} {1,12} {2}", "Object", "Size", "Type");

                foreach (ClrSegment seg in heap.Segments)
                {
                    for (ulong obj = seg.FirstObject; obj != 0; obj = seg.NextObject(obj))
                    {
                        if (live && !liveObjs.Contains(obj))
                            continue;

                        // This gets the type of the object.
                        ClrType type = heap.GetObjectType(obj);
                        ulong size = type.GetSize(obj);

                        // If the user didn't request "-stat", print out the object.
                        if (!stat)
                            Console.WriteLine("{0,16:X} {1,12:n0} {2}", obj, size, type.Name);

                        // Add an entry to the dictionary, if one doesn't already exist.
                        Entry entry = null;
                        if (!stats.TryGetValue(type, out entry))
                        {
                            entry = new Entry();
                            entry.Name = type.Name;
                            stats[type] = entry;
                        }

                        // Update the statistics for this object.
                        entry.Count++;
                        entry.Size += type.GetSize(obj);
                    }
                }

                // Now print out statistics.
                if (!stat)
                    Console.WriteLine();

                // We'll actually let linq do the heavy lifting.
                var sortedStats = from entry in stats.Values
                                    orderby entry.Size
                                    select entry;

                ulong totalSize = 0, totalCount = 0;
                Console.WriteLine("{0,12} {1,12} {2}", "Size", "Count", "Type");
                foreach (var entry in sortedStats)
                {
                    Console.WriteLine("{0,12:n0} {1,12:n0} {2}", entry.Size, entry.Count, entry.Name);
                    totalSize += entry.Size;
                    totalCount += (uint)entry.Count;
                }

                Console.WriteLine();
                Console.WriteLine("Total: {0:n0} bytes in {1:n0} objects", totalSize, totalCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception:");
                Console.WriteLine(ex);
            }
        }

        private static ObjectSet GetLiveObjects(ClrHeap heap)
        {
            ObjectSet considered = new ObjectSet(heap);
            Stack<ulong> eval = new Stack<ulong>();

            foreach (var root in heap.EnumerateRoots())
                eval.Push(root.Object);

            while (eval.Count > 0)
            {
                ulong obj = eval.Pop();
                if (considered.Contains(obj))
                    continue;

                considered.Add(obj);

                var type = heap.GetObjectType(obj);
                if (type == null)  // Only if heap corruption
                    continue;

                type.EnumerateRefsOfObject(obj, delegate(ulong child, int offset)
                {
                    if (child != 0 && !considered.Contains(child))
                        eval.Push(child);
                });
            }

            return considered;
        }
        
        private static ClrRuntime CreateRuntime(string dump, string dac)
        {
            // Create the data target.  This tells us the versions of CLR loaded in the target process.
            DataTarget dataTarget = DataTarget.LoadCrashDump(dump);

            // Now check bitness of our program/target:
            bool isTarget64Bit = dataTarget.PointerSize == 8;
            if (Environment.Is64BitProcess != isTarget64Bit)
                throw new Exception(string.Format("Architecture mismatch:  Process is {0} but target is {1}", Environment.Is64BitProcess ? "64 bit" : "32 bit", isTarget64Bit ? "64 bit" : "32 bit"));

            // Note I just take the first version of CLR in the process.  You can loop over every loaded
            // CLR to handle the SxS case where both v2 and v4 are loaded in the process.
            ClrInfo version = dataTarget.ClrVersions[0];

            // Next, let's try to make sure we have the right Dac to load.  Note we are doing this manually for
            // illustration.  Simply calling version.CreateRuntime with no arguments does the same steps.
            if (dac != null && Directory.Exists(dac))
                dac = Path.Combine(dac, version.DacInfo.FileName);
            else if (dac == null || !File.Exists(dac))
                dac = dataTarget.SymbolLocator.FindBinary(version.DacInfo);

            // Finally, check to see if the dac exists.  If not, throw an exception.
            if (dac == null || !File.Exists(dac))
                throw new FileNotFoundException("Could not find the specified dac.", dac);

            // Now that we have the DataTarget, the version of CLR, and the right dac, we create and return a
            // ClrRuntime instance.
            return version.CreateRuntime(dac);
        }

        public static bool TryParseArgs(string[] args, out string dump, out string dac, out bool stat, out bool live)
        {
            dump = null;
            dac = null;
            stat = false;
            live = false;

            foreach (string arg in args)
            {
                if (arg == "-stat")
                {
                    stat = true;
                }
                else if (arg == "-live")
                {
                    live = true;
                }
                else if (dump == null)
                {
                    dump = arg;
                }
                else if (dac == null)
                {
                    dac = arg;
                }
                else
                {
                    Console.WriteLine("Too many arguments.");
                    return false;
                }
            }

            return dump != null;
        }

        public static void Usage()
        {
            string fn = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Console.WriteLine("Usage: {0} [-stat] [-live] crash.dmp [dac_file_name]", fn);
        }
    }


    
    class ObjectSet
    {
        struct Entry
        {
            public ulong High;
            public ulong Low;
            public int Index;
        }

        public ObjectSet(ClrHeap heap)
        {
            m_shift = IntPtr.Size == 4 ? 3 : 4;
            int count = heap.Segments.Count;

            m_data = new BitArray[count];
            m_entries = new Entry[count];
    #if DEBUG
                ulong last = 0;
    #endif

            for (int i = 0; i < count; ++i)
            {
                var seg = heap.Segments[i];
    #if DEBUG
                    Debug.Assert(last < seg.Start);
                    last = seg.Start;
    #endif

                m_data[i] = new BitArray(GetBitOffset(seg.Length));
                m_entries[i].Low = seg.Start;
                m_entries[i].High = seg.End;
                m_entries[i].Index = i;
            }
        }

        public void Add(ulong value)
        {
            if (value == 0)
            {
                m_zero = true;
                return;
            }

            int index = GetIndex(value);
            if (index == -1)
                return;

            int offset = GetBitOffset(value - m_entries[index].Low);

            m_data[index].Set(offset, true);
        }

        public bool Contains(ulong value)
        {
            if (value == 0)
                return m_zero;


            int index = GetIndex(value);
            if (index == -1)
                return false;

            int offset = GetBitOffset(value - m_entries[index].Low);

            return m_data[index][offset];
        }

        public int Count
        {
            get
            {
                // todo, this is nasty.
                int count = 0;
                foreach (var set in m_data)
                    foreach (bool bit in set)
                        if (bit) count++;

                return count;
            }
        }

        private int GetBitOffset(ulong offset)
        {
            Debug.Assert(offset < int.MaxValue);
            return GetBitOffset((int)offset);
        }

        private int GetBitOffset(int offset)
        {
            return offset >> m_shift;
        }

        private int GetIndex(ulong value)
        {
            int low = 0;
            int high = m_entries.Length - 1;

            while (low <= high)
            {
                int mid = (low + high) >> 1;
                if (value < m_entries[mid].Low)
                    high = mid - 1;
                else if (value > m_entries[mid].High)
                    low = mid + 1;
                else
                    return mid;
            }

            // Outside of the heap.
            return -1;
        }

        BitArray[] m_data;
        Entry[] m_entries;
        int m_shift;
        bool m_zero;
    }
}
