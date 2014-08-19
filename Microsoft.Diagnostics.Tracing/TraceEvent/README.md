# TraceEvent Samples

To run the demos you simply launch `TraceEvent.sln` in Visual Studio and hit
<kbd>F5</kbd>.

## More Information

The samples are all under the `TraceEvent` project in your solution and
there is a file for each sample of the form `NN_<SampleName>.cs`. The samples
have detailed comments that tell what they do as well as `WriteLine` statements
that also indicate what is going on. 

So you can either simply run all the samples, or take a look at the comments
in each one to see which one is most appropriate for your needs. Each sample
has a `Run` method that is is main entry point, so it is easy to run just
one of the samples. For example

    TraceEventSamples.SimpleEventSourceMonitor.Run();

Will run just the `SimpleEventSourceMonitor` sample. 

By default the output goes to `Console.Out` but you can redirect it to another
`TextWriter` by setting `AllSamples.Out`. This is useful for GUI Apps. 

## User Guide

This sample also includes a [user guide](docs/TraceEvent.md) which is worth
reading.

A useful technique is to copy & paste some of the sample code to your own code.