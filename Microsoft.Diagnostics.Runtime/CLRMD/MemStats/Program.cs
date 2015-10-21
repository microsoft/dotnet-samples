// Please go to the ClrMD project page on github for full source and to report issues:
//    https://github.com/Microsoft/clrmd

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Diagnostics.Runtime;
using System.IO;

namespace EEHeap
{
    class Program
    {
        static void Main(string[] args)
        {
            string dump, dac;
            if (!TryParseArgs(args, out dump, out dac))
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

                // To get memory statistics, you can use EnumerateMemoryRegions.  This enumerates
                // the address and size of every memory region (I.E., heap) that CLR allocates.
                // You can use this information to track down what's "too large" in your process.
                Dictionary<ClrMemoryRegionType, Entry> stats = new Dictionary<ClrMemoryRegionType, Entry>();
                foreach (var region in runtime.EnumerateMemoryRegions())
                {
                    Entry entry;
                    if (!stats.TryGetValue(region.Type, out entry))
                    {
                        entry = new Entry();
                        stats[region.Type] = entry;
                    }

                    entry.Regions.Add(region);
                    entry.Size += region.Size;
                }

                // Print out total stats
                var sortedEntries = from t in stats.Values
                                    orderby t.Size
                                    select t;

                Console.WriteLine("Total stats for {0} AppDomain{1}:", runtime.AppDomains.Count, runtime.AppDomains.Count > 1 ? "s" : "");
                Console.WriteLine("{0,12} {1}", "Size", "Memory Type");

                foreach (var entry in sortedEntries)
                    Console.WriteLine("{0,12:n0} {1}", entry.Size, entry.Name);

                // Print out per-appdomain usage.  You could probably get more clever with linq here,
                // but I tried to keep this as simple as possible.
                foreach (ClrAppDomain ad in runtime.AppDomains)
                {
                    Console.WriteLine();
                    Console.WriteLine("Memory usage for AppDomain '{0}':", ad.Name);
                    foreach (Entry entry in stats.Values)
                    {
                        if (!entry.HasAppDomainData)
                            continue;

                        long size = entry.Regions.Where(p => p.AppDomain == ad).Sum(p => (uint)p.Size);
                        if (size > 0)
                            Console.WriteLine("{0,12:n0} {1}", size, entry.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception:");
                Console.WriteLine(ex);
            }
        }

        public static bool TryParseArgs(string[] args, out string dump, out string dac)
        {
            dump = null;
            dac = null;

            foreach (string arg in args)
            {
                if (dump == null)
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
            Console.WriteLine("Usage: {0} crash.dmp [dac_file_name]", fn);
        }
    }
    class Entry
    {
        public bool HasAppDomainData { get { return Regions[0].AppDomain != null; } }
        public string Name
        {
            get
            {
                // ClrMemoryRegion.ToString should do a better job with these two types...
                if (Regions[0].Type == ClrMemoryRegionType.GCSegment)
                    return "GC Segment";
                if (Regions[0].Type == ClrMemoryRegionType.ReservedGCSegment)
                    return "Reserved GC Segment";

                return Regions[0].ToString();
            }
        }
        public ulong Size;
        public List<ClrMemoryRegion> Regions = new List<ClrMemoryRegion>();
    }

}
