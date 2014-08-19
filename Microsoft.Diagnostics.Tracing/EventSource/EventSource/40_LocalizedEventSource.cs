using System;
using System.IO;
using Microsoft.Diagnostics.Tracing;
using System.Globalization;
using System.Threading;

//
// illustrates how to localize an event source.
//
// * LocalizedEventSource
//   demonstrates the following new features over CustomizedEventSource:
//     * specifies the LocalizationResources property on the EventSourceAttribute
//       used to customize LocalizedEventSource.   The property indicates that
//       all events should look also int the specified resource file to see if
//       they have localized event messages, task names, opcode names or keyword
//       names. 
//
//   * In this example we only localize to the FR-fr (french) culture, but the 
//       principle is the same.  
//
//       NOTE: The LocalizationResources property needs to be updated to refer to
//             the actual Default Namespace (aka the RootNamespace) of the current
//             project.
//
// * LocalizedEventSourceDemo 
//     * to illustrate the behavior under different cultures create separate app 
//       domains for three cultures. Each app domain will have its own instance 
//       of a LocalizedEventSource. This instance's strings are set at the time
//       the first listener (whether an ETW listener or an EventListener) 
//       expresses interest in the event source, by enabling the event provider 
//       (in the case of an ETW listener), or by calling EventListener.EnableEvents 
//       (in the case of an EventListener). The current UI culture at this time
//       defines the localization of the messages included in the ETW manifest as
//       well as the Message property to be included in the EventWrittenEventArgs.
//
//     * in each app domain it simulates processing multiple requests by calling 
//       the methods on LocalizedEventSource to fire events.
//
namespace EventSourceSamples
{
    [EventSource(Name = "Samples-EventSourceDemos-Localized",
                LocalizationResources = "EventSourceSamples.LesResource")]
    public sealed class LocalizedEventSource : EventSource
    {
        #region Singleton instance
        static public LocalizedEventSource Log = new LocalizedEventSource();
        #endregion

        // Note that because of LocalizedEventSource this method actually has a 'Message' associated
        // with it.   Open the 'resx' LesResource.resx file to see the message.  
        [Event(1, Keywords = Keywords.Requests, Task = Tasks.Request, Opcode = EventOpcode.Start)]
        public void RequestStart(int RequestID, string Url) { WriteEvent(1, RequestID, Url); }

        [Event(2, Keywords = Keywords.Requests, Task = Tasks.Request, Opcode = EventOpcode.Info, Level = EventLevel.Verbose)]
        public void RequestPhase(int RequestID, string PhaseName) { WriteEvent(2, RequestID, PhaseName); }

        // Note that because of LocalizedEventSource this method actually has a 'Message' associated
        // with it.   Open the 'resx' LesResource.resx file to see the message.  
        [Event(3, Keywords = Keywords.Requests, Task = Tasks.Request, Opcode = EventOpcode.Stop)]
        public void RequestStop(int RequestID) { WriteEvent(3, RequestID); }

        [Event(4, Keywords = Keywords.Debug, Message = "DebugMessage: {0}")]
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

    public class LocalizedEventSourceDemo : MarshalByRefObject
    {
        static TextWriter Out = AllSamples.Out;

        /// <summary>
        /// This is a demo of using LocalizedEventSourceDemo.  
        /// </summary>
        public static void Run()
        {
            Out.WriteLine("******************** LocalizedEventSource Demo ********************");
            Out.WriteLine("This app that messages can be localized to particular cultures");
            Out.WriteLine("Normally a process has only a single culture that is shared machine wide");
            Out.WriteLine("However in this demo we create an AppDomain and give it a different culture");
            Out.WriteLine("The messages that are logged will attempt to use the AppDomain's culture.");
            Out.WriteLine("Here we use English, French and Romanian.   However we don't resource");
            Out.WriteLine("data for Romanian so it falls back to English.");
            Out.WriteLine("");

            foreach (var cultureName in new string[] { "en-US", "fr-FR", "ro-RO" })
            {
                // use the app domain name as a cheap way of passing an argument:
                // the culture name to use on the thread.
                var workerDomainFriendlyName = "workerDom_" + cultureName;

                // create the new domain
                var workerDom = AppDomain.CreateDomain(workerDomainFriendlyName);

                // run the request processing code
                workerDom.DoCallBack(RunWorker);
            }

            Out.WriteLine();
        }

        private static void RunWorker()
        {
            // set current culture based on appDomain's friendly name
            var adCultureName = AppDomain.CurrentDomain.FriendlyName.Substring("workerDom_".Length);
            var culture = new CultureInfo(adCultureName);
            var savedUICulture = Thread.CurrentThread.CurrentUICulture;
            var savedCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            Out.WriteLine("**** Test in UI culture = {0}", Thread.CurrentThread.CurrentUICulture);
            Out.WriteLine("Writing Events");
            // the new worker app domains need an event listener to display the events
            using (var el = new ConsoleEventListener())
            {
                int id = 0;
                foreach (var req in new string[] { "/home/index.aspx", "/home/catalog/121" })
                    DoRequest(req, ++id);
            }
            Out.WriteLine("Done Writing Events");

            // restore original culture
            Thread.CurrentThread.CurrentUICulture = savedUICulture;
            Thread.CurrentThread.CurrentCulture = savedCulture;
        }

        private static void DoRequest(string request, int requestId)
        {
            LocalizedEventSource.Log.RequestStart(requestId, request);

            foreach (var phase in new string[] { "initialize", "query_db", "query_webservice", "process_results", "send_results" })
            {
                LocalizedEventSource.Log.RequestPhase(requestId, phase);
                // simulate error on request for "/home/catalog/121"
                if (request == "/home/catalog/121" && phase == "query_db")
                {
                    LocalizedEventSource.Log.DebugTrace("Error on page: " + request);
                    break;
                }
            }

            LocalizedEventSource.Log.RequestStop(requestId);
        }
    }
}
