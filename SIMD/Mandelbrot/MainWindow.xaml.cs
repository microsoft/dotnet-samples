using System;
using System.Windows.Interop;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Windows.Threading;
using Algorithms;

namespace Mandelbrot
{
    public static class ext
    {
        public static double Clamp(this double val, double lo, double hi)
        {
            return Math.Min(Math.Max(val, lo), hi);
        }
        public static float Clamp(this float val, float lo, float hi)
        {
            return Math.Min(Math.Max(val, lo), hi);
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        private double RenderXC, RenderYC, RenderRange;

        #region Properties

        public static bool IsRyuJITLoaded()
        {
            return Process.GetCurrentProcess()
                .Modules.Cast<ProcessModule>()
                .Where(pm =>
                    pm.ModuleName.ToLowerInvariant().IndexOf("protojit") >= 0).Any();
        }
        public bool IsRyuJIT { get { return IsRyuJITLoaded(); } set { } }
        private double _XC;
        public double XC { get { return _XC; } set { lock (valRWLock) SetProperty(ref _XC, value); } }

        private double _YC;
        public double YC { get { return _YC; } set { lock (valRWLock) SetProperty(ref _YC, value); } }

        private double _Range;
        public double Range { get { return _Range; } set { lock (valRWLock) SetProperty(ref _Range, value); } }

        private double _ElapsedTime;
        public double ElapsedTime { get { return _ElapsedTime; } set { SetProperty(ref _ElapsedTime, value); } }

        private bool _UseSIMD;
        public bool UseSIMD { get { return _UseSIMD; } set { SetProperty(ref _UseSIMD, value); } }

        private bool _UseFloat;
        public bool UseFloat { get { return _UseFloat; } set { SetProperty(ref _UseFloat, value); } }

        private bool _UseADT;
        public bool UseADT { get { return _UseADT; } set { SetProperty(ref _UseADT, value); } }

        private bool _UseThreads;
        public bool UseThreads { get { return _UseThreads; } set { SetProperty(ref _UseThreads, value); } }

        private bool _Unsupported;
        public bool Unsupported { get { return _Unsupported; } set { SetProperty(ref _Unsupported, value); } }

        #endregion

        private object renderLock = new object();
        private object valRWLock = new object();
        private DispatcherTimer renderClock;
        public MainWindow()
        {
            _UseADT = false;
            _UseFloat = true;
            _UseSIMD = false;
            _UseThreads = false;

            // This is a good spot to show consistent rendering progress
            XC = -1.248;
            YC = -.0362;
            Range = .001;
            InitializeComponent();
            renderClock = new DispatcherTimer();
            renderClock.Tick += renderClock_Tick;
            renderClock.Interval = new TimeSpan(0, 0, 0, 0, 100); // Render at 10 FPS (instead of 2.5)
            DpiDraw();
        }
        int width;
        int height;
        byte[] bytes;
        void renderClock_Tick(object sender, EventArgs e)
        {
            try
            {
                bool stopClock = done;
                if (!abort)
                    theImage.Source = WriteableBitmap.Create(width, height, 96, 96, PixelFormats.Bgr32, null, bytes, width * 4);
                if (stopClock || abort)
                    renderClock.Stop();
            }
            catch (OutOfMemoryException)
            {
                GC.Collect();
            }
        }

        private void ResetValues()
        {
            XC = -0.5;
            YC = 0.0;
            Range = 3.0;
        }
        private void AddPixel(int x, int y, int iters)
        {
            if (y >= height || x >= width)
                return;
            int pos = 4 * (y * width + x);
            int val = 1000 - Math.Min(iters, 1000);
            byte blue = (byte)(val % 43 * 23);
            byte red = (byte)(val % 97 * 41);
            byte green = (byte)(val % 71 * 19);
            bytes[pos++] = red;
            bytes[pos++] = green;
            bytes[pos++] = blue;
            bytes[pos] = 0;
        }

        public static volatile bool abort = false;
        private static bool CheckAbort() { return abort; }
        private volatile bool done = true;

#pragma warning disable 4014
        private void DrawMandelbrot(int cw, int ch)
        {
            double range, xc, yc;
            lock (valRWLock)
            {
                RenderRange = range = Range;
                RenderXC = xc = XC;
                RenderYC = yc = YC;
                width = cw;
                height = ch;
                bytes = new byte[width * height * 4];
            }
            lock (renderLock)
            {
                var render = FractalRenderer.SelectRender(AddPixel, CheckAbort, UseSIMD, !UseFloat, UseThreads, UseADT);
                Unsupported = render == null;
                if (render == null)
                    return;

                abort = false;
                done = false;
                Dispatcher.InvokeAsync(renderClock.Start);
                double xmin = (xc - range / 2.0).Clamp(-3.0, 1);
                double xmax = (xc + range / 2.0).Clamp(-3.0, 1);
                if (xmin > xmax)
                {
                    double t = xmin;
                    xmin = xmax;
                    xmax = t;
                }
                double ymin = (yc - range / 2.0).Clamp(-1.5f, 1.5f);
                double ymax = (yc + range / 2.0).Clamp(-1.5f, 1.5f);
                if (ymin > ymax)
                {
                    double t = ymin;
                    ymin = ymax;
                    ymax = t;
                }
                double ystep = (range / (double)ch).Clamp(0, ymax - ymin);
                double xstep = (range / (double)cw).Clamp(0, xmax - xmin);
                double step = Math.Max(ystep, xstep);
                xmin = xc - (cw * step / 2);
                xmax = xc + (cw * step / 2);
                ymin = yc - (ch * step / 2);
                ymax = yc + (ch * step / 2);

                if (xmin == xmax || ymin == ymax ||
                    xmin + xstep <= xmin || ymin + ystep <= ymin ||
                    ymax - ystep >= ymax || xmax - xstep >= xmax)
                    return;

                Stopwatch timer = new Stopwatch();
                timer.Start();
                render((float)xmin, (float)xmax, (float)ymin, (float)ymax, (float)step);
                ElapsedTime = timer.ElapsedMilliseconds;
                abort = false;
                done = true;
            }
        }

#pragma warning restore
        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition((IInputElement)sender);
            double r = RenderRange / 2.0;
            double ratioX = canvas.ActualWidth / canvas.ActualHeight;
            double ratioY = 1.0;
            if (ratioX < 1.0)
            {
                ratioY = canvas.ActualWidth / canvas.ActualHeight;
                ratioX = 1.0;
            }
            XC = ext.Clamp(RenderXC - ratioX * RenderRange / 2.0 + ratioX * RenderRange * p.X / canvas.ActualWidth, -3.0, 1.0);
            YC = ext.Clamp(RenderYC - ratioY * RenderRange / 2.0 + ratioY * RenderRange * p.Y / canvas.ActualHeight, -2.0, 2.0);
        }
        private void ResetDrawing()
        {
            abort = true;
            lock (renderLock) { }
            DpiDraw();
        }

        private void DpiDraw()
        {
            double cw = (double)canvas.ActualWidth;
            double ch = (double)canvas.ActualHeight;
            if (cw < 1 || ch < 1)
                return;
            var ps = PresentationSource.FromVisual(this);
            if (ps == null)
                return;
            var ct = ps.CompositionTarget;
            if (ct == null)
                return;
            var mtrx = ct.TransformToDevice;
            double dpiX = (double)mtrx.M11;
            double dpiY = (double)mtrx.M22;
            Task.Run(() => DrawMandelbrot((int)(cw * dpiX), (int)(ch * dpiY)));
        }
        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResetDrawing();
        }
        private void Render_Click(object sender, RoutedEventArgs e)
        {
            ResetDrawing();
        }
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetValues();
            ResetDrawing();
        }
    }
}
