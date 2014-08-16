using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Diagnostics;
using System.IO;

namespace TraceEventSamples
{
    /// <summary>
    /// AllSamples contains a harness for running a the TraceEvent samples. 
    /// </summary>
    partial class AllSamples
    {
        /// <summary>
        /// The samples are 'console based' in that the spew text to an output stream.   By default this is
        /// the Console, but you can redirect it elsewhere by overriding this static variable.  
        /// </summary>
        public static TextWriter Out = Console.Out;

        /// <summary>
        /// This is the main entry point for all the samples.   It runs them sequentially, modify to run the ones you are really interested in. 
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("If run in a debugger, the program will break after each demo.");
            SimpleEventSourceMonitor.Run(); Debugger.Break();       // Break point between demos, hit F5 to continue. 
            SimpleEventSourceFile.Run(); Debugger.Break();
            ObserveGCEvents.Run(); Debugger.Break();
            ObserveJitEvents.Run(); Debugger.Break();
            ObserveEventSource.Run(); Debugger.Break();
            ModuleLoadMonitor.Run(); Debugger.Break();
            KernelAndClrMonitor.Run(); Debugger.Break();
            KernelAndClrFile.Run(); Debugger.Break();
            KernelAndClrMonitorWin7.Run(); Debugger.Break();
            KernelAndClrFileWin7.Run(); Debugger.Break();
            SimpleTraceLog.Run(); Debugger.Break();
            TraceLogMonitor.Run(); Debugger.Break();
            SimpleFileRelogger.Run(); Debugger.Break();
            SimpleMonitorRelogger.Run(); Debugger.Break();
            Console.WriteLine("Done with samples");
        }
    }
}