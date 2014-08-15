using System;
using System.IO;
using Microsoft.Diagnostics.Tracing;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

//
// Shows how to use EventSouces to send messages to the Windows EventLog
//
// * EventLogEventSource
//     * Uses the new 'Channel' attribute to indicate that certain events
//       should go to the Windows Event Log.   Note that the EventSource
//       has to be registered with the Windows OS for this to work.  
//       In this example we send messages to the 'Admin' channel.   
//
// * EventLogEventSourceDemo 
//     * simulates a deployment step needed to register the event source's 
//       manifest on the machine. If running unelevated it will prompt the user
//       to allow running wevtutil.exe, the tool that performs the registration.
//
//     * simulates processing multiple requests by calling the methods on 
//       EventLogEventSource to fire events.

//     * pauses to allow the user to examine the event logs created under 
//       'Application and Services Logs/Microsoft/EventSourceDemos/Channeled'
//
//     * undoes the registration steps performed earlier.
//
namespace EventSourceSamples
{
    [EventSource(Name = "Samples-EventSourceDemos-EventLog")]
    public sealed class EventLogEventSource : EventSource
    {
        #region Singleton instance
        static public EventLogEventSource Log = new EventLogEventSource();
        #endregion

        [Event(1, Keywords = Keywords.Requests, Message = "Start processing request\n\t*** {0} ***\nfor URL\n\t=== {1} ===",
            Channel = EventChannel.Admin, Task = Tasks.Request, Opcode = EventOpcode.Start)]
        public void RequestStart(int RequestID, string Url) { WriteEvent(1, RequestID, Url); }

        [Event(2, Keywords = Keywords.Requests, Message = "Entering Phase {1} for request {0}",
            Channel = EventChannel.Analytic,  Task = Tasks.Request, Opcode = EventOpcode.Info, Level = EventLevel.Verbose)]
        public void RequestPhase(int RequestID, string PhaseName) { WriteEvent(2, RequestID, PhaseName); }

        [Event(3, Keywords = Keywords.Requests, Message = "Stop processing request\n\t*** {0} ***",
            Channel = EventChannel.Admin, Task = Tasks.Request, Opcode = EventOpcode.Stop)]
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

    public class EventLogEventSourceDemo
    {
        static TextWriter Out = AllSamples.Out;
        static string DeploymentFolder { get; set; }

        /// <summary>
        /// This is a demo of using ChannelEventSource.  
        /// </summary>
        public static void Run()
        {
            Out.WriteLine("******************** EventLogEventSource Demo ********************");

            // Deploy the app
            DeploymentFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"EventSourceSamples");
            RegisterEventSourceWithOperatingSystem.SimulateInstall(DeploymentFolder, EventLogEventSource.Log.Name);

            GenerateEvents();            // Generate some events. 

            // Let the user inspect that results
            Out.WriteLine("Launching 'start eventvwr' to view the newly generated events.");
            Out.WriteLine("Look in 'Application and Services Logs/Samples/EventSourceDemos/EventLog'");
            Out.WriteLine("Close the event viewer when complete.");
            Process.Start(new ProcessStartInfo("eventvwr") { UseShellExecute = true }).WaitForExit();
            Out.WriteLine("Press <Enter> to continue.");
            Console.ReadLine();

            // Uninstall.  
            RegisterEventSourceWithOperatingSystem.SimulateUninstall(DeploymentFolder);
            Out.WriteLine();
        }

        public static void GenerateEvents()
        {
            var requests = new string[] {
                "/home/index.aspx",
                "/home/catalog/index.aspx",
                "/home/catalog/100",
                "/home/catalog/121",
                "/home/catalog/144",
            };

            Out.WriteLine("Writing some events to the EventSource.");
            int id = 0;
            foreach (var req in requests)
                DoRequest(req, ++id);
            Out.WriteLine("Done writing events.");
        }
        private static void DoRequest(string request, int requestId)
        {
            EventLogEventSource.Log.RequestStart(requestId, request);

            foreach (var phase in new string[] { "initialize", "query_db", "query_webservice", "process_results", "send_results" })
            {
                EventLogEventSource.Log.RequestPhase(requestId, phase);
                // simulate error on request for "/home/catalog/121"
                if (request == "/home/catalog/121" && phase == "query_db")
                {
                    EventLogEventSource.Log.DebugTrace("Error on page: " + request);
                    break;
                }
            }

            EventLogEventSource.Log.RequestStop(requestId);
        }
    }

