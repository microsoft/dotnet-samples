using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

using MdDumper.Visualization;

namespace MdDumper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || new[] {"/?", "-?", "-h", "--help"}.Any(x => string.Equals(args[0], x, StringComparison.OrdinalIgnoreCase)))
            {
                PrintUsage();
                return;
            }

            foreach (var fileName in args)
            {
                Console.WriteLine(fileName);
                Console.WriteLine(new string('*', 80));

                try
                {
                    using (var stream = File.OpenRead(fileName))
                    using (var peFile = new PEReader(stream))
                    {
                        var metadataReader = peFile.GetMetadataReader();
                        var visualizer = new MetadataVisualizer(metadataReader, Console.Out);
                        visualizer.Visualize();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);                    
                }
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("This tool dumps the contents of all tables in a set of PE files.");
            Console.WriteLine("usage: mddumper <file>...");
        }
    }
}
