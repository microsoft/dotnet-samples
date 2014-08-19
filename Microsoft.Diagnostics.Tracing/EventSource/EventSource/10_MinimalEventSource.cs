using System;
using System.IO;
using Microsoft.Diagnostics.Tracing;


//
// This demonstrates a minimal event source class definition and its use.
//
// * MinimalEventSource specifies:
//     * an explicit name for the ETW provider it defines (in the EventSourceAttribute)
//     * the singleton instance exposed to the user code: MinimalEventSource.Log
//     * one non-decorated ETW event method: MinimalEventSource.ImageLoad()
//
// * MinimalEventSourceDemo 
//     * fires ETW events by calling MinimalEventSource.Log.Load()
//
namespace EventSourceSamples
{
    // Give your event sources a descriptive name using the EventSourceAttribute, otherwise the name of the class is used. 
    [EventSource(Name = "Samples-EventSourceDemos-Minimal")]
    public sealed class MinimalEventSource : EventSource
    {
        #region Singleton instance

        // define the singleton instance of the event source
        public static MinimalEventSource Log = new MinimalEventSource();

        #endregion

        #region Events and NonEvents

        /// <summary>
        /// Call this method to notify listeners of a Load event for image 'imageName'
        /// </summary>
        /// <param name="baseAddress">The base address where the image was loaded</param>
        /// <param name="imageName">The image name</param>
        public void Load(long baseAddress, string imageName)
        {
            // Notes:
            //   1. the event ID passed to WriteEvent (1) corresponds to the (implied) event ID
            //      assigned to this method. The event ID could have been explicitly declared
            //      using an EventAttribute for this method
            //   2. the arguments passed to Load are forwarded in the same order to the 
            //      WriteEvent overload called below.
            WriteEvent(1, baseAddress, imageName);
        }

        #endregion
    }

    public class MinimalEventSourceDemo
    {
        static TextWriter Out = AllSamples.Out;

        /// <summary>
        /// This is a simplest demo of eventSources.  Because AllSamples.Run created a Console
        /// listener, these messages go to the console.   You can also use an ETW controller like
        /// the 'PerfView tool' (bing PerfView for download) to turn on the events e.g. 
        /// 
        /// PerfView /onlyProviders=*Samples-EventSourceDemos-Minimal collect
        /// </summary>
        public static void Run()
        {
            Out.WriteLine("******************** MinimalEventSource Demo ********************");
            Out.WriteLine("Sending 3 'Load' events from the Samples-EventSourceDemos-Minimal source.");

            // Just send out three 'Load' events, with different arguments.  
            MinimalEventSource.Log.Load(0x40000, "MyFile0");
            MinimalEventSource.Log.Load(0x80000, "MyFile1");
            MinimalEventSource.Log.Load(0xc0000, "MyFile2");

            Out.WriteLine("Done.");
        }
    }
}