    /// <summary>
    /// For the Windows EventLog to listen for EventSources, they must be
    /// registered with the operating system.  This is a deployment step 
    /// (typically done by a installer).   For demo purposes, however we 
    /// have written code run by the demo itself that accomplishes this 
    /// </summary>
    static class RegisterEventSourceWithOperatingSystem
    {
        static TextWriter Out = AllSamples.Out;

        /// <summary>
        /// Simulate an installation to 'destFolder' for the named eventSource.  If you don't
        /// specify eventSourceName all eventSources information next to the EXE is registered.
        /// </summary>
        public static void SimulateInstall(string destFolder, string eventSourceName= "", bool prompt = true)
        {
            Out.WriteLine("Simulating the steps needed to register the EventSource with the OS");
            Out.WriteLine("These steps are only needed for Windows Event Log support.");
            Out.WriteLine("Admin privileges are needed to do this, so you will see elevation prompts");
            Out.WriteLine("If you are not already elevated.  Consider running from an admin window.");
            Out.WriteLine();

            if (prompt)
            {
                Out.WriteLine("Press <Enter> to proceed with installation");
                Console.ReadLine();
            }

            Out.WriteLine("Deploying EventSource to {0}", destFolder);
            // create deployment folder if needed
            if (Directory.Exists(destFolder))
            {
                Out.WriteLine("Error: detected a previous deployment.   Cleaning it up.");
                SimulateUninstall(destFolder, false);
                Out.WriteLine("Done Cleaning up orphaned installation.");
            }

            Out.WriteLine("Copying the EventSource manifest and compiled Manifest DLL to target directory.");
            Directory.CreateDirectory(destFolder);
            var sourceFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (var filename in Directory.GetFiles(sourceFolder, "*" + eventSourceName + "*.etwManifest.???"))
            {
                var destPath =  Path.Combine(destFolder, Path.GetFileName(filename));
                Out.WriteLine("xcopy \"{0}\" \"{1}\"", filename, destPath);
                File.Copy(filename, destPath, true);
            }

            Out.WriteLine("Registering the manifest with the OS (Need to be elevated)");
            foreach (var filename in Directory.GetFiles(destFolder, "*.etwManifest.man"))
            {
                var commandArgs = string.Format("im {0} /rf:\"{1}\" /mf:\"{1}\"",
                    filename,
                    Path.Combine(destFolder, Path.GetFileNameWithoutExtension(filename) + ".dll"));

                // as a precaution uninstall the manifest.   It is easy for the demos to not be cleaned up 
                // and the install will fail if the EventSource is already registered.   
                Process.Start(new ProcessStartInfo("wevtutil.exe", "um" + commandArgs.Substring(2)) { Verb = "runAs" }).WaitForExit();
                Thread.Sleep(200);          // just in case elevation makes the wait not work.  

                Out.WriteLine("  wevtutil " + commandArgs);
                // The 'RunAs' indicates it needs to be elevated. 
                // Unfortunately this also makes it problematic to get the output or error code.  
                Process.Start(new ProcessStartInfo("wevtutil.exe", commandArgs) { Verb = "runAs" }).WaitForExit();
            }

            System.Threading.Thread.Sleep(1000);
            Out.WriteLine("Done deploying app.");
            Out.WriteLine();
        }

        /// <summary>
        /// Reverses the Install step 
        /// </summary>
        public static void SimulateUninstall(string destFolder, bool prompt = true)
        {
            Out.WriteLine("Uninstalling the EventSoure demos from {0}", destFolder);
            Out.WriteLine("This also requires elevation.");
            Out.WriteLine("Please close the event viewer if you have not already done so!");

            if (prompt)
            {
                Out.WriteLine("Press <Enter> to proceed with uninstall.");
                Console.ReadLine();
            }

            // run wevtutil elevated to unregister the ETW manifests
            Out.WriteLine("Unregistering manifests");
            foreach (var filename in Directory.GetFiles(destFolder, "*.etwManifest.man"))
            {
                var commandArgs = string.Format("um {0}", filename);
                Out.WriteLine("    wevtutil " + commandArgs);
                // The 'RunAs' indicates it needs to be elevated.  
                var process = Process.Start(new ProcessStartInfo("wevtutil.exe", commandArgs) { Verb = "runAs" });
                process.WaitForExit();
            }

            Out.WriteLine("Removing {0}", destFolder);
            // If this fails, it means that something is using the directory.  Typically this is an eventViewer or 
            // a command prompt in that directory or visual studio.    If all else fails, rebooting should fix this.  
            if (Directory.Exists(destFolder))
                Directory.Delete(destFolder, true);
            Out.WriteLine("Done uninstalling app.");
        }
    }
}
