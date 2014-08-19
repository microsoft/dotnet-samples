using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;

namespace Microsoft.Diagnostics.RuntimeExt
{
    [ComVisible(true)]
    [Guid("7505BB76-73B1-11E1-BAD9-E6174924019B")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(IMDActivator))]
    public class CLRMDActivator : IMDActivator
    {
        public void CreateFromCrashDump(string crashdump, out IMDTarget ppTarget)
        {
            ppTarget = new MDTarget(crashdump);
        }

        public void CreateFromIDebugClient(object iDebugClient, out IMDTarget ppTarget)
        {
            ppTarget = new MDTarget(iDebugClient);
        }
    }

    class MDInterface : IMDInterface
    {
        ClrInterface m_heapint;
        public MDInterface(ClrInterface heapint)
        {
            m_heapint = heapint;
        }

        public void GetName(out string pName)
        {
            pName = m_heapint.Name;
        }

        public void GetBaseInterface(out IMDInterface ppBase)
        {
            if (m_heapint.BaseInterface != null)
                ppBase = new MDInterface(m_heapint.BaseInterface);
            else
                ppBase = null;
        }
    }

    class MDHandle : IMDHandle
    {
        ClrHandle m_handle;
        public MDHandle(ClrHandle handle)
        {
            m_handle = handle;
        }

        public void GetHandleData(out ulong pAddr, out ulong pObjRef, out MDHandleTypes pType)
        {
            pAddr = m_handle.Address;
            pObjRef = m_handle.Object;
            pType = (MDHandleTypes)m_handle.HandleType;
        }

        public void IsStrong(out int pStrong)
        {
            pStrong = m_handle.Strong ? 1 : 0;
        }

        public void GetRefCount(out int pRefCount)
        {
            pRefCount = (int)m_handle.RefCount;
        }

        public void GetDependentTarget(out ulong pTarget)
        {
            pTarget = m_handle.DependentTarget;
        }

        public void GetAppDomain(out IMDAppDomain ppDomain)
        {
            if (m_handle.AppDomain != null)
                ppDomain = new MDAppDomain(m_handle.AppDomain);
            else
                ppDomain = null;
        }
    }

    class MDRoot : IMDRoot
    {
        ClrRoot m_root;
        public MDRoot(ClrRoot root)
        {
            m_root = root;
        }

        public void GetRootInfo(out ulong pAddress, out ulong pObjRef, out MDRootType pType)
        {
            pAddress = m_root.Address;
            pObjRef = m_root.Object;
            pType = (MDRootType)m_root.Kind;
        }

        public void GetType(out IMDType ppType)
        {
            ppType = null;
        }

        public void GetName(out string ppName)
        {
            ppName = m_root.Name;
        }

        public void GetAppDomain(out IMDAppDomain ppDomain)
        {
            if (m_root.AppDomain != null)
                ppDomain = new MDAppDomain(m_root.AppDomain);
            else
                ppDomain = null;
        }
    }

    class MDAppDomain : IMDAppDomain
    {
        ClrAppDomain m_appDomain;
        public MDAppDomain(ClrAppDomain ad)
        {
            m_appDomain = ad;
        }

        public void GetName(out string pName)
        {
            pName = m_appDomain.Name;
        }

        public void GetID(out int pID)
        {
            pID = m_appDomain.Id;
        }

        public void GetAddress(out ulong pAddress)
        {
            pAddress = m_appDomain.Address;
        }
    }

    class MDSegment : IMDSegment
    {
        ClrSegment m_seg;
        public MDSegment(ClrSegment seg)
        {
            m_seg = seg;
        }

        public void GetStart(out ulong pAddress)
        {
            pAddress = m_seg.Start;
        }

        public void GetEnd(out ulong pAddress)
        {
            pAddress = m_seg.End;
        }

        public void GetReserveLimit(out ulong pAddress)
        {
            pAddress = m_seg.ReservedEnd;
        }

        public void GetCommitLimit(out ulong pAddress)
        {
            pAddress = m_seg.CommittedEnd;
        }

        public void GetLength(out ulong pLength)
        {
            pLength = m_seg.Length;
        }

        public void GetProcessorAffinity(out int pProcessor)
        {
            pProcessor = m_seg.ProcessorAffinity;
        }

        public void IsLarge(out int pLarge)
        {
            pLarge = m_seg.Large ? 1 : 0;
        }

        public void IsEphemeral(out int pEphemeral)
        {
            pEphemeral = m_seg.Ephemeral ? 1 : 0;
        }

        public void GetGen0Info(out ulong pStart, out ulong pLen)
        {
            pStart = m_seg.Gen0Start;
            pLen = m_seg.Gen0Length;
        }

        public void GetGen1Info(out ulong pStart, out ulong pLen)
        {
            pStart = m_seg.Gen1Start;
            pLen = m_seg.Gen1Length;
        }

        public void GetGen2Info(out ulong pStart, out ulong pLen)
        {
            pStart = m_seg.Gen2Start;
            pLen = m_seg.Gen2Length;
        }

        public void EnumerateObjects(out IMDObjectEnum ppEnum)
        {
            List<ulong> refs = new List<ulong>();
            for (ulong obj = m_seg.FirstObject; obj != 0; obj = m_seg.NextObject(obj))
                refs.Add(obj);

            ppEnum = new MDObjectEnum(refs);
        }
    }

    class MDMemoryRegion : IMDMemoryRegion
    {
        ClrMemoryRegion m_region;
        public MDMemoryRegion(ClrMemoryRegion region)
        {
            m_region = region;
        }

        public void GetRegionInfo(out ulong pAddress, out ulong pSize, out MDMemoryRegionType pType)
        {
            pAddress = m_region.Address;
            pSize = m_region.Size;
            pType = (MDMemoryRegionType)m_region.Type;
        }

        public void GetAppDomain(out IMDAppDomain ppDomain)
        {
            if (m_region.AppDomain != null)
                ppDomain = new MDAppDomain(m_region.AppDomain);
            else
                ppDomain = null;
        }

        public void GetModule(out string pModule)
        {
            pModule = m_region.Module;
        }

        public void GetHeapNumber(out int pHeap)
        {
            pHeap = m_region.HeapNumber;
        }

        public void GetDisplayString(out string pName)
        {
            pName = m_region.ToString(true);
        }

        public void GetSegmentType(out MDSegmentType pType)
        {
            pType = (MDSegmentType)m_region.GCSegmentType;
        }
    }

    class MDThread : IMDThread
    {
        ClrThread m_thread;
        public MDThread(ClrThread thread)
        {
            m_thread = thread;
        }

        public void GetAddress(out ulong pAddress)
        {
            pAddress = m_thread.Address;
        }

        public void IsFinalizer(out int pIsFinalizer)
        {
            pIsFinalizer = m_thread.IsFinalizer ? 1 : 0;
        }

        public void IsAlive(out int pIsAlive)
        {
            pIsAlive = m_thread.IsAlive ? 1 : 0;
        }

        public void GetOSThreadId(out int pOSThreadId)
        {
            pOSThreadId = (int)m_thread.OSThreadId;
        }

        public void GetAppDomainAddress(out ulong pAppDomain)
        {
            pAppDomain = m_thread.AppDomain;
        }

        public void GetLockCount(out int pLockCount)
        {
            pLockCount = (int)m_thread.LockCount;
        }

        public void GetCurrentException(out IMDException ppException)
        {
            if (m_thread.CurrentException != null)
                ppException = new MDException(m_thread.CurrentException);
            else
                ppException = null;
        }

        public void GetTebAddress(out ulong pTeb)
        {
            pTeb = m_thread.Teb;
        }

        public void GetStackLimits(out ulong pBase, out ulong pLimit)
        {
            pBase = m_thread.StackBase;
            pLimit = m_thread.StackLimit;
        }

        public void EnumerateStackTrace(out IMDStackTraceEnum ppEnum)
        {
            ppEnum = new MDStackTraceEnum(m_thread.StackTrace);
        }
    }

    class MDRCW : IMDRCW
    {
        RcwData m_rcw;
        public MDRCW(RcwData rcw)
        {
            m_rcw = rcw;
        }

        public void GetIUnknown(out ulong pIUnk)
        {
            pIUnk = m_rcw.IUnknown;
        }

        public void GetObject(out ulong pObject)
        {
            pObject = m_rcw.Object;
        }

        public void GetRefCount(out int pRefCnt)
        {
            pRefCnt = m_rcw.RefCount;
        }

        public void GetVTable(out ulong pHandle)
        {
            pHandle = m_rcw.VTablePointer;
        }

        public void IsDisconnected(out int pDisconnected)
        {
            pDisconnected = m_rcw.Disconnected ? 1 : 0;
        }

        public void EnumerateInterfaces(out IMDCOMInterfaceEnum ppEnum)
        {
            ppEnum = new MDCOMInterfaceEnum(m_rcw.Interfaces);
        }
    }

    class MDCCW : IMDCCW
    {
        CcwData m_ccw;
        public MDCCW(CcwData ccw)
        {
            m_ccw = ccw;
        }

        public void GetIUnknown(out ulong pIUnk)
        {
            pIUnk = m_ccw.IUnknown;
        }

        public void GetObject(out ulong pObject)
        {
            pObject = m_ccw.Object;
        }

        public void GetHandle(out ulong pHandle)
        {
            pHandle = m_ccw.Handle;
        }

        public void GetRefCount(out int pRefCnt)
        {
            pRefCnt = m_ccw.RefCount;
        }

        public void EnumerateInterfaces(out IMDCOMInterfaceEnum ppEnum)
        {
            ppEnum = new MDCOMInterfaceEnum(m_ccw.Interfaces);
        }
    }

    class MDStaticField : IMDStaticField
    {
        ClrStaticField m_field;
        public MDStaticField(ClrStaticField field)
        {
            m_field = field;
        }

        public void GetName(out string pName)
        {
            pName = m_field.Name;
        }

        public void GetType(out IMDType ppType)
        {
            ppType = MDType.Construct(m_field.Type);
        }

        public void GetElementType(out int pCET)
        {
            pCET = (int)m_field.ElementType;
        }

        public void GetSize(out int pSize)
        {
            pSize = m_field.Size;
        }

        public void GetFieldValue(IMDAppDomain appDomain, out IMDValue ppValue)
        {
            object value = m_field.GetFieldValue((ClrAppDomain)appDomain);
            ppValue = new MDValue(value, m_field.ElementType);
        }

        public void GetFieldAddress(IMDAppDomain appDomain, out ulong pAddress)
        {
            ulong addr = m_field.GetFieldAddress((ClrAppDomain)appDomain);
            pAddress = addr;
        }
    }

    class MDThreadStaticField : IMDThreadStaticField
    {
        ClrThreadStaticField m_field;
        public MDThreadStaticField(ClrThreadStaticField field)
        {
            m_field = field;
        }

        public void GetName(out string pName)
        {
            pName = m_field.Name;
        }

        public void GetType(out IMDType ppType)
        {
            ppType = MDType.Construct(m_field.Type);
        }

        public void GetElementType(out int pCET)
        {
            pCET = (int)m_field.ElementType;
        }

        public void GetSize(out int pSize)
        {
            pSize = m_field.Size;
        }

        public void GetFieldValue(IMDAppDomain appDomain, IMDThread thread, out IMDValue ppValue)
        {
            object value = m_field.GetFieldValue((ClrAppDomain)appDomain, (ClrThread)thread);
            ppValue = new MDValue(value, m_field.ElementType);
        }

        public void GetFieldAddress(IMDAppDomain appDomain, IMDThread thread, out ulong pAddress)
        {
            pAddress = m_field.GetFieldAddress((ClrAppDomain)appDomain, (ClrThread)thread);
        }
    }

    class MDField : IMDField
    {
        ClrInstanceField m_field;
        public MDField(ClrInstanceField field)
        {
            m_field = field;
        }

        public void GetName(out string pName)
        {
            pName = m_field.Name;
        }

        public void GetType(out IMDType ppType)
        {
            ppType = MDType.Construct(m_field.Type);
        }

        public void GetElementType(out int pCET)
        {
            pCET = (int)m_field.ElementType;
        }

        public void GetSize(out int pSize)
        {
            pSize = m_field.Size;
        }

        public void GetOffset(out int pOffset)
        {
            pOffset = m_field.Offset;
        }

        public void GetFieldValue(ulong objRef, int interior, out IMDValue ppValue)
        {
            object value = m_field.GetFieldValue(objRef, interior != 0);
            ppValue = new MDValue(value, m_field.ElementType);
        }

        public void GetFieldAddress(ulong objRef, int interior, out ulong pAddress)
        {
            pAddress = m_field.GetFieldAddress(objRef, interior != 0);
        }
    }

    class MDType : IMDType
    {
        public static IMDType Construct(ClrType type)
        {
            if (type == null)
                return null;

            return new MDType(type);
        }

        ClrType m_type;
        public MDType(ClrType type)
        {
            m_type = type;

            if (type == null)
                throw new NullReferenceException();
        }

        public void GetName(out string pName)
        {
            pName = m_type.Name;
        }

        public void GetSize(ulong objRef, out ulong pSize)
        {
            pSize = m_type.GetSize(objRef);
        }

        public void ContainsPointers(out int pContainsPointers)
        {
            pContainsPointers = m_type.ContainsPointers ? 1 : 0;
        }

        public void GetCorElementType(out int pCET)
        {
            pCET = (int)m_type.ElementType;
        }

        public void GetBaseType(out IMDType ppBaseType)
        {
            ppBaseType = Construct(m_type.BaseType);
        }

        public void GetArrayComponentType(out IMDType ppArrayComponentType)
        {
            ppArrayComponentType = Construct(m_type.ArrayComponentType);
        }

        public void GetCCW(ulong addr, out IMDCCW ppCCW)
        {
            if (m_type.IsCCW(addr))
                ppCCW = new MDCCW(m_type.GetCCWData(addr));
            else
                ppCCW = null;
        }

        public void GetRCW(ulong addr, out IMDRCW ppRCW)
        {
            if (m_type.IsRCW(addr))
                ppRCW = new MDRCW(m_type.GetRCWData(addr));
            else
                ppRCW = null;
        }

        public void IsArray(out int pIsArray)
        {
            pIsArray = m_type.IsArray ? 1 : 0;
        }

        public void IsFree(out int pIsFree)
        {
            pIsFree = m_type.IsFree ? 1 : 0;
        }

        public void IsException(out int pIsException)
        {
            pIsException = m_type.IsException ? 1 : 0;
        }

        public void IsEnum(out int pIsEnum)
        {
            pIsEnum = m_type.IsEnum ? 1 : 0;
        }

        public void GetEnumElementType(out int pValue)
        {
            pValue = (int)m_type.GetEnumElementType();
        }

        public void GetEnumNames(out IMDStringEnum ppEnum)
        {
            ppEnum = new MDStringEnum(m_type.GetEnumNames().ToArray());
        }

        public void GetEnumValueInt32(string name, out int pValue)
        {
            if (!m_type.TryGetEnumValue(name, out pValue))
                new InvalidOperationException("Mismatched type.");
        }

        public void GetFieldCount(out int pCount)
        {
            pCount = m_type.Fields.Count;
        }

        public void GetField(int index, out IMDField ppField)
        {
            ppField = new MDField(m_type.Fields[index]);
        }

        public int GetFieldData(ulong obj, int interior, int count, MD_FieldData[] fields, out int pNeeded)
        {
            int total = m_type.Fields.Count;
            if (fields == null || count == 0)
            {
                pNeeded = total;
                return 1; // S_FALSE
            }

            for (int i = 0; i < count && i < total; ++i)
            {
                var field = m_type.Fields[i];
                fields[i].name = field.Name;
                fields[i].type = field.Type.Name;
                fields[i].offset = field.Offset;
                fields[i].size = field.Size;
                fields[i].corElementType = (int)field.ElementType;

                if (field.ElementType == ClrElementType.Struct ||
                    field.ElementType == ClrElementType.String ||
                    field.ElementType == ClrElementType.Float ||
                    field.ElementType == ClrElementType.Double)
                {
                    fields[i].value = field.GetFieldAddress(obj, interior != 0);
                }
                else
                {
                    object value = field.GetFieldValue(obj, interior != 0);

                    if (value == null)
                    {
                        fields[i].value = 0;
                    }
                    else
                    {
                        if (value is int)
                            fields[i].value = (ulong)(int)value;
                        else if (value is uint)
                            fields[i].value = (uint)value;
                        else if (value is long)
                            fields[i].value = (ulong)(long)value;
                        else if (value is ulong)
                            fields[i].value = (ulong)value;
                        else if (value is byte)
                            fields[i].value = (ulong)(byte)value;
                        else if (value is sbyte)
                            fields[i].value = (ulong)(sbyte)value;
                        else if (value is ushort)
                            fields[i].value = (ulong)(ushort)value;
                        else if (value is short)
                            fields[i].value = (ulong)(short)value;
                        else if (value is bool)
                            fields[i].value = ((bool)value) ? (ulong)1 : (ulong)0;

                    }
                }
            }

            if (count < total)
            {
                pNeeded = count;
                return 1; // S_FALSE
            }

            pNeeded = total;
            return 0; // S_OK;
        }

        public void GetStaticFieldCount(out int pCount)
        {
            pCount = m_type.StaticFields.Count;
        }

        public void GetStaticField(int index, out IMDStaticField ppStaticField)
        {
            ppStaticField = new MDStaticField(m_type.StaticFields[index]);
        }

        public void GetThreadStaticFieldCount(out int pCount)
        {
            pCount = m_type.ThreadStaticFields.Count;
        }

        public void GetThreadStaticField(int index, out IMDThreadStaticField ppThreadStaticField)
        {
            ppThreadStaticField = new MDThreadStaticField(m_type.ThreadStaticFields[index]);
        }

        public void GetArrayLength(ulong objRef, out int pLength)
        {
            pLength = m_type.GetArrayLength(objRef);
        }

        public void GetArrayElementAddress(ulong objRef, int index, out ulong pAddr)
        {
            pAddr = m_type.GetArrayElementAddress(objRef, index);
        }

        public void GetArrayElementValue(ulong objRef, int index, out IMDValue ppValue)
        {
            object value = m_type.GetArrayElementValue(objRef, index);
            ClrElementType elementType = m_type.ArrayComponentType != null ? m_type.ArrayComponentType.ElementType : ClrElementType.Unknown;
            ppValue = new MDValue(value, elementType);
        }

        public void EnumerateReferences(ulong objRef, out IMDReferenceEnum ppEnum)
        {
            List<MD_Reference> refs = new List<MD_Reference>();
            m_type.EnumerateRefsOfObject(objRef, delegate(ulong child, int offset)
            {
                if (child != 0)
                {
                    MD_Reference r = new MD_Reference();
                    r.address = child;
                    r.offset = offset;
                    refs.Add(r);
                }
            });


            ppEnum = new ReferenceEnum(refs);
        }

        public void EnumerateInterfaces(out IMDInterfaceEnum ppEnum)
        {
            ppEnum = new InterfaceEnum(m_type.Interfaces);
        }
    }

    class MDException : IMDException
    {
        private ClrException m_ex;

        public MDException(ClrException ex)
        {
            m_ex = ex;
        }

        void IMDException.GetGCHeapType(out IMDType ppType)
        {
            ppType = new MDType(m_ex.Type);
        }

        void IMDException.GetMessage(out string pMessage)
        {
            pMessage = m_ex.Message;
        }

        void IMDException.GetObjectAddress(out ulong pAddress)
        {
            pAddress = m_ex.Address;
        }

        void IMDException.GetInnerException(out IMDException ppException)
        {
            if (m_ex.Inner != null)
                ppException = new MDException(m_ex.Inner);
            else
                ppException = null;
        }

        void IMDException.GetHRESULT(out int pHResult)
        {
            pHResult = m_ex.HResult;
        }

        void IMDException.EnumerateStackFrames(out IMDStackTraceEnum ppEnum)
        {
            ppEnum = new MDStackTraceEnum(m_ex.StackTrace);
        }
    }

    class MDHeap : IMDHeap
    {
        ClrHeap m_heap;
        public MDHeap(ClrHeap heap)
        {
            m_heap = heap;
        }

        public void GetObjectType(ulong addr, out IMDType ppType)
        {
            ppType = new MDType(m_heap.GetObjectType(addr));
        }

        public void GetExceptionObject(ulong addr, out IMDException ppExcep)
        {
            ppExcep = new MDException(m_heap.GetExceptionObject(addr));
        }

        public void EnumerateRoots(out IMDRootEnum ppEnum)
        {
            ppEnum = new MDRootEnum(new List<ClrRoot>(m_heap.EnumerateRoots()));
        }

        public void EnumerateSegments(out IMDSegmentEnum ppEnum)
        {
            ppEnum = new MDSegmentEnum(m_heap.Segments);
        }
    }

    class MDRuntime : IMDRuntime
    {
        ClrRuntime m_runtime;
        public MDRuntime(ClrRuntime runtime)
        {
            m_runtime = runtime;
        }

        public void IsServerGC(out int pServerGC)
        {
            pServerGC = m_runtime.ServerGC ? 1 : 0;
        }

        public void GetHeapCount(out int pHeapCount)
        {
            pHeapCount = m_runtime.HeapCount;
        }

        public int ReadVirtual(ulong addr, byte[] buffer, int requested, out int pRead)
        {
            int read;
            bool success = m_runtime.ReadMemory(addr, buffer, requested, out read);

            pRead = (int)read;
            return success ? 0 : -1;
        }

        public int ReadPtr(ulong addr, out ulong pValue)
        {
            bool success = m_runtime.ReadPointer(addr, out pValue);
            return success ? 1 : 0;
        }

        public void Flush()
        {
            m_runtime.Flush();
        }

        public void GetHeap(out IMDHeap ppHeap)
        {
            ppHeap = new MDHeap(m_runtime.GetHeap());
        }

        public void EnumerateAppDomains(out IMDAppDomainEnum ppEnum)
        {
            ppEnum = new MDAppDomainEnum(m_runtime.AppDomains);
        }

        public void EnumerateThreads(out IMDThreadEnum ppEnum)
        {
            ppEnum = new MDThreadEnum(m_runtime.Threads);
        }

        public void EnumerateFinalizerQueue(out IMDObjectEnum ppEnum)
        {
            ppEnum = new MDObjectEnum(new List<ulong>(m_runtime.EnumerateFinalizerQueue()));
        }

        public void EnumerateGCHandles(out IMDHandleEnum ppEnum)
        {
            ppEnum = new MDHandleEnum(m_runtime.EnumerateHandles());
        }

        public void EnumerateMemoryRegions(out IMDMemoryRegionEnum ppEnum)
        {
            ppEnum = new MDMemoryRegionEnum(new List<ClrMemoryRegion>(m_runtime.EnumerateMemoryRegions()));
        }
    }

    class MDRuntimeInfo : IMDRuntimeInfo
    {
        ClrInfo m_info;

        public MDRuntimeInfo(ClrInfo info)
        {
            m_info = info;
        }

        public void GetRuntimeVersion(out string pVersion)
        {
            pVersion = m_info.ToString();
        }

        public void GetDacLocation(out string pLocation)
        {
            pLocation = m_info.TryGetDacLocation();
        } 

        public void GetDacRequestData(out int pTimestamp, out int pFilesize)
        {
            pTimestamp = (int)m_info.DacInfo.TimeStamp;
            pFilesize = (int)m_info.DacInfo.FileSize;
        }

        public void GetDacRequestFilename(out string pRequestFileName)
        {
            pRequestFileName = m_info.DacInfo.FileName;
        }
    }

    class MDTarget : IMDTarget
    {
        DataTarget m_target;

        public MDTarget(string crashdump)
        {
            // TODO: Complete member initialization
            m_target = DataTarget.LoadCrashDump(crashdump);
        }

        public MDTarget(object iDebugClient)
        {
            m_target = DataTarget.CreateFromDebuggerInterface((IDebugClient)iDebugClient);
        }

        public void GetRuntimeCount(out int pCount)
        {
            pCount = m_target.ClrVersions.Count;
        }

        public void GetRuntimeInfo(int num, out IMDRuntimeInfo ppInfo)
        {
            ppInfo = new MDRuntimeInfo(m_target.ClrVersions[num]);
        }

        public void GetPointerSize(out int pPointerSize)
        {
            pPointerSize = (int)m_target.PointerSize;
        }

        public void CreateRuntimeFromDac(string dacLocation, out IMDRuntime ppRuntime)
        {
            ppRuntime = new MDRuntime(m_target.CreateRuntime(dacLocation));
        }

        public void CreateRuntimeFromIXCLR(object ixCLRProcess, out IMDRuntime ppRuntime)
        {
            ppRuntime = new MDRuntime(m_target.CreateRuntime(ixCLRProcess));
        }
    }

    static class HRESULTS
    {
        internal const int E_FAIL = -1;
        internal const int S_OK = 0;
        internal const int S_FALSE = 1;
    }

    class MDStringEnum : IMDStringEnum
    {
        IList<string> m_data;
        int m_curr;

        public MDStringEnum(IList<string> strings)
        {
            m_data = strings;
        }

        public void GetCount(out int pCount)
        {
            pCount = m_data.Count;
        }

        public void Reset()
        {
            m_curr = 0;
        }

        public int Next(out string pValue)
        {
            if (m_curr < m_data.Count)
            {
                pValue = m_data[m_curr];
                m_curr++;
                return HRESULTS.S_OK;
            }

            pValue = null;
            if (m_curr == m_data.Count)
            {
                m_curr++;
                return HRESULTS.S_FALSE;
            }

            return HRESULTS.E_FAIL;
        }
    }

    class InterfaceEnum : IMDInterfaceEnum
    {
        private IList<ClrInterface> m_data;
        private int m_curr;
        public InterfaceEnum(IList<ClrInterface> interfaces)
        {
            m_data = interfaces;
        }

        public void GetCount(out int pCount)
        {
            pCount = m_data.Count;
        }

        public void Reset()
        {
            m_curr = 0;
        }

        public int Next(out IMDInterface pValue)
        {
            if (m_curr < m_data.Count)
            {
                pValue = new MDInterface(m_data[m_curr]);
                m_curr++;
                return HRESULTS.S_OK;
            }

            pValue = null;
            if (m_curr == m_data.Count)
            {
                m_curr++;
                return HRESULTS.S_FALSE;
            }

            return HRESULTS.E_FAIL;
        }
    }

    class MDCOMInterfaceEnum : IMDCOMInterfaceEnum
    {
        IList<ComInterfaceData> m_data;
        int m_curr;

        public MDCOMInterfaceEnum(IList<ComInterfaceData> data)
        {
            m_data = data;
        }

        public void GetCount(out int pCount)
        {
            pCount = m_data.Count;
        }

        public void Reset()
        {
            m_curr = 0;
        }

        public int Next(IMDType pType, out ulong pInterfacePtr)
        {
            if (m_curr < m_data.Count)
            {
                pType = new MDType(m_data[m_curr].Type);
                pInterfacePtr = m_data[m_curr].InterfacePointer;
                m_curr++;
                return HRESULTS.S_OK;
            }

            pType = null;
            pInterfacePtr = 0;

            if (m_curr == m_data.Count)
            {
                m_curr++;
                return HRESULTS.S_FALSE;
            }

            return HRESULTS.E_FAIL;
        }
    }

    class MDValue : IMDValue
    {
        object m_value;
        ClrElementType m_cet;

        public MDValue(object value, ClrElementType cet)
        {
            m_value = value;
            m_cet = cet;

            if (m_value == null)
                m_cet = ClrElementType.Unknown;

            switch (m_cet)
            {
                case ClrElementType.NativeUInt:  // native unsigned int
                case ClrElementType.Pointer:
                case ClrElementType.FunctionPointer:
                    m_cet = ClrElementType.UInt64;
                    break;
                   
                case ClrElementType.String:
                    if (m_value == null)
                        m_cet = ClrElementType.Unknown;
                    break;
        
                case ClrElementType.Class:
                case ClrElementType.Array:
                case ClrElementType.SZArray:
                    m_cet = ClrElementType.Object;
                    break;
            }
        }

        public void IsNull(out int pNull)
        {
            pNull = (m_cet == ClrElementType.Unknown || m_value == null ) ? 1 : 0;
        }


        public void GetElementType(out int pCET)
        {
            pCET = (int)m_cet;
        }

        public void GetInt32(out int pValue)
        {
            ulong value = GetValue64();
            pValue = (int)value;
        }

        private ulong GetValue64()
        {
            if (m_value is int)
                return (ulong)(int)m_value;

            else if (m_value is uint)
                return (ulong)(uint)m_value;
            
            else if (m_value is long)
                return (ulong)(long)m_value;

            return (ulong)m_value;
        }

        public void GetUInt32(out uint pValue)
        {
            ulong value = GetValue64();
            pValue = (uint)value;
        }

        public void GetInt64(out long pValue)
        {
            ulong value = GetValue64();
            pValue = (long)value;
        }

        public void GetUInt64(out ulong pValue)
        {
            ulong value = GetValue64();
            pValue = (ulong)value;
        }

        public void GetString(out string pValue)
        {
            pValue = (string)m_value;
        }


        public void GetBool(out int pBool)
        {
            if (m_value is bool)
                pBool = ((bool)m_value) ? 1 : 0;
            else
                pBool = (int)GetValue64();
        }
    }

    class ObjectSegmentEnum : IMDObjectEnum
    {
        ClrSegment m_seg;
        ulong m_obj = 0;
        bool m_done = false;

        public ObjectSegmentEnum(ClrSegment seg)
        {
            m_seg = seg;
            m_obj = seg.FirstObject;
        }

        public int Next(int count, ulong[] refs, out int pWrote)
        {
            if (m_obj == 0 && !m_done)
            {
                m_done = true;
                pWrote = 0;
                return HRESULTS.S_FALSE;
            }

            if (m_done)
            {
                pWrote = 0;
                return HRESULTS.E_FAIL;
            }

            int wrote = 0;

            while (wrote < count && m_obj != 0)
            {
                refs[wrote++] = m_obj;
                m_obj = m_seg.NextObject(m_obj);
            }

            pWrote = wrote;
            if (wrote < count || m_obj == 0)
            {
                m_done = true;
                return HRESULTS.S_FALSE;
            }

            return HRESULTS.S_OK;
        }

        public void GetCount(out int pCount)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            m_obj = 0;
            m_done = true;
        }
    }

    class MDObjectEnum : IMDObjectEnum
    {
        IList<ulong> m_refs;
        int m_curr;

        public MDObjectEnum(IList<ulong> refs)
        {
            m_refs = refs;
        }

        public void GetCount(out int pCount)
        {
            pCount = m_refs.Count;
        }

        public int Next(int count, ulong[] refs, out int pWrote)
        {
            if (m_curr == m_refs.Count)
            {
                m_curr = m_refs.Count + 1;
                pWrote = 0;
                return HRESULTS.S_FALSE;
            }
            else if (m_curr > m_refs.Count)
            {
                pWrote = 0;
                return HRESULTS.E_FAIL;
            }

            int i;
            for (i = 0; m_curr < m_refs.Count && i < count; ++i, ++m_curr)
                refs[i] = m_refs[m_curr];

            pWrote = i;
            if (i != count)
            {
                m_curr = m_refs.Count + 1;
                return HRESULTS.S_FALSE;
            }

            return HRESULTS.S_OK;
        }

        public void Reset()
        {
            m_curr = 0;
        }
    }

    class MDAppDomainEnum : IMDAppDomainEnum
    {
        IList<ClrAppDomain> m_refs;
        int m_curr;

        public MDAppDomainEnum(IList<ClrAppDomain> refs)
        {
            m_refs = refs;
        }

        public void GetCount(out int pCount)
        {
            pCount = m_refs.Count;
        }

        public int Next(out IMDAppDomain ppAppDomain)
        {
            if (m_curr < m_refs.Count)
            {
                ppAppDomain = new MDAppDomain(m_refs[m_curr++]);
                return HRESULTS.S_OK;
            }
            else if (m_curr == m_refs.Count)
            {
                m_curr++;
                ppAppDomain = null;
                return HRESULTS.S_FALSE;
            }

            ppAppDomain = null;
            return HRESULTS.E_FAIL;
        }

        public void Reset()
        {
            m_curr = 0;
        }
    }

    class ReferenceEnum : IMDReferenceEnum
    {
        private IList<MD_Reference> m_refs;
        int m_curr;
        public ReferenceEnum(IList<MD_Reference> refs)
        {
            m_refs = refs;
        }

        public void GetCount(out int pCount)
        {
            pCount = m_refs.Count;
        }

        public void Reset()
        {
            m_curr = 0;
        }

        public int Next(int count, MD_Reference[] refs, out int pWrote)
        {
            if (m_curr == m_refs.Count)
            {
                m_curr = m_refs.Count + 1;
                pWrote = 0;
                return HRESULTS.S_FALSE;
            }
            else if (m_curr > m_refs.Count)
            {
                pWrote = 0;
                return HRESULTS.E_FAIL;
            }

            int i;
            for (i = 0; m_curr < m_refs.Count && i < count; ++i, ++m_curr)
                refs[i] = m_refs[m_curr];

            pWrote = i;
            if (i != count)
            {
                m_curr = m_refs.Count + 1;
                return HRESULTS.S_FALSE;
            }

            return HRESULTS.S_OK;
        }
    }

    class MDSegmentEnum : IMDSegmentEnum
    {
        IList<ClrSegment> m_segments;
        int m_curr;

        public MDSegmentEnum(IList<ClrSegment> segments)
        {
            m_segments = segments;
        }

        public void GetCount(out int pCount)
        {
            pCount = m_segments.Count;
        }

        public int Next(out IMDSegment ppSegment)
        {
            if (m_curr < m_segments.Count)
            {
                ppSegment = new MDSegment(m_segments[m_curr++]);
                return HRESULTS.S_OK;
            }
            else if (m_curr == m_segments.Count)
            {
                m_curr++;
                ppSegment = null;
                return HRESULTS.S_FALSE;
            }

            ppSegment = null;
            return HRESULTS.E_FAIL;
        }

        public void Reset()
        {
            m_curr = 0;
        }
    }

    class MDRootEnum : IMDRootEnum
    {
        IList<ClrRoot> m_roots;
        int m_curr = 0;

        public MDRootEnum(IList<ClrRoot> roots)
        {
            m_roots = roots;
        }

        public void GetCount(out int pCount)
        {
            pCount = m_curr;
        }

        public int Next(out IMDRoot ppRoot)
        {
            if (m_curr < m_roots.Count)
            {
                ppRoot = new MDRoot(m_roots[m_curr]);
                return HRESULTS.S_OK;
            }
            else if (m_curr == m_roots.Count)
            {
                ppRoot = null;
                return HRESULTS.S_FALSE;
            }

            ppRoot = null;
            return HRESULTS.E_FAIL;
        }

        public void Reset()
        {
            m_curr = 0;
        }
    }

    class MDStackTraceEnum : IMDStackTraceEnum
    {
        IList<ClrStackFrame> m_frames;
        int m_curr = 0;

        public MDStackTraceEnum(IList<ClrStackFrame> frames)
        {
            m_frames = frames;
        }

        public void GetCount(out int pCount)
        {
            pCount = m_frames.Count;
        }

        public int Next(out ulong pIP, out ulong pSP, out string pFunction)
        {
            if (m_curr < m_frames.Count)
            {
                ClrStackFrame frame = m_frames[m_curr];
                pIP = frame.InstructionPointer;
                pSP = frame.StackPointer;
                pFunction = frame.ToString();
                return HRESULTS.S_OK;
            }

            m_curr++;
            pIP = 0;
            pSP = 0;
            pFunction = null;
            return (m_curr == m_frames.Count) ? HRESULTS.S_FALSE : HRESULTS.E_FAIL;
        }

        public void Reset()
        {
            m_curr = 0;
        }
    }

    class MDHandleEnum : IMDHandleEnum
    {
        IList<ClrHandle> m_handles;
        int m_curr;

        public MDHandleEnum(IEnumerable<ClrHandle> handles)
        {
            m_handles = new List<ClrHandle>(handles);
        }

        public void GetCount(out int pCount)
        {
            throw new NotImplementedException();
        }

        public int Next(out IMDHandle ppHandle)
        {
            if (m_curr < m_handles.Count)
            {
                ppHandle = new MDHandle(m_handles[m_curr++]);
                return HRESULTS.S_OK;
            }
            else if (m_curr == m_handles.Count)
            {
                m_curr++;
                ppHandle = null;
                return HRESULTS.S_FALSE;
            }

            ppHandle = null;
            return HRESULTS.E_FAIL;
        }

        public void Reset()
        {
            m_curr = 0;
        }
    }


    class MDMemoryRegionEnum : IMDMemoryRegionEnum
    {
        IList<ClrMemoryRegion> m_regions;
        int m_curr;

        public MDMemoryRegionEnum(IList<ClrMemoryRegion> regions)
        {
            m_regions = regions;
        }

        public void GetCount(out int pCount)
        {
            pCount = m_regions.Count;
        }

        public int Next(out IMDMemoryRegion ppRegion)
        {
            if (m_curr < m_regions.Count)
            {
                ppRegion = new MDMemoryRegion(m_regions[m_curr++]);
                return HRESULTS.S_OK; ;
            }
            else if (m_curr == m_regions.Count)
            {
                m_curr++;
                ppRegion = null;
                return HRESULTS.S_FALSE;
            }

            ppRegion = null;
            return HRESULTS.E_FAIL;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }


    class MDThreadEnum : IMDThreadEnum
    {
        IList<ClrThread> m_threads;
        int m_curr = 0;

        public MDThreadEnum(IList<ClrThread> threads)
        {
            m_threads = threads;
        }

        public void GetCount(out int pCount)
        {
            pCount = m_threads.Count;
        }

        public int Next(out IMDThread ppThread)
        {
            if (m_curr < m_threads.Count)
            {
                ppThread = new MDThread(m_threads[m_curr++]);
                return HRESULTS.S_OK;
            }
            else if (m_curr == m_threads.Count)
            {
                m_curr++;
                ppThread = null;
                return HRESULTS.S_FALSE;
            }

            ppThread = null;
            return HRESULTS.E_FAIL;
        }

        public void Reset()
        {
            m_curr = 0;
        }
    }
}
