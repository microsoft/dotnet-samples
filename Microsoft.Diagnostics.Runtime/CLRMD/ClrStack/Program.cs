// Please go to the ClrMD project page on github for full source and to report issues:
//    https://github.com/Microsoft/clrmd

using System;
using Microsoft.Diagnostics.Runtime;
using System.IO;

namespace ClrStack
{
    class Program
    {
        static void Main(string[] args)
        {
            string dump, dac;
            bool dso;
            if (!TryParseArgs(args, out dump, out dac, out dso))
            {
                Usage();
                return;
            }

            ClrRuntime runtime = CreateRuntime(dump, dac);

            // Walk each thread in the process.
            foreach (ClrThread thread in runtime.Threads)
            {
                // The ClrRuntime.Threads will also report threads which have recently died, but their
                // underlying datastructures have not yet been cleaned up.  This can potentially be
                // useful in debugging (!threads displays this information with XXX displayed for their
                // OS thread id).  You cannot walk the stack of these threads though, so we skip them
                // here.
                if (!thread.IsAlive)
                    continue;

                Console.WriteLine("Thread {0:X}:", thread.OSThreadId);
                Console.WriteLine("Stack: {0:X} - {1:X}", thread.StackBase, thread.StackLimit);

                // Each thread tracks a "last thrown exception".  This is the exception object which
                // !threads prints.  If that exception object is present, we will display some basic
                // exception data here.  Note that you can get the stack trace of the exception with
                // ClrHeapException.StackTrace (we don't do that here).
                ClrException exception = thread.CurrentException;
                if (exception != null)
                    Console.WriteLine("Exception: {0:X} ({1}), HRESULT={2:X}", exception.Address, exception.Type.Name, exception.HResult);

                // Walk the stack of the thread and print output similar to !ClrStack.
                Console.WriteLine();
                Console.WriteLine("Managed Callstack:");
                foreach (ClrStackFrame frame in thread.StackTrace)
                {
                    // Note that CLRStackFrame currently only has three pieces of data: stack pointer,
                    // instruction pointer, and frame name (which comes from ToString).  Future
                    // versions of this API will allow you to get the type/function/module of the
                    // method (instead of just the name).  This is not yet implemented.
                    Console.WriteLine("{0,16:X} {1,16:X} {2}", frame.StackPointer, frame.InstructionPointer, frame.DisplayString);
                }

                // Print a !DumpStackObjects equivalent.
                if (dso)
                {
                    // We'll need heap data to find objects on the stack.
                    ClrHeap heap = runtime.GetHeap();

                    // Walk each pointer aligned address on the stack.  Note that StackBase/StackLimit
                    // is exactly what they are in the TEB.  This means StackBase > StackLimit on AMD64.
                    ulong start = thread.StackBase;
                    ulong stop = thread.StackLimit;

                    // We'll walk these in pointer order.
                    if (start > stop)
                    {
                        ulong tmp = start;
                        start = stop;
                        stop = tmp;
                    }

                    Console.WriteLine();
                    Console.WriteLine("Stack objects:");

                    // Walk each pointer aligned address.  Ptr is a stack address.
                    for (ulong ptr = start; ptr <= stop; ptr += (ulong)runtime.PointerSize)
                    {
                        // Read the value of this pointer.  If we fail to read the memory, break.  The
                        // stack region should be in the crash dump.
                        ulong obj;
                        if (!runtime.ReadPointer(ptr, out obj))
                            break;

                        // 003DF2A4 
                        // We check to see if this address is a valid object by simply calling
                        // GetObjectType.  If that returns null, it's not an object.
                        ClrType type = heap.GetObjectType(obj);
                        if (type == null)
                            continue;

                        // Don't print out free objects as there tends to be a lot of them on
                        // the stack.
                        if (!type.IsFree)
                            Console.WriteLine("{0,16:X} {1,16:X} {2}", ptr, obj, type.Name);
                    }
                }

                Console.WriteLine();
                Console.WriteLine("----------------------------------");
                Console.WriteLine();
            }
        }

        public static bool TryParseArgs(string[] args, out string dump, out string dac, out bool dso)
        {
            dump = null;
            dac = null;
            dso = false;

            foreach (string arg in args)
            {
                if (arg == "-dso")
                {
                    dso = true;
                }
                else if (dump == null)
                {
                    dump = arg;
                }
                else if (dac == null)
                {
                    dac = arg;
                }
                else
                {
                    Console.WriteLine("Too many arguments.");
                    return false;
                }
            }

            return dump != null;
        }


        private static ClrRuntime CreateRuntime(string dump, string dac)
        {
            // Create the data target.  This tells us the versions of CLR loaded in the target process.
            DataTarget dataTarget = DataTarget.LoadCrashDump(dump);

            // Now check bitness of our program/target:
            bool isTarget64Bit = dataTarget.PointerSize == 8;
            if (Environment.Is64BitProcess != isTarget64Bit)
                throw new Exception(string.Format("Architecture mismatch:  Process is {0} but target is {1}", Environment.Is64BitProcess ? "64 bit" : "32 bit", isTarget64Bit ? "64 bit" : "32 bit"));

            // Note I just take the first version of CLR in the process.  You can loop over every loaded
            // CLR to handle the SxS case where both v2 and v4 are loaded in the process.
            ClrInfo version = dataTarget.ClrVersions[0];

            // Next, let's try to make sure we have the right Dac to load.  Note we are doing this manually for
            // illustration.  Simply calling version.CreateRuntime with no arguments does the same steps.
            if (dac != null && Directory.Exists(dac))
                dac = Path.Combine(dac, version.DacInfo.FileName);
            else if (dac == null || !File.Exists(dac))
                dac = dataTarget.SymbolLocator.FindBinary(version.DacInfo);

            // Finally, check to see if the dac exists.  If not, throw an exception.
            if (dac == null || !File.Exists(dac))
                throw new FileNotFoundException("Could not find the specified dac.", dac);

            // Now that we have the DataTarget, the version of CLR, and the right dac, we create and return a
            // ClrRuntime instance.
            return version.CreateRuntime(dac);
        }

        public static void Usage()
        {
            string fn = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Console.WriteLine("Usage: {0} [-dso] crash.dmp [dac_file_name]", fn);
        }
    }
}
