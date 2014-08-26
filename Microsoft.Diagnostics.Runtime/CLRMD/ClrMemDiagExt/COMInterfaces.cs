// DO NOT MODIFY!  This is generated code.
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Microsoft.Diagnostics.Runtime;

namespace Microsoft.Diagnostics.RuntimeExt
{
    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.GuidAttribute("41CBFB96-45E4-4F2C-9002-82FD2ECD585F")]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDTarget
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetRuntimeCount([Out] out int pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetRuntimeInfo(int num, [Out] [MarshalAs((UnmanagedType)28)] out IMDRuntimeInfo ppInfo);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetPointerSize([Out] out int pPointerSize);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CreateRuntimeFromDac([MarshalAs((UnmanagedType)19)] string dacLocation, [Out] [MarshalAs((UnmanagedType)28)] out IMDRuntime ppRuntime);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CreateRuntimeFromIXCLR([MarshalAs((UnmanagedType)25)] Object ixCLRProcess, [MarshalAs((UnmanagedType)28)] out IMDRuntime ppRuntime);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("4AADFA25-0486-48EB-9338-D4B39E23AF82")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDRuntimeInfo
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetRuntimeVersion([Out] [MarshalAs((UnmanagedType)19)] out string pVersion);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetDacLocation([Out] [MarshalAs((UnmanagedType)19)] out string pVersion);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetDacRequestData([Out] out int pTimestamp, [Out] out int pFilesize);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetDacRequestFilename([Out] [MarshalAs((UnmanagedType)19)] out string pRequestFileName);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.GuidAttribute("2900B785-981D-4A6F-82DC-AE7B9DA08EA2")]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDRuntime
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void IsServerGC([Out] out int pServerGC);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetHeapCount([Out] out int pHeapCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        [PreserveSig]
        int ReadVirtual(ulong addr, [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] byte[] buffer, int requested, [Out] out int pRead);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        [PreserveSig]
        int ReadPtr(ulong addr, [Out] out ulong pValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Flush();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetHeap([Out] [MarshalAs((UnmanagedType)28)] out IMDHeap ppHeap);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumerateAppDomains([Out] [MarshalAs((UnmanagedType)28)] out IMDAppDomainEnum ppEnum);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumerateThreads([Out] [MarshalAs((UnmanagedType)28)] out IMDThreadEnum ppEnum);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumerateFinalizerQueue([Out] [MarshalAs((UnmanagedType)28)] out IMDObjectEnum ppEnum);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumerateGCHandles([Out] [MarshalAs((UnmanagedType)28)] out IMDHandleEnum ppEnum);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumerateMemoryRegions([Out] [MarshalAs((UnmanagedType)28)] out IMDMemoryRegionEnum ppEnum);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("B53137DF-FC18-4470-A0D9-8EE4F829C970")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDHeap
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetObjectType(ulong addr, [Out] [MarshalAs((UnmanagedType)28)] out IMDType ppType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetExceptionObject(ulong addr, [Out] [MarshalAs((UnmanagedType)28)] out IMDException ppExcep);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumerateRoots([Out] [MarshalAs((UnmanagedType)28)] out IMDRootEnum ppEnum);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumerateSegments([Out] [MarshalAs((UnmanagedType)28)] out IMDSegmentEnum ppEnum);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("BCA27E5A-5226-4CB6-AE95-239560E4FD71")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDAppDomainEnum
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCount([Out] out int pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Reset();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        [PreserveSig]
        int Next([Out] [MarshalAs((UnmanagedType)28)] out IMDAppDomain ppAppDomain);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("CA6A4D0A-7770-4254-A072-DA72A6CC1A72")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDThreadEnum
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCount([Out] out int pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Reset();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        [PreserveSig]
        int Next([Out] [MarshalAs((UnmanagedType)28)] out IMDThread ppThread);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.GuidAttribute("61119B5D-53DE-443C-911B-EB0607555451")]
    public interface IMDObjectEnum
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCount([Out] out int pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Reset();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        [PreserveSig]
        int Next(int count, [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] ulong[] objs, [Out] out int pWrote);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("32742CA6-56E7-438D-8FE8-1D30BE2F1F86")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDHandleEnum
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCount([Out] out int pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Reset();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        [PreserveSig]
        int Next([Out] [MarshalAs((UnmanagedType)28)] out IMDHandle ppHandle);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("9B68F32F-0DDD-452B-88F1-972496F44210")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDMemoryRegionEnum
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCount([Out] out int pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Reset();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        [PreserveSig]
        int Next([Out] [MarshalAs((UnmanagedType)28)] out IMDMemoryRegion ppRegion);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("FF5B59F4-07A0-4D7C-8B59-69338EECEA16")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDType
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetName([Out] [MarshalAs((UnmanagedType)19)] out string pName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetSize(ulong objRef, [Out] out ulong pSize);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void ContainsPointers([Out] out int pContainsPointers);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCorElementType([Out] out int pCET);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetBaseType([Out] [MarshalAs((UnmanagedType)28)] out IMDType ppBaseType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetArrayComponentType([Out] [MarshalAs((UnmanagedType)28)] out IMDType ppArrayComponentType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCCW(ulong addr, [Out] [MarshalAs((UnmanagedType)28)] out IMDCCW ppCCW);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetRCW(ulong addr, [Out] [MarshalAs((UnmanagedType)28)] out IMDRCW ppRCW);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void IsArray([Out] out int pIsArray);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void IsFree([Out] out int pIsFree);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void IsException([Out] out int pIsException);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void IsEnum([Out] out int pIsEnum);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetEnumElementType([Out] out int pValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetEnumNames([Out] [MarshalAs((UnmanagedType)28)] out IMDStringEnum ppEnum);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetEnumValueInt32([MarshalAs((UnmanagedType)19)] string name, [Out] out int pValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetFieldCount([Out] out int pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetField(int index, [Out] [MarshalAs((UnmanagedType)28)] out IMDField ppField);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        [PreserveSig]
        int GetFieldData(ulong obj, int interior, int count, [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] MD_FieldData[] fields, [Out] out int pNeeded);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetStaticFieldCount([Out] out int pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetStaticField(int index, [Out] [MarshalAs((UnmanagedType)28)] out IMDStaticField ppStaticField);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetThreadStaticFieldCount([Out] out int pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetThreadStaticField(int index, [Out] [MarshalAs((UnmanagedType)28)] out IMDThreadStaticField ppThreadStaticField);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetArrayLength(ulong objRef, [Out] out int pLength);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetArrayElementAddress(ulong objRef, int index, [Out] out ulong pAddr);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetArrayElementValue(ulong objRef, int index, [Out] [MarshalAs((UnmanagedType)28)] out IMDValue ppValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumerateReferences(ulong objRef, [Out] [MarshalAs((UnmanagedType)28)] out IMDReferenceEnum ppEnum);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumerateInterfaces([Out] [MarshalAs((UnmanagedType)28)] out IMDInterfaceEnum ppEnum);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.GuidAttribute("E3F30A85-0DBB-44C3-AA3E-460CCF31A2F1")]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDException
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetGCHeapType([Out] [MarshalAs((UnmanagedType)28)] out IMDType ppType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetMessage([Out] [MarshalAs((UnmanagedType)19)] out string pMessage);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetObjectAddress([Out] out ulong pAddress);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetInnerException([Out] [MarshalAs((UnmanagedType)28)] out IMDException ppException);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetHRESULT([Out] [MarshalAs((UnmanagedType)45)] out int pHResult);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumerateStackFrames([Out] [MarshalAs((UnmanagedType)28)] out IMDStackTraceEnum ppEnum);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("90783F46-39C8-480A-BD7C-0D89FA97D5AD")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDRootEnum
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCount([Out] out int pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Reset();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        [PreserveSig]
        int Next([Out] [MarshalAs((UnmanagedType)28)] out IMDRoot ppRoot);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.GuidAttribute("18851D2F-A705-492C-9A80-202F39300E80")]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDSegmentEnum
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCount([Out] out int pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Reset();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        [PreserveSig]
        int Next([Out] [MarshalAs((UnmanagedType)28)] out IMDSegment ppSegment);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.GuidAttribute("5A73920A-F8C5-4FCB-A725-8564F41BB055")]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDCCW
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetIUnknown([Out] out ulong pIUnk);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetObject([Out] out ulong pObject);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetHandle([Out] out ulong pHandle);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetRefCount([Out] out int pRefCnt);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumerateInterfaces([Out] [MarshalAs((UnmanagedType)28)] out IMDCOMInterfaceEnum ppEnum);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("192249CE-6A17-4307-BA70-7AA871682128")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDRCW
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetIUnknown([Out] out ulong pIUnk);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetObject([Out] out ulong pObject);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetRefCount([Out] out int pRefCnt);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetVTable([Out] out ulong pHandle);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void IsDisconnected([Out] out int pDisconnected);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumerateInterfaces([Out] [MarshalAs((UnmanagedType)28)] out IMDCOMInterfaceEnum ppEnum);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.GuidAttribute("B53B96FD-F8E3-4B86-AA5A-ECA1B6352840")]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDStringEnum
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCount([Out] out int pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Reset();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        [PreserveSig]
        int Next([Out] [MarshalAs((UnmanagedType)19)] out string pValue);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("823556FD-FFC5-4139-8D84-D9B72E835D2F")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDField
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetName([Out] [MarshalAs((UnmanagedType)19)] out string pName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetType([Out] [MarshalAs((UnmanagedType)28)] out IMDType ppType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetElementType([Out] out int pCET);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetSize([Out] out int pSize);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetOffset([Out] out int pOffset);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetFieldValue(ulong objRef, int interior, [Out] [MarshalAs((UnmanagedType)28)] out IMDValue ppValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetFieldAddress(ulong objRef, int interior, [Out] out ulong pAddress);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Unicode)]
    public struct MD_FieldData
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)]
        public string name;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)]
        public string type;
        public int corElementType;
        public int offset;
        public int size;
        public ulong value;
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("0E65FD09-D7A6-4B45-BEAC-76A002F52525")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDStaticField
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetName([Out] [MarshalAs((UnmanagedType)19)] out string pName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetType([Out] [MarshalAs((UnmanagedType)28)] out IMDType ppType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetElementType([Out] out int pCET);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetSize([Out] out int pSize);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetFieldValue([MarshalAs((UnmanagedType)28)] IMDAppDomain appDomain, [Out] [MarshalAs((UnmanagedType)28)] out IMDValue ppValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetFieldAddress([MarshalAs((UnmanagedType)28)] IMDAppDomain appDomain, [Out] out ulong pAddress);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("809B99EF-7E7C-4351-BE76-9DCA2624D53E")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDThreadStaticField
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetName([Out] [MarshalAs((UnmanagedType)19)] out string pName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetType([Out] [MarshalAs((UnmanagedType)28)] out IMDType ppType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetElementType([Out] out int pCET);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetSize([Out] out int pSize);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetFieldValue([MarshalAs((UnmanagedType)28)] IMDAppDomain appDomain, [MarshalAs((UnmanagedType)28)] IMDThread thread, [Out] [MarshalAs((UnmanagedType)28)] out IMDValue ppValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetFieldAddress([MarshalAs((UnmanagedType)28)] IMDAppDomain appDomain, [MarshalAs((UnmanagedType)28)] IMDThread thread, [Out] out ulong pAddress);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("B72A6495-EE8A-4491-A01D-295B36A730F2")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDValue
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void IsNull(out int pNull);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetElementType(out int pCET);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetInt32(out int pValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetUInt32(out UInt32 pValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetInt64(out Int64 pValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetUInt64(out ulong pValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetString([MarshalAs((UnmanagedType)19)] out string pValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetBool(out int pBool);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("35DE7FD4-902C-4CC5-9C87-A7C9632B4B4A")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDReferenceEnum
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCount([Out] out int pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Reset();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        [PreserveSig]
        int Next(int count, [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] MD_Reference[] refs, [Out] out int pWrote);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("723BC010-963F-11E1-A8B0-0800200C9A66")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDInterfaceEnum
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCount([Out] out int pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Reset();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        [PreserveSig]
        int Next([Out] [MarshalAs((UnmanagedType)28)] out IMDInterface ppValue);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("E630B609-502A-43F7-8F88-1598F21F2848")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDCOMInterfaceEnum
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCount([Out] out int pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Reset();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        [PreserveSig]
        int Next([MarshalAs((UnmanagedType)28)] IMDType pType, out ulong pInterfacePtr);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.GuidAttribute("6558598B-772F-4686-8F14-72C565072171")]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDAppDomain
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetName([Out] [MarshalAs((UnmanagedType)19)] out string pName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetID([Out] out int pID);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetAddress([Out] out ulong pAddress);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("A64706CC-C45D-481B-A4B4-4E3D04D27F91")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDThread
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetAddress([Out] out ulong pAddress);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void IsFinalizer([Out] out int IsFinalizer);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void IsAlive([Out] out int IsAlive);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetOSThreadId([Out] out int pOSThreadId);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetAppDomainAddress([Out] out ulong pAppDomain);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetLockCount([Out] out int pLockCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCurrentException([Out] [MarshalAs((UnmanagedType)28)] out IMDException ppException);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetTebAddress([Out] out ulong pTeb);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetStackLimits([Out] out ulong pBase, [Out] out ulong pLimit);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumerateStackTrace([Out] [MarshalAs((UnmanagedType)28)] out IMDStackTraceEnum ppEnum);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("04DF4D19-7BFB-4535-BE6C-4115AB5F313D")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDStackTraceEnum
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCount([Out] out int pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Reset();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        [PreserveSig]
        int Next([Out] out ulong pIP, [Out] out ulong pSP, [Out] [MarshalAs((UnmanagedType)19)] out string pFunction);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Unicode)]
    public struct MD_Reference
    {
        public ulong address;
        public int offset;
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("31980D20-963F-11E1-A8B0-0800200C9A66")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDInterface
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetName([Out] [MarshalAs((UnmanagedType)19)] out string pName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetBaseInterface([Out] [MarshalAs((UnmanagedType)28)] out IMDInterface ppBase);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.GuidAttribute("CF7DD882-7CF9-4F13-91C3-3E102CEDA943")]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDRoot
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetRootInfo([Out] out ulong pAddress, [Out] out ulong pObjRef, [Out] out MDRootType pType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetType([Out] [MarshalAs((UnmanagedType)28)] out IMDType ppType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetName([Out] [MarshalAs((UnmanagedType)19)] out string ppName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetAppDomain([Out] [MarshalAs((UnmanagedType)28)] out IMDAppDomain ppDomain);
    }

    public enum MDRootType
    {
        MDRoot_StaticVar,
        MDRoot_ThreadStaticVar,
        MDRoot_LocalVar,
        MDRoot_Strong,
        MDRoot_Weak,
        MDRoot_Pinning,
        MDRoot_Finalizer,
        MDRoot_AsyncPinning,
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("62CE62BA-066A-4FBD-B150-B4E5EAA080EE")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDSegment
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetStart([Out] out ulong pAddress);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetEnd([Out] out ulong pAddress);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetReserveLimit([Out] out ulong pAddress);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCommitLimit([Out] out ulong pAddress);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetLength([Out] out ulong pLength);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetProcessorAffinity([Out] out int pProcessor);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void IsLarge([Out] out int pLarge);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void IsEphemeral([Out] out int pEphemeral);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetGen0Info([Out] out ulong pStart, [Out] out ulong pLen);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetGen1Info([Out] out ulong pStart, [Out] out ulong pLen);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetGen2Info([Out] out ulong pStart, [Out] out ulong pLen);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumerateObjects([Out] [MarshalAs((UnmanagedType)28)] out IMDObjectEnum ppEnum);
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.GuidAttribute("FDB00A49-0584-4C24-95A4-B5CB08917596")]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDHandle
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetHandleData([Out] out ulong pAddr, [Out] out ulong pObjRef, [Out] out MDHandleTypes pType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void IsStrong([Out] out int pStrong);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetRefCount([Out] out int pRefCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetDependentTarget([Out] out ulong pTarget);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetAppDomain([Out] [MarshalAs((UnmanagedType)28)] out IMDAppDomain ppDomain);
    }

    public enum MDHandleTypes
    {
        MDHandle_WeakShort,
        MDHandle_WeakLong,
        MDHandle_Strong,
        MDHandle_Pinned,
        MDHandle_Variable,
        MDHandle_RefCount,
        MDHandle_Dependent,
        MDHandle_AsyncPinned,
        MDHandle_SizedRef,
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.GuidAttribute("8E5310DF-0F3A-4456-ACC4-52DEE8754767")]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDMemoryRegion
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetRegionInfo([Out] out ulong pAddress, [Out] out ulong pSize, [Out] out MDMemoryRegionType pType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetAppDomain([Out] [MarshalAs((UnmanagedType)28)] out IMDAppDomain ppDomain);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetModule([Out] [MarshalAs((UnmanagedType)19)] out string pModule);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetHeapNumber([Out] out int pHeap);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetDisplayString([Out] [MarshalAs((UnmanagedType)19)] out string pName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetSegmentType([Out] out MDSegmentType pType);
    }

    public enum MDMemoryRegionType
    {
        MDRegion_LowFrequencyLoaderHeap,
        MDRegion_HighFrequencyLoaderHeap,
        MDRegion_StubHeap,
        MDRegion_IndcellHeap,
        MDRegion_LookupHeap,
        MDRegion_ResolveHeap,
        MDRegion_DispatchHeap,
        MDRegion_CacheEntryHeap,
        MDRegion_JITHostCodeHeap,
        MDRegion_JITLoaderCodeHeap,
        MDRegion_ModuleThunkHeap,
        MDRegion_ModuleLookupTableHeap,
        MDRegion_GCSegment,
        MDRegion_ReservedGCSegment,
        MDRegion_HandleTableChunk,
    }

    public enum MDSegmentType
    {
        MDSegment_Ephemeral,
        MDSegment_Regular,
        MDSegment_LargeObject,
    }

    [System.Runtime.InteropServices.ComImportAttribute()]
    [System.Runtime.InteropServices.GuidAttribute("5DC19835-504C-47AF-B96B-06AF1A737AE9")]
    [System.Runtime.InteropServices.TypeLibTypeAttribute((System.Runtime.InteropServices.TypeLibTypeFlags)128)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute((System.Runtime.InteropServices.ComInterfaceType)1)]
    public interface IMDActivator
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CreateFromCrashDump([MarshalAs((UnmanagedType)19)] string crashdump, [Out] [MarshalAs((UnmanagedType)28)] out IMDTarget ppTarget);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CreateFromIDebugClient([MarshalAs((UnmanagedType)25)] Object iDebugClient, [Out] [MarshalAs((UnmanagedType)28)] out IMDTarget ppTarget);
    }

}
