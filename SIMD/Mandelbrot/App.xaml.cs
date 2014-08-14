using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Numerics;
using System.Diagnostics;
using System.Threading;

namespace Mandelbrot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Goofy magic to get SIMD working with the CTP
        // This will not be required once RyuJIT is official
        // You must use the type in the class constructor
        // It will not be accelerated in this function, though...
        static Vector<float> dummy;
        static App() { dummy = Vector<float>.One; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!VectorMath.IsHardwareAccelerated)
            {
                MessageBox.Show("SIMD isn't enabled for the current process. Please make sure that" + Environment.NewLine + Environment.NewLine +
                                "(1) You've run the 'enable-jit.cmd' script prior to running this app" + Environment.NewLine + 
                                "(2) You disable the setting that suppresses JIT optimizations when debugging", "Error");
                Environment.Exit(1);
            }
        }
    }
}
