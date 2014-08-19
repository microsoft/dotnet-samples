using System;
using Microsoft.Diagnostics.Tracing;

namespace EventSourceSamples
{
    [EventSource(Name="Company-ProductName-ComponentName")]
    public sealed class CustomizedXferEventSource : EventSource
    {
        [Event(1, Task=Tasks.Request, Opcode=EventOpcode.Send)]
        public void RequestStart(Guid relatedActivityId, int reqId, string url)
        {
            WriteEventWithRelatedActivityId(1, relatedActivityId, reqId, url);
        }

        #region Keywords / Task / Opcodes
        public static class Tasks
        {
            public const EventTask Request = (EventTask)0x1;
        }
        #endregion

        #region Singleton instance
        public static CustomizedXferEventSource Log = new CustomizedXferEventSource();
        #endregion
    }

#if false
    [EventSource(Name = "Company-ProductName-ComponentName")]
    public sealed class CustomizedForPerfXferEventSource : EventSource
    {
        [Event(1, Task = Tasks.Request, Opcode = EventOpcode.Send)]
        public void RequestStart(Guid relatedActivityId, int reqId, string url)
        {
            if (IsEnabled())
                WriteEventWithRelatedActivityId(1, relatedActivityId, reqId, url);
        }

        [NonEvent]
        unsafe private void WriteEventWithRelatedActivityId(int eventId, Guid relatedActivityId, 
                        int arg1, string arg2)
        {
            if (IsEnabled())
            {
                if (arg2 == null) arg2 = string.Empty;
                fixed (char* stringBytes = arg2)
                {
                    EventData* descrs = stackalloc EventData[2];
                    descrs[0].DataPointer = (IntPtr)(&arg1);
                    descrs[0].Size = 4;
                    descrs[1].DataPointer = (IntPtr)stringBytes;
                    descrs[1].Size = ((arg2.Length + 1) * 2);
                    WriteEventWithRelatedActivityIdCore(eventId,
                    &relatedActivityId, 2, descrs);
                }
            }
        }

    #region Keywords / Task / Opcodes
        public static class Tasks
        {
            public const EventTask Request = (EventTask)0x1;
        }
        #endregion

    #region Singleton instance
        public static CustomizedForPerfXferEventSource Log = new CustomizedForPerfXferEventSource();
        #endregion
    }
#endif
}
