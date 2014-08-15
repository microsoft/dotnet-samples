using System;
using System.IO;
using Microsoft.Diagnostics.Tracing;
using System.Diagnostics;

//
// This builds on the previous sample by illustrating how to localize an event source
// and send this localized information to the Windows EventLog.  
//
// * LocalizedEventLogEventSource
//     * specifies the LocalizationResources property on the EventSourceAttribute
//       used to customize LocalizedEventSource.   The property indicates that
//       all events should look also int the specified resource file to see if
//       they have localized event messages, task names, opcode names or keyword
//       names. 
//
//   * In this example we only localize to the FR-fr (French) culture, but the 
//       principle is the same. 
//
//       NOTE: The LocalizationResources property needs to be updated to refer to
//             the actual Default Namespace (aka the RootNamespace) of the current
//             project.
//
// * LocalizedEventLogEventSourceDemo 
//     * simulates a deployment step needed to register the event source's 
//       manifest on the machine. If running unelevated it will prompt the user
//       to allow running wevtutil.exe, the tool that performs the registration.
//
//     * simulates processing multiple requests by calling the methods on 
//       LocalizedEventLogEventSource to fire events.

//     * pauses to allow the user to examine the event logs created under 
//       'Application and Services Logs/Microsoft/EventSourceDemos/Channeled'
//
//     * undoes the registration steps performed earlier.
//  
namespace EventSourceSamples
{
    [EventSource(Name = "Samples-EventSourceDemos-LocalizedEventLog",
                 LocalizationResources = "EventSourceSamples.LesResource")]
    public sealed class LocalizedEventLogEventSource : EventSource
    {
        #region Singleton instance
        static public LocalizedEventLogEventSource Log = new LocalizedEventLogEventSource();
        #endregion

        // Note that because of LocalizedEventSource this method actually has a 'Message' associated
        // with it.   Open the 'resx' LesResource.resx file to see the message.  
        [Event(1, Keywords = Keywords.Requests, Task = Tasks.Request, Opcode = EventOpcode.Start, Channel = EventChannel.Admin)]
        public void RequestStart(int RequestID, string Url) { WriteEvent(1, RequestID, Url); }

        [Event(2, Keywords = Keywords.Requests, Task = Tasks.Request, Opcode = EventOpcode.Info, Level = EventLevel.Verbose)]
        public void RequestPhase(int RequestID, string PhaseName) { WriteEvent(2, RequestID, PhaseName); }

        // Note that because of LocalizedEventSource this method actually has a 'Message' associated
        // with it.   Open the 'resx' LesResource.resx file to see the message.  
        [Event(3, Keywords = Keywords.Requests, Task = Tasks.Request, Opcode = EventOpcode.Stop, Channel = EventChannel.Admin)]
        public void RequestStop(int RequestID) { WriteEvent(3, RequestID); }

        [Event(4, Keywords = Keywords.Debug, Message = "DebugMessage: {0}", Channel = EventChannel.Debug)]
        public void DebugTrace(string Message) { WriteEvent(4, Message); }

        #region Keywords / Tasks / Opcodes

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

    public class LocalizedEventLogEventSourceDemo
    {
        static TextWriter Out = AllSamples.Out;
        static string DeploymentFolder { get; set; }

        /// <summary>
        /// This is a demo of using ChannelEventSource.  
        /// </summary>
        public static void Run()
        {
            Out.WriteLine("******************** LocalizedEventLogEventSourceDemo Demo ********************");
            Out.WriteLine("This demonstrate that the events sent to the EventLog will be localized to");
            Out.WriteLine("The culture of the machine.   Currently however the demo only supports English");
            Out.WriteLine("and French.   Thus to see something interesting you must set your machine's"); 
            Out.WriteLine("culture to French.");

            // We need to simulate deployment of the app, in this case we go to C:\ProgramData\EventSourceSamples
            DeploymentFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"EventSourceSamples");
            RegisterEventSourceWithOperatingSystem.SimulateInstall(DeploymentFolder, LocalizedEventLogEventSource.Log.Name);

            RunWorker();        // Generate some events.  

            Out.WriteLine("Launching 'start eventvwr' to view the newly generated events.");
            Out.WriteLine("Look in 'Application and Services Logs/Samples/EventSourceDemos/EventLog'");
            Out.WriteLine("To see the localized messages, you need to set your culture to Fr-fr.");
            Out.WriteLine("Close the event viewer when complete.");
            Process.Start(new ProcessStartInfo("eventvwr") { UseShellExecute = true }).WaitForExit();
            Out.WriteLine("Press <Enter> to continue.");
            Console.ReadLine();

            RegisterEventSourceWithOperatingSystem.SimulateUninstall(DeploymentFolder);
            Out.WriteLine();
        }

        public static void RunWorker()
        {
            var requests = new string[] {
                "/home/index.aspx",
                "/home/catalog/index.aspx",
                "/home/catalog/100",
                "/home/catalog/121",
                "/home/catalog/144",
            };

            int id = 0;
            foreach (var req in requests)
                DoRequest(req, ++id);
        }
        
        private static void DoRequest(string request, int requestId)
        {
            LocalizedEventLogEventSource.Log.RequestStart(requestId, request);

            foreach (var phase in new string[] { "initialize", "query_db", "query_webservice", "process_results", "send_results" })
            {
                LocalizedEventLogEventSource.Log.RequestPhase(requestId, phase);
                // simulate error on request for "/home/catalog/121"
                if (request == "/home/catalog/121" && phase == "query_db")
                {
                    LocalizedEventLogEventSource.Log.DebugTrace("Error on page: " + request);
                    break;
                }
            }

            LocalizedEventLogEventSource.Log.RequestStop(requestId);
        }
    }

}
