# Microsoft.Diagnostics.Runtime "CLR MD"

CLR MD is a C# API used to build diagnostics tools. It gives you the power and
flexibility of what the SOS and PSSCOR debugger extensions can do in a simple,
fast C# API.

Some features include:

1. Memory Diagnostics
1. Walking the GC Heap.
2. Walking roots in the process.
3. Walking all heaps that CLR owns, such as JIT code heaps, AppDomain heaps, etc.
2. Walk threads in the process to get managed callstacks.
3. Walk AppDomains in the process.
4. Walk COM wrappers in your process (v4.5+ only).
5. And more...

## FAQ

Please see the [FAQ](./docs/FAQ.md) for more information.

## Tutorials

Here you will find a step by step walkthrough on how to use the CLR MD API.
These tutorials are meant to be read and worked through in linear order to teach
you the surface area of the API and what you can do with it.

1. [Getting Started](./docs/GettingStarted.md) - A brief introduction to the API
   and how to create a CLRRuntime instance.

2. [The CLRRuntime Object](./docs/ClrRuntime.md) - Basic operations like
   enumerating AppDomains, Threads, the Finalizer Queue, etc.

3. [Walking the Heap](./docs/WalkingTheHeap.md) - Walking objects on the GC heap,
   working with types in CLR MD.

4. [Types and Fields in CLRMD](./docs/TypesAndFields.md) - More information about
   dealing with types and fields in CLRMD.
