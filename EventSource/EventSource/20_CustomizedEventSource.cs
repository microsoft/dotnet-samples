using System;
using System.IO;
using Microsoft.Diagnostics.Tracing;

//
// This demonstrates a typical event source definition with a some customization to enable
// easy event filtering from ETW controllers.
//
// * CustomizedEventSource
//   demonstrates the following new features over MinimalEventSource:
//     * defines custom Keywords and Tasks. Note that these have to be defined as public
//       nested classes within the EventSource, containing only const members of the 
//       appropriate type (EventKeywords and EventTask respectively)
//     * specifies known Opcodes Start and Stop on the RequestStart/RequestStop methods.
//       This allows ETW tools to calculate a "duration" for each request.
//
// * CustomizedEventSourceDemo 
//     * simulates processing multiple requests by calling the methods on CustomizedEventSource
//       to fire ETW events
//
namespace EventSourceSamples
{
    [EventSource(Name = "Samples-EventSourceDemos-Customized")]
    public sealed class CustomizedEventSource : EventSource
    {
        #region Singleton instance
        static public CustomizedEventSource Log = new CustomizedEventSource();
        #endregion

        // Note that each 'Event' attribute has a number that identifies the event.  This number is the same
        // as number passed to 'WriteEvent' inside the method.   This number must be unique, and should not be
        // bigger than it needs to be (EventSource makes an array of these numbers).   When event methods have
        // Event Attributes, they can go in any order in the file.  
        //
        // By making a 'Task' for the event, as well as using the 'start' or 'stop' opcode' you tell the system
        // that the start and stop are related and the delta between them is interesting and should be displayed
        // to the user.  If you use tasks and opcodes, you should name your method the task name concatenated
        // with the opcode name.   We do this for RequestStart and RequestStop.  
        [Event(1, Keywords = Keywords.Requests, Task = Tasks.Request, Opcode=EventOpcode.Start)]
        public void RequestStart(int RequestID, string Url) { WriteEvent(1, RequestID, Url); }

        // By giving tasks Keywords, you attach the event to one or groups that can be turned on and off
        // independently.   By giving it a level (default is Info), you indicate how common (verbose) the
        // event is and allow users to filter the more verbose ones easily.   
        [Event(2, Keywords = Keywords.Requests, Level = EventLevel.Verbose)]
        public void RequestPhase(int RequestID, string PhaseName) { WriteEvent(2, RequestID, PhaseName); }

        [Event(3, Keywords = Keywords.Requests, Task = Tasks.Request, Opcode=EventOpcode.Stop)]
        public void RequestStop(int RequestID) { WriteEvent(3, RequestID); }

        [Event(4, Keywords = Keywords.Debug)]
        public void DebugTrace(string Message) { WriteEvent(4, Message); }

        #region Keywords / Tasks / Opcodes

        /// <summary>
        /// By defining keywords, we can turn on events independently.   Because we defined the 'Request'
        /// and 'Debug' keywords and assigned the 'Request' keywords to the first three events, these 
        /// can be turned on and off by setting this bit when you enable the EventSource.   Similarly
        /// the 'Debug' event can be turned on and off independently.  
        /// </summary>
        public class Keywords   // This is a bitvector
        {
            public const EventKeywords Requests = (EventKeywords)0x0001;
            public const EventKeywords Debug = (EventKeywords)0x0002;
        }

        public class Tasks
        {
            public const EventTask Request = (EventTask)0x1;
        }

        #endregion
    }
    public class CustomizedEventSourceDemo
    {
        static TextWriter Out = AllSamples.Out;

        /// <summary>
        /// This is a demo of using CustomizedEventSource.  
        /// </summary>
        public static void Run()
        {
            Out.WriteLine("******************** CustomizedEventSource Demo ********************");
            Out.WriteLine("Sending a variety of messages, including 'Start', an 'Stop' Messages.");

            // Simulate some requests.  
            DoRequest("/home/index.aspx", 0);
            DoRequest("/home/catalog/100", 1);
            DoRequest("/home/catalog/121", 2);
            DoRequest("/home/catalog/144", 3);

            Out.WriteLine("Done generating events.");
            Out.WriteLine();
        }

        /// <summary>
        /// Simulates a server request.  Each request has a start and stop time and is given an ID to correlate it all./
        /// Requests have various events (phases) in the middle.   This is just a demo.  
        /// </summary>
        private static void DoRequest(string request, int requestId)
        {
            CustomizedEventSource.Log.RequestStart(requestId, request);

            foreach (var phase in new string[] { "initialize", "query_db", "query_webservice", "process_results", "send_results" })
            {
                CustomizedEventSource.Log.RequestPhase(requestId, phase);
                // simulate error on request for "/home/catalog/121"
                if (request == "/home/catalog/121" && phase == "query_db")
                {
                    CustomizedEventSource.Log.DebugTrace("Error on page: " + request);
                    break;
                }
            }

            CustomizedEventSource.Log.RequestStop(requestId);
        }
    }
}
