// Please go to the ClrMD project page on github for full source and to report issues:
//    https://github.com/Microsoft/clrmd

using System;
using Microsoft.Diagnostics.Runtime;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace DumpHeap
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
            bool stat;
            string dump, dac;
            if (!TryParseArgs(args, out dump, out dac, out stat))
            {
                Usage();
                Environment.Exit(1);
            }

            try
            {
                // Create a ClrRuntime instance from the dump and dac location.  The ClrRuntime
                // object represents a version of CLR loaded in the process.  It contains data
                // such as the managed threads in the process, the AppDomains in the process,
                // the managed heap, and so on.
                ClrRuntime runtime = CreateRuntime(dump, dac);

                // Walk the entire heap and build heap statistics in "stats".
                Dictionary<ClrType, Entry> stats = new Dictionary<ClrType, Entry>();

                if (!stat)
                    Console.WriteLine("{0,16} {1,12} {2}", "Object", "Size", "Type");

                // This is the way to walk every object on the heap:  Get the ClrHeap instance
                // from the runtime.  Walk every segment in heap.Segments, and use
                // ClrSegment.FirstObject and ClrSegment.NextObject to iterate through
                // objects on that segment.
                ClrHeap heap = runtime.GetHeap();
                foreach (ClrSegment seg in heap.Segments)
                {
                    for (ulong obj = seg.FirstObject; obj != 0; obj = seg.NextObject(obj))
                    {
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

                Console.WriteLine("{0,12} {1,12} {2}", "Size", "Count", "Type");
                foreach (var entry in sortedStats)
                    Console.WriteLine("{0,12:n0} {1,12:n0} {2}", entry.Size, entry.Count, entry.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception:");
                Console.WriteLine(ex);
            }
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

        public static bool TryParseArgs(string[] args, out string dump, out string dac, out bool stat)
        {
            dump = null;
            dac = null;
            stat = false;

            foreach (string arg in args)
            {
                if (arg == "-stat")
                {
                    stat = true;
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
            Console.WriteLine("Usage: {0} [-stat] crash.dmp [dac_file_name]", fn);
        }
    }
}
