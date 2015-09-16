# Microsoft.Diagnostics.Runtime "CLR MD"

[CLR MD][CLRMD] is a C# API used to build diagnostics tools. It gives you the
power and flexibility of what the SOS and PSSCOR debugger extensions can do in
a simple, fast C# API.

Some features include:

1. Memory Diagnostics
1. Walking the GC Heap.
2. Walking roots in the process.
3. Walking all heaps that CLR owns, such as JIT code heaps, AppDomain heaps, etc.
2. Walk threads in the process to get managed callstacks.
3. Walk AppDomains in the process.
4. Walk COM wrappers in your process (v4.5+ only).
5. And more...

CLR MD itself is open source as well and can be found [here][CLRMD].

[CLRMD]: https://github.com/microsoft/clrmd
