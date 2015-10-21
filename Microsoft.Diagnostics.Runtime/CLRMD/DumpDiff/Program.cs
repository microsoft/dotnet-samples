// Please go to the ClrMD project page on github for full source and to report issues:
//    https://github.com/Microsoft/clrmd

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Diagnostics.Runtime;

namespace DumpDiff
{
    class Entry
    {
        public ClrType Type;
        public int Count;
        public long Size;
    }

    class Program
    {
        static void Main(string[] args)
        {
            string dump1, dump2, dac1, dac2;
            if (!TryParseArgs(args, out dump1, out dump2, out dac1, out dac2))
            {
                Usage();
                Environment.Exit(1);
            }

            Dictionary<ClrType, Entry> first = ReadHeap(dump1, dac1);
            Dictionary<ClrType, Entry> second = ReadHeap(dump2, dac2 == null ? dac1 : dac2);

            // Merge first into second.  After this foreach, second will contain all types in both dumps,
            // Entry.Count/Size will contain the difference between the counts/sizes from the first dump
            // to the second dump.
            foreach (var item in first)
            {
                Entry entry;
                if (second.TryGetValue(item.Key, out entry))
                {
                    entry.Count -= item.Value.Count;
                    entry.Size -= item.Value.Size;
                }
                else
                {
                    item.Value.Count = -item.Value.Count;
                    item.Value.Size = -item.Value.Size;
                    second[item.Key] = item.Value;
                }
            }

            Console.WriteLine("{0,12} {1,14} {2}", "Count", "Size", "Type");
            foreach (var entry in from t in second.Values orderby t.Size select t)
                Console.WriteLine("{0,12:n0} {1,14:n0} {2}", entry.Count, entry.Size, entry.Type.Name);
        }


        private static Dictionary<ClrType, Entry> ReadHeap(string dump, string dac)
        {
            // Load one crash dump and build heap statistics.
            ClrRuntime runtime = CreateRuntime(dump, dac);
            ClrHeap heap = runtime.GetHeap();

            var entries = new Dictionary<ClrType, Entry>();

            foreach (var seg in heap.Segments)
            {
                for (ulong obj = seg.FirstObject; obj != 0; obj = seg.NextObject(obj))
                {
                    ClrType type = heap.GetObjectType(obj);
                    if (type == null)
                        continue;

                    Entry entry;
                    if (!entries.TryGetValue(type, out entry))
                    {
                        entry = new Entry();
                        entry.Type = type;
                        entries[type] = entry;
                    }

                    entry.Count++;
                    entry.Size += (long)type.GetSize(obj);
                }
            }

            return entries;
        }



        public static bool TryParseArgs(string[] args, out string dump1, out string dump2, out string dac1, out string dac2)
        {
            dump1 = null;
            dump2 = null;
            dac1 = null;
            dac2 = null;

            foreach (string arg in args)
            {
                if (dump1 == null)
                {
                    dump1 = arg;
                }
                else if (dump2 == null)
                {
                    dump2 = arg;
                }
                else if (dac1 == null)
                {
                    dac1 = arg;
                }
                else if (dac2 == null)
                {
                    dac2 = arg;
                }
                else
                {
                    Console.WriteLine("Too many arguments.");
                    return false;
                }
            }

            return dump1 != null && dump2 != null;
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

        public static void Usage()
        {
            string fn = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Console.WriteLine("Usage: {0} crash.dmp crash2.dmp [dump1_dac] [dump2_dac]", fn);
        }
    }
}
