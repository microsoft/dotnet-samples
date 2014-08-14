using Algorithms;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading;

namespace Mandelbrot
{
    /// <summary>
    /// Interaction logic for FlyThru.xaml: the demo for the //build 2014 conference
    /// </summary>
    public partial class FlyThru : Window, INotifyPropertyChanged
    {
        // Various read-only properties displayed in the UI
        public double XC { get; private set; }

        public double YC { get; private set; }

        public double Scale { get; private set; }

        public TimeSpan ElapsedTime { get; private set; }

        public Visibility IsRyuJIT { get; private set; }

        // These are read/write
        public bool UseSIMD { get; set; }

        public bool UseThreads { get; set; }

        // The length of this array determines how many frames get rendered for the dmoe
        private Tuple<float, float, float>[] RenderPoints = new Tuple<float, float, float>[150];
        private DispatcherTimer renderClock;

        public FlyThru()
        {
            // Start point/range
            float xs = -0.5f;
            float ys = 0.0f;
            float rs = 3.0f;

            // End point/range
            float xe = -.2649f;
            float ye = -.8506f;
            float re = 0.00048828125f;

            // Interpolate all the points in between
            float l = 1.0f / (RenderPoints.Length - 1);
            for (int i = 0; i < RenderPoints.Length; i++)
            {
                float scale = (float)Math.Pow(l * i, 0.03125);
                RenderPoints[i] = Tuple.Create(xs + (xe - xs) * scale, ys + (ye - ys) * scale, rs + (re - rs) * scale);
            }

            // Initialize the visible elements, just for fun
            XC = xs;
            YC = ys;
            Scale = rs;
            IsRyuJIT = MainWindow.IsRyuJITLoaded() ? Visibility.Visible : Visibility.Collapsed;

            // WPF initialization
            InitializeComponent();

            // Create the timer to update the screen
            renderClock = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 16) };// 62.5 FPS should be sufficient
            renderClock.Tick += renderClock_Tick;
        }

        // The user clicked the "Begin" button
        private void Begin_Click(object sender, RoutedEventArgs e)
        {
            double cw = (double)canvas.ActualWidth;
            double ch = (double)canvas.ActualHeight;
            // Cowardly refuse to render a tiny region...
            if (cw < 10 || ch < 10)
                return;

            // This stuff is to make the bitmap screen-resolution specific
            // It looks really amazing (though renders much more slowly) on
            // 3200x1800 resolution laptop screens :-)

            #region Resolution Independence

            var ps = PresentationSource.FromVisual(this);
            if (ps == null)
                return;
            var ct = ps.CompositionTarget;
            if (ct == null)
                return;
            var mtrx = ct.TransformToDevice;
            double dpiX = mtrx.M11;
            double dpiY = mtrx.M22;

            #endregion Resolution Independence

            // Disable the button until the rendering is complete
            BeginButton.IsEnabled = false;

            // Start the rendering on a background task thread
            Task.Run(() =>
            {
                DrawMandelbrot((int)(cw * dpiX), (int)(ch * dpiY));
                // Re-enable the begin button once the drawing is completed
                // Has to be run on the UI thread...
                Dispatcher.Invoke(() => { BeginButton.IsEnabled = true; });
            });
        }

        private static bool CheckAbort() { return false; } // Holdover from the MainWindow xaml (needed for the rendering API)

        // This is all stuff for the rendering itself
        private int width, height;
        private byte[] bytes = null;
        private byte[] toImage = null;
        private WriteableBitmap theBitmap = null;

        void renderClock_Tick(object sender, EventArgs e)
        {
            byte[] img = Interlocked.Exchange(ref toImage, null);
            // If we have a render buffer, update the UI accordingly
            if (img != null)
            {
                // Update the on-screen data elements
                Notify();
                // Allocate new bitmap if we need to,
                // or if the window has been resized since last "begin" button click
                if (theBitmap == null || theBitmap.Width != width || theBitmap.Height != height)
                {
                    theBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
                    theImage.Source = theBitmap;
                }
                // Copy data from the render buffer to the on-screen bitmap
                theBitmap.WritePixels(new Int32Rect(0, 0, width, height), img, width * 4, 0, 0);
            }
        }

        private void AddPixel(int x, int y, int iters)
        {
            if (y >= height || x >= width)
                return;
            int pos = 4 * (y * width + x);
            int val = 1000 - Math.Min(iters, 1000);
            // This here is how I pick a color.
            // It was highly scientific...
            byte blue = (byte)(val % 43 * 23);
            byte red = (byte)(val % 97 * 41);
            byte green = (byte)(val % 71 * 19);
            bytes[pos++] = red;
            bytes[pos++] = green;
            bytes[pos++] = blue;
            bytes[pos] = 0;
        }

        private void DrawMandelbrot(int cw, int ch)
        {
            // Get the renderer the user selected
            var render = FractalRenderer.SelectRender(AddPixel, CheckAbort, UseSIMD, false, UseThreads, false); // always float, always raw
            // Create a stopwatch to clock the calculation speed
            Stopwatch timer = new Stopwatch();
            // Allocate a pair of render buffers that will be swapped per frame
            byte[] buffer1 = new byte[cw * ch * 4];
            byte[] buffer2 = new byte[cw * ch * 4];
            // This is the buffer selector
            bool which = false;
            // Set the two render properties
            width = cw;
            height = ch;
            // Make sure the toImage buffer is null (no animation starting yet)
            toImage = null;

            // Start the XX FPS clock tick
            Dispatcher.InvokeAsync(renderClock.Start);
            // Start the timer
            timer.Start();
            foreach (var pt in RenderPoints)
            {
                // Select the buffer
                bytes = which ? buffer1 : buffer2;

                // Get the frame location & scale
                float scale = pt.Item3;
                float xc = pt.Item1;
                float yc = pt.Item2;
                XC = xc;
                YC = yc;
                Scale = scale;

                // Get the min/max/step values and make sure they're all sensible
                float xmin = (xc - scale / 2.0f).Clamp(-3.0f, 1f);
                float xmax = (xc + scale / 2.0f).Clamp(-3.0f, 1f);
                if (xmin > xmax)
                {
                    float t = xmin;
                    xmin = xmax;
                    xmax = t;
                }
                float ymax = (yc + scale / 2.0f).Clamp(-1.5f, 1.5f);
                float ymin = (yc - scale / 2.0f).Clamp(-1.5f, 1.5f);
                if (ymin > ymax)
                {
                    float t = ymin;
                    ymin = ymax;
                    ymax = t;
                }
                float ystep = (scale / (float)ch).Clamp(0, ymax - ymin);
                float xstep = (scale / (float)cw).Clamp(0, xmax - xmin);
                float step = Math.Max(ystep, xstep);
                xmin = xc - (cw * step / 2);
                xmax = xc + (cw * step / 2);
                ymin = yc - (ch * step / 2);
                ymax = yc + (ch * step / 2);
                // Render this frame
                render(xmin, xmax, ymin, ymax, step);
                // Frame's complete: publish the current buffer for the
                // render thread to draw
                bytes = Interlocked.Exchange(ref toImage, bytes);
                if (bytes == null)
                {
                    // The render thread finished with the previous frame, swap it and keep going
                    which = !which;
                }
                else
                {
                    // We've finished a frame before the rendering thread had a change to
                    // render the previous frame: leave the buffer selection alone, so the
                    // frame we just calculated gets skipped.
                }
                // Update the published clock
                ElapsedTime = timer.Elapsed;
            }
            // Stop the timer
            timer.Stop();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Notify all the UI elements as having been updated
        protected void Notify()
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs("XC"));
                eventHandler(this, new PropertyChangedEventArgs("YC"));
                eventHandler(this, new PropertyChangedEventArgs("Scale"));
                eventHandler(this, new PropertyChangedEventArgs("ElapsedTime"));
            }
        }

        #endregion INotifyPropertyChanged Members
    }
}