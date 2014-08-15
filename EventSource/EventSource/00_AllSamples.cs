using System;
using System.IO;
using System.Diagnostics;


namespace EventSourceSamples
{
    public class AllSamples
    {
        /// <summary>
        /// The samples are 'console based' in that they spew text to an output stream.   By default this is
        /// the Console, but you can redirect it elsewhere by overriding this static variable.  
        /// </summary>
        public static TextWriter Out = Console.Out;

        /// <summary>
        /// This is the main entry point for all the samples.   It runs them sequentially
        /// </summary>
        public static void Run()
        {
            // The model for EventSources is that any listeners that subscribe to an EventSource get the
            // messages.   This could be ETW, or the Windows EventLog or other EventListeners.   To demo
            // EventListeners, we  create an EventListener that also sends the logging messages to the console
            // and have ALL the demos send data there (as well as perhaps other places). 
            //
            // Note that eventListener do NOT die if they are unreferenced (because EventSources themselves
            // refer to them).   Thus you must explicitly Dispose an EventListener (e.g. a using statement) 
            // if you wish it to die.  
            using (var eventListener = new ConsoleEventListener())
            {
                MinimalEventSourceDemo.Run(); Debugger.Break();         // Break between demos, Hit F5 to continue. 
                CustomizedEventSourceDemo.Run(); Debugger.Break();
                EventLogEventSourceDemo.Run(); Debugger.Break();
                LocalizedEventSourceDemo.Run(); Debugger.Break();

                // We don't run LocalizedEventLogEventSourceDemo by default it will not produce
                // different results than the non-localized version unless you set your 
                // computer's culture to French.  If you are willing to do this (or change 
                // the demo to support a language you do care about), then you can enable it. 
                // LocalizedEventLogEventSourceDemo.Run(); Debugger.Break();
            }
        }
    }
}
