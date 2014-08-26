using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Diagnostics.Runtime;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.Collections;


static class Program
{

    public static void PrintDict(ClrRuntime runtime, string args)
    {
        bool failed = false;
        ulong obj = 0;
        try
        {
            obj = Convert.ToUInt64(args, 16);
            failed = obj == 0;
        }
        catch (ArgumentException)
        {
            failed = true;
        }

        if (failed)
        {
            Console.WriteLine("Usage: !PrintDict <dictionary>");
            return;
        }

        ClrHeap heap = runtime.GetHeap();
        ClrType type = heap.GetObjectType(obj);

        if (type == null)
        {
            Console.WriteLine("Invalid object {0:X}", obj);
            return;
        }

        if (!type.Name.StartsWith("System.Collections.Generic.Dictionary"))
        {
            Console.WriteLine("Error: Expected object {0:X} to be a dictionary, instead it's of type '{1}'.");
            return;
        }

        // Get the entries field.
        ulong entries = type.GetFieldValue(obj, "entries");

        if (entries == 0)
            return;

        ClrType entryArray = heap.GetObjectType(entries);
        ClrType arrayComponent = entryArray.ArrayComponentType;
        ClrInstanceField hashCodeField = arrayComponent.GetFieldByName("hashCode");
        ClrInstanceField keyField = arrayComponent.GetFieldByName("key");
        ClrInstanceField valueField = arrayComponent.GetFieldByName("value");

        Console.WriteLine("{0,8} {1,16} : {2}", "hash", "key", "value");
        int len = entryArray.GetArrayLength(entries);
        for (int i = 0; i < len; ++i)
        {
            ulong arrayElementAddr = entryArray.GetArrayElementAddress(entries, i);

            int hashCode = (int)hashCodeField.GetFieldValue(arrayElementAddr, true);
            object key = keyField.GetFieldValue(arrayElementAddr, true);
            object value = valueField.GetFieldValue(arrayElementAddr, true);

            key = Format(heap, key);
            value = Format(heap, value);

            bool skip = key is ulong && (ulong)key == 0 && value is ulong && (ulong)value == 0;

            if (!skip)
                Console.WriteLine("{0,8:X} {1,16} : {2}", hashCode, key, value);
        }
    }

    static object Format(ClrHeap heap, object val)
    {
        if (val == null)
            return "{null}";

        if (val is ulong)
        {
            ulong addr = (ulong)val;
            var type = heap.GetObjectType(addr);
            if (type != null && type.Name == "System.String")
                return type.GetValue(addr);
            else
                return ((ulong)val).ToString("X");
        }

        return val;
    }

    static ulong GetFieldValue(this ClrType type, ulong obj, string name)
    {
        var field = type.GetFieldByName(name);
        if (field == null)
            return 0;

        object val = field.GetFieldValue(obj);
        if (val is ulong)
            return (ulong)val;

        return 0;
    }

    static void Main(string[] args)
    {
        //ClrRuntime runtime = CreateRuntime(@"C:\Users\leculver\Desktop\work\projects\rmd_test_data\dumps\v4.0.30319.239_x86.cab",
        //                                   @"C:\Users\leculver\Desktop\work\projects\rmd_test_data\dacs");
        //PrintDict(runtime, "0262b058");

        List<string> typeNames = new List<string>();
        ClrRuntime runtime = CreateRuntime(@"D:\work\03_09_ml\ml.dmp", @"D:\work\03_09_ml");
        ClrHeap heap = runtime.GetHeap();
        foreach (var type in heap.EnumerateTypes())
        {
            typeNames.Add(type.Name);
        }

        typeNames.Sort(delegate(string a, string b) { return -a.Length.CompareTo(b.Length); });

        for (int i = 0; i < 10; ++i)
            Console.WriteLine("{0} {1}", typeNames[i].Length, typeNames[i]);
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
        var version = dataTarget.ClrVersions[0];

        // Next, let's try to make sure we have the right Dac to load.  CLRVersionInfo will actually
        // have the full path to the right dac if you are debugging the a version of CLR you have installed.
        // If they gave us a path (and not the actual filename of the dac), we'll try to handle that case too:
        if (dac != null && Directory.Exists(dac))
            dac = Path.Combine(dac, version.DacInfo.FileName);
        else if (dac == null || !File.Exists(dac))
            dac = version.TryGetDacLocation();

        // Finally, check to see if the dac exists.  If not, throw an exception.
        if (dac == null || !File.Exists(dac))
            throw new FileNotFoundException("Could not find the specified dac.", dac);

        // Now that we have the DataTarget, the version of CLR, and the right dac, we create and return a
        // ClrRuntime instance.
        return dataTarget.CreateRuntime(dac);
    }
}


/*
ClrHeap heap = runtime.GetHeap();
        
List<ulong> items = new List<ulong>();
ClrType threadPoolGlobals = heap.GetTypeByName("System.Threading.ThreadPoolGlobals");

foreach (var appDomain in runtime.AppDomains)
{
    // Walk System.Threading.ThreadPoolGlobals.workQueue
    ulong workQueue = threadPoolGlobals.GetStaticFieldValue(appDomain, "workQueue");
    if (workQueue == 0)
        continue;

    ulong currSeg = heap.GetFieldValue(workQueue, "queueHead");
    while (currSeg != 0)
    {
        ulong nodes = heap.GetFieldValue(currSeg, "nodes");
        if (nodes != 0)
        {
            // I use the GCDesc to quickly walk the 'nodes' array here.  It's simpler than enumerating each index.
            var nodeType = heap.GetObjectType(nodes);
            nodeType.EnumerateRefsOfObject(nodes, delegate(ulong iThreadPoolWorkItem, int offset)
            {
                if (iThreadPoolWorkItem != 0)
                    items.Add(iThreadPoolWorkItem);
            });
        }

        currSeg = heap.GetFieldValue(currSeg, "Next");
    }
}

// Walk System.Threading.ThreadPoolGlobals.allThreadQueues
Dictionary<ClrAppDomain, List<ulong>> perThreadData = new Dictionary<ClrAppDomain, List<ulong>>();
ClrType threadPoolWorkQueue = heap.GetTypeByName("System.Threading.ThreadPoolWorkQueue");
ClrType sparseArrayType = heap.GetTypeByName("System.Threading.ThreadPoolWorkQueue.SparseArray");
ClrType workStealingQueueType = heap.GetTypeByName("System.Threading.ThreadPoolWorkQueue.WorkStealingQueue");
foreach (var appDomain in runtime.AppDomains)
{
    ulong allThreadQueues = threadPoolWorkQueue.GetStaticFieldValue(appDomain, "allThreadQueues");
    if (allThreadQueues == 0)
        continue;

    if (!perThreadData.ContainsKey(appDomain))
        perThreadData[appDomain] = new List<ulong>();

    ulong sparseArray = heap.GetFieldValue(allThreadQueues, "m_array");
    if (sparseArray == 0)
        continue;

    // Walk all items in SparseArray
    ulong sparseArrayInner = heap.GetFieldValue(sparseArray, "m_array");
    if (sparseArrayInner == 0)
        continue;

    var sparseArrayInnerType = heap.GetObjectType(sparseArrayInner);
    int length = sparseArrayInnerType.GetArrayLength(sparseArray);
    for (int i = 0; i < length; ++i)
    {
        ulong workStealingQueue = (ulong)sparseArrayType.GetArrayElementValue(sparseArray, i);
        if (workStealingQueue == 0)
            continue;

        ulong m_array = heap.GetFieldValue(workStealingQueue, "m_array");
        if (m_array != 0)
        {
            // Walk all items in the WorkStealingQueue.
            workStealingQueueType.EnumerateRefsOfObject(m_array, delegate(ulong iThreadPoolWorkItem, int offset2)
            {
                perThreadData[appDomain].Add(iThreadPoolWorkItem);
                items.Add(iThreadPoolWorkItem);
            });
        }
    }
}

// Now print out data.
}

private static ClrRuntime CreateRuntime(string crashDump, string dacLocation)
{
DataTarget target = DataTarget.LoadCrashDump(crashDump);

// We loop here because we could have both v2 and v4 in the same process.
if (!File.Exists(dacLocation))
{
    foreach (CLRVersionInfo version in target.EnumerateClrVersions())
    {
        dacLocation = Path.Combine(dacLocation, version.DacRequestFileName);
        break;
    }
}

// The ClrRuntime object is how you read everything.
ClrRuntime runtime = target.CreateRuntime(dacLocation);
return runtime;
}
}

static class Extensions
{
public static ulong GetFieldValue(this ClrHeap heap, ulong obj, string fieldName)
{
var type = heap.GetObjectType(obj);
var field = type.GetFieldByName(fieldName);
if (!field.HasSimpleValue)
    return 0;
return (ulong)field.GetFieldValue(obj);
}

public static ulong GetStaticFieldValue(this ClrType type, ClrAppDomain ad, string fieldName)
{
foreach (var sf in type.StaticFields)
    if (sf.Name == fieldName)
        return (ulong)sf.GetFieldValue(ad);

return 0;
}

public static ClrType GetTypeByName(this ClrHeap heap, string name)
{
foreach (var type in heap.EnumerateTypes())
    if (type.Name == name)
        return type;

return null;
}
}

/*
class Program
{
class Entry
{
public ClrType Type;
public int Count;
public long Size;
}

static void Main(string[] args)
{
// TODO:  Change these paths.
string baseDir = @"d:\work\03_19_janus";

Dictionary<string, Entry> first = GetHeapStats(Path.Combine(baseDir, "old.dmp"), Path.Combine(baseDir, "mscordacwks_old.dll"));
Dictionary<string, Entry> second = GetHeapStats(Path.Combine(baseDir, "new.dmp"), Path.Combine(baseDir, "mscordacwks_new.dll"));

List<Entry> diff = new List<Entry>(second.Values);

using (TextWriter writer = File.CreateText(Path.Combine(baseDir, "heapdiff.txt")))
{
    foreach (var item in diff)
    {
        Entry entry = null;
        if (first.TryGetValue(item.Type.Name, out entry))
        {
            item.Count -= entry.Count;
            item.Size -= entry.Size;
        }
    }

    foreach (var item in first.Values)
    {
        if (!second.ContainsKey(item.Type.Name))
        {
            item.Count = -item.Count;
            item.Size = -item.Size;

            diff.Add(item);
        }
    }

    diff.RemoveAll(delegate(Entry e) { return e.Count == 0; });
    diff.Sort(delegate(Entry a, Entry b) { return a.Size.CompareTo(b.Size); });

    foreach (var item in diff)
        writer.WriteLine("{0,12:n0} {1,12:n0} {2}", item.Count, item.Size, item.Type.Name);
}
}


private static Dictionary<string, Entry> GetHeapStats(string dump, string dac)
{
ClrRuntime runtime = CreateRuntime(dump, dac);
Dictionary<string, Entry> heapStats = new Dictionary<string, Entry>();
var heap = runtime.GetHeap();

foreach (var seg in heap.Segments)
{
    for (ulong obj = seg.Start; obj != 0; obj = seg.NextObject(obj))
    {
        var type = heap.GetObjectType(obj);
        Entry entry;

        if (!heapStats.TryGetValue(type.Name, out entry))
        {
            entry = new Entry();
            entry.Type = type;
            heapStats[type.Name] = entry;
        }

        entry.Size += (long)type.GetSize(obj);
        entry.Count++;
    }
}
return heapStats;
}

private static ClrRuntime CreateRuntime(string crashDump, string dacLocation)
{
DataTarget target = DataTarget.LoadCrashDump(crashDump);

// We loop here because we could have both v2 and v4 in the same process.
if (!File.Exists(dacLocation))
{
    foreach (CLRVersionInfo version in target.EnumerateClrVersions())
    {
        dacLocation = Path.Combine(dacLocation, version.DacRequestFileName);
        break;
    }
}

// The ClrRuntime object is how you read everything.
ClrRuntime runtime = target.CreateRuntime(dacLocation);
return runtime;
}
}
*/
/*
class Program
{
    class Entry
    {
        public ClrMemoryRegionType Type { get { return Regions[0].Type; } }
        public List<ClrMemoryRegion> Regions = new List<ClrMemoryRegion>();
        public string Name;
        public int Count;
        public ulong Size;
    }

    static void Main(string[] args)
    {
        // TODO:  Change these paths.
        string crashDump = @"D:\work\03_09_ml\ml_dec.dmp";
        string dacLocation = @"D:\work\03_09_ml";

        // Now create the ClrRuntime instance.
        ClrRuntime runtime = CreateRuntime(crashDump, dacLocation);
        using (TextWriter writer = File.CreateText(crashDump + ".txt"))
        {
            Dictionary<ClrMemoryRegionType, Entry> regionStats = new Dictionary<ClrMemoryRegionType, Entry>();

            writer.WriteLine("Memory statistics:");
            writer.WriteLine();
            writer.WriteLine("Address\tSize\tType");
            foreach (var mem in runtime.EnumerateMemoryRegions())
            {
                Entry entry = null;
                if (!regionStats.TryGetValue(mem.Type, out entry))
                {
                    entry = new Entry();
                    entry.Name = mem.ToString();
                    regionStats[mem.Type] = entry;
                }

                entry.Size += mem.Size;
                entry.Count++;
                entry.Regions.Add(mem);

                //writer.WriteLine("{0:X}\t{1}\t{2}", mem.Address, mem.Size, mem.ToString());
            }

            var stats = from t in regionStats.Values
                        orderby t.Size
                        select t;

            writer.WriteLine();
            writer.WriteLine("{0,16}\t{1,8}\t{2}", "Size", "Count", "Type");

            foreach (var type in stats)
                writer.WriteLine("{0,16}\t{1,8}\t{2}", type.Size, type.Count, type.Name);

            writer.WriteLine();
            writer.WriteLine("Per AppDomain:");

            PrintOneDomain(writer, regionStats, null);
            foreach (var domain in runtime.AppDomains)
                PrintOneDomain(writer, regionStats, domain);
        }
    }

    private static void PrintOneDomain(TextWriter writer, Dictionary<ClrMemoryRegionType, Entry> regionStats, ClrAppDomain domain)
    {
        writer.WriteLine();
        if (domain == null)
            writer.WriteLine("No associated domain:");
        else
            writer.WriteLine("Domain '{0}':", domain.Name);

        int totalCount = 0;
        ulong totalSize = 0;

        foreach (var entry in regionStats.Values)
        {
            var results = from t in entry.Regions
                          where domain == t.AppDomain
                          select t;

            int count = 0;
            ulong size = 0;

            foreach (var res in results)
            {
                count++;
                size += res.Size;
            }

            totalCount += count;
            totalSize += size;

            writer.WriteLine("{0,14:n0} {1,14:n0} {2}", count, size, entry.Name);
        }

        writer.WriteLine("{0,14:n0} {1,14:n0} total", totalCount, totalSize);
    }


    private static ClrRuntime CreateRuntime(string crashDump, string dacLocation)
    {
        DataTarget target = DataTarget.LoadCrashDump(crashDump);

        // We loop here because we could have both v2 and v4 in the same process.
        foreach (CLRVersionInfo version in target.EnumerateClrVersions())
        {
            dacLocation = Path.Combine(dacLocation, version.DacRequestFileName);
            break;
        }

        // The ClrRuntime object is how you read everything.
        ClrRuntime runtime = target.CreateRuntime(dacLocation);
        return runtime;
    }
}
*/
/*
class Program
{
    class EvalItem
    {
        public ulong Addr { get; set; }
        public bool FromXml { get; set; }

        public EvalItem(ulong addr, bool fromXml = false)
        {
            Addr = addr;
            FromXml = fromXml;
        }
    }

    class TypeData
    {
        public ClrType Type;
        public int Count;
        public ulong Size;
    }

    static void Main(string[] args)
    {
        // TODO:  Change these paths.
        string crashDump = @"D:\work\03_09_ml\ml2.dmp";
        string dacLocation = @"D:\work\03_09_ml";

        // Types we care about.
        HashSet<string> searchTypes = new HashSet<string>(new string[] {"System.Xml.XmlText", "System.Xml.XmlAttribute", "System.Xml.XmlElement", "System.Xml.XmlAttributeCollection"});
        string mlType = "MerrillLynch.Framework.Security.ServiceSiteMapProvider";
        
        // Evaluation stack for our algorithm.
        Stack<EvalItem> eval = new Stack<EvalItem>();

        // Now create the ClrRuntime instance, get the heap.
        ClrRuntime runtime = CreateRuntime(crashDump, dacLocation);
        ClrHeap heap = runtime.GetHeap();
        
        // Walk the heap, find all ServiceSiteMapProvider objects.
        foreach (var seg in heap.Segments)
        {
            for (ulong obj = seg.FirstObject; obj != 0; obj = seg.NextObject(obj))
            {
                ClrType type = heap.GetObjectType(obj);
                if (type != null && type.Name == mlType)
                    eval.Push(new EvalItem(obj));
            }
        }

        // The eval variable now has all SSMP objects in the dump.  Now do an !objsize
        // algorithm.
        ObjectSet considered = new ObjectSet(heap);
        Dictionary<ClrType, TypeData> heapStat = new Dictionary<ClrType, TypeData>();
        ulong totalSize = 0;
        ulong totalSizeFromXml = 0;
        uint fromXmlCount = 0;

        Stopwatch sw = new Stopwatch();
        sw.Start();

        while (eval.Count > 0)
        {
            EvalItem obj = eval.Pop();

            // Don't consider an object more than once
            if (considered.Contains((uint)obj.Addr))
                continue;
            considered.Add((uint)obj.Addr);

            ClrType type = heap.GetObjectType(obj.Addr);
            ulong size = type.GetSize(obj.Addr);
            bool fromXml = obj.FromXml || searchTypes.Contains(type.Name);

            totalSize += size;
            if (fromXml)
            {
                totalSizeFromXml += size;
                fromXmlCount++;

                TypeData typeData = null;
                if (!heapStat.TryGetValue(type, out typeData))
                {
                    typeData = new TypeData();
                    typeData.Type = type;
                    heapStat[type] = typeData;
                }

                typeData.Size += size;
                typeData.Count++;
            }

            // This enumerates all objects an object references.
            type.EnumerateRefsOfObject(obj.Addr, delegate(ulong child, int offset)
                {
                    if (!considered.Contains((uint)child))
                        eval.Push(new EvalItem(child, fromXml));
                });
        }

        long msec = sw.ElapsedMilliseconds;
        writer.WriteLine(msec);

        var types = from t in heapStat.Values
                    orderby t.Size
                    select t;

        writer.WriteLine("Size\tCount\tType");
        foreach (var type in types)
        {
            writer.WriteLine("{0}\t{1}\t{2}", type.Size, type.Count, type.Type.Name);
        }

        writer.WriteLine("Total size from ServiceSiteMapProvider:  {0} bytes in {1} objects", totalSize, considered.Count);
        writer.WriteLine("Total size from XML objects: {0} bytes in {1} objects", totalSizeFromXml, fromXmlCount);
        writer.WriteLine("Difference:  {0} bytes in {1} objects not reachable from XML objects.", totalSize - totalSizeFromXml, considered.Count - fromXmlCount);
    }

    private static ClrRuntime CreateRuntime(string crashDump, string dacLocation)
    {
        DataTarget target = DataTarget.LoadCrashDump(crashDump);

        // We loop here because we could have both v2 and v4 in the same process.
        foreach (CLRVersionInfo version in target.EnumerateClrVersions())
        {
            dacLocation = Path.Combine(dacLocation, version.DacRequestFileName);
            break;
        }

        // The ClrRuntime object is how you read everything.
        ClrRuntime runtime = target.CreateRuntime(dacLocation);
        return runtime;
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
        int offset = GetBitOffset(value - m_entries[index].Low);

        m_data[index].Set(offset, true);
    }

    public bool Contains(ulong value)
    {
        if (value == 0)
            return m_zero;


        int index = GetIndex(value);
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

        throw new Exception(string.Format("Object value {0:X} was outside the bounds of the heap.", value));
    }

    BitArray[] m_data;
    Entry[] m_entries;
    int m_shift;
    bool m_zero;
}
*/