using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RayTracer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// A value indicating whether or not to use multiple threads for the rendering procedure
        /// </summary>
        public bool MultiThreadedRendering { get; set; }
        /// <summary>
        /// A value indicating whether or not to render each line as it is computed, or to hold rendering until the entire image is generated
        /// </summary>
        public bool DrawLinesAsync { get; set; }

        private Scene scene;
        private float fov;
        private DateTime renderStartTime;
        private int linesRendered;
        private WriteableBitmap writeableBitmap;
        private double totalLines;

        /// <summary>
        /// The horizontal field of view of the current scene camera, in degrees
        /// </summary>
        public float FieldOfView
        {
            get { return fov; }
            set
            {
                fov = value;
                if (scene != null)
                {
                    scene.Camera.FieldOfView = fov;
                }
            }
        }
        /// <summary>
        /// The depth of reflections to render in the scene
        /// </summary>
        public int ReflectionDepth
        {
            get
            {
                if (scene != null)
                {
                    return scene.Camera.ReflectionDepth;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (scene != null)
                {
                    scene.Camera.ReflectionDepth = value;
                }
            }
        }

        public MainWindow()
        {
            scene = Scene.TwoPlanes;
            this.ReflectionDepth = 5;
            this.FieldOfView = 120;
            scene.Camera.FieldOfView = this.FieldOfView;
            this.MultiThreadedRendering = true;
            this.DrawLinesAsync = true;

            InitializeComponent();
            InitializeKeyListeners();
            InitializeSceneOptions();
            this.SizeToContent = System.Windows.SizeToContent.Manual;
            this.Width = 640;
            this.Height = 480;

            Task.Run(() =>
                {
                    Task.Delay(100).Wait();
                    this.InvokeOnApplicationDispatcher(() =>
                        {
                            DrawSceneLinesMultiThreaded();
                        });
                });
        }

        private void InitializeSceneOptions()
        {
            var allScenesOptions = typeof(Scene).GetProperties(BindingFlags.Static | BindingFlags.Public).Where(pi => pi.PropertyType == typeof(Scene));
            foreach (var option in allScenesOptions)
            {
                var menuItem = new MenuItem()
                {
                    Header = option.Name
                };
                menuItem.Click += (sender, e) => OnSceneButtonClicked((Scene)option.GetValue(null));

                this.ScenesMenuItem.Items.Add(menuItem);
            }
        }

        private void OnSceneButtonClicked(Scene scene)
        {
            this.scene = scene;
            this.scene.Camera.FieldOfView = this.FieldOfView;
            this.scene.Camera.ReflectionDepth = this.ReflectionDepth;
        }

        private void InitializeKeyListeners()
        {
            this.KeyDown += OnKeyPressed;
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                RefreshButtonPressed();
            }
            if (e.Key == Key.F11)
            {
                FullScreenButtonPressed();
            }
        }

        private void FullScreenButtonPressed()
        {
            if (this.WindowStyle == System.Windows.WindowStyle.None)
            {
                this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
                this.WindowState = System.Windows.WindowState.Normal;
            }
            else
            {
                this.WindowStyle = System.Windows.WindowStyle.None;
                this.WindowState = System.Windows.WindowState.Maximized;
            }
        }

        private void RefreshButtonPressed()
        {
            DrawScene();
        }

        public void DrawScene()
        {
            if (MultiThreadedRendering)
            {
                if (DrawLinesAsync)
                {
                    DrawSceneLinesMultiThreaded();
                }
                else
                {
                    DrawSceneMultiThreaded();
                }
            }
            else
            {
                if (DrawLinesAsync)
                {
                    DrawSceneLinesSingleThreaded();
                }
                else
                {
                    DrawSceneSingleThreaded();
                }
            }
        }

        // Specific rendering methods, based on which settings items are selected:

        private void DrawSceneLinesSingleThreaded()
        {
            writeableBitmap = GenerateDefaultWriteableBitmap();
            MainImage.Source = writeableBitmap;
            scene.Camera.RenderSceneLines(scene, (int)ImageGrid.RenderSize.Width, (int)ImageGrid.RenderSize.Height, OnLineRendered);
        }

        private async void DrawSceneSingleThreaded()
        {
            MainImage.Source = null;
            var timeBefore = DateTime.UtcNow;
            var bitmap = await scene.Camera.RenderSceneToBitmap(scene, (int)ImageGrid.RenderSize.Width, (int)ImageGrid.RenderSize.Height);
            bitmap.Save("TESTSAVE.png");
            var bitmapSource = BitmapToSource(bitmap);
            MainImage.Source = bitmapSource;
            var totalTime = (DateTime.UtcNow - timeBefore);
            SetRenderTimeText(totalTime.TotalMilliseconds);
        }

        private async void DrawSceneMultiThreaded()
        {
            MainImage.Source = null;
            var timeBefore = DateTime.UtcNow;
            var bitmap = await scene.Camera.RenderSceneToBitmapThreaded(scene, (int)ImageGrid.RenderSize.Width, (int)ImageGrid.RenderSize.Height);
            bitmap.Save("TESTSAVE.png");
            var bitmapSource = BitmapToSource(bitmap);
            MainImage.Source = bitmapSource;
            var totalTime = (DateTime.UtcNow - timeBefore);
            SetRenderTimeText(totalTime.TotalMilliseconds);
        }

        private void DrawSceneLinesMultiThreaded()
        {
            writeableBitmap = GenerateDefaultWriteableBitmap();
            MainImage.Source = writeableBitmap;
            linesRendered = 0;
            totalLines = writeableBitmap.Height;
            renderStartTime = DateTime.UtcNow;
            scene.Camera.RenderSceneLinesThreaded(scene, (int)ImageGrid.RenderSize.Width, (int)ImageGrid.RenderSize.Height, OnLineRendered);

        }

        private WriteableBitmap GenerateDefaultWriteableBitmap()
        {
            return new WriteableBitmap((int)ImageGrid.RenderSize.Width, (int)ImageGrid.RenderSize.Height, 96, 96, PixelFormats.Bgra32, null);
        }

        private void OnLineRendered(int rowNumber, RayTracer.Color[] lineColors)
        {
            var colors = ColorsToBRGA32(lineColors);
            InvokeOnApplicationDispatcher(() => WriteColorsToBitmap(rowNumber, colors));
            linesRendered++;
            if (linesRendered == totalLines)
            {
                var totalTime = (DateTime.UtcNow - renderStartTime).TotalMilliseconds;
                SetRenderTimeText(totalTime);
            }
        }

        private void InvokeOnApplicationDispatcher(Action action)
        {
            var application = Application.Current;
            if (application != null)
            {
                application.Dispatcher.Invoke(() =>
                    {
                        action();
                    });
            }
        }

        private void WriteColorsToBitmap(int rowNumber, int[] colors)
        {
            writeableBitmap.WritePixels(new Int32Rect(0, (int)writeableBitmap.Height - 1 - rowNumber, (int)writeableBitmap.Width, 1), colors, colors.Length * 4, 0);
        }

        private void SetRenderTimeText(double totalTime)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this.RenderTimeTextBlock.Text = totalTime.ToString() + " ms";
                }));
        }

        private int[] ColorsToBRGA32(RayTracer.Color[] lineColors)
        {
            var int32Colors = new Int32[lineColors.Length];
            for (int i = 0; i < lineColors.Length; i++)
            {
                int32Colors[i] = RayTracer.Color.ToBGRA32(lineColors[i]);
            }

            return int32Colors;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        private static BitmapSource BitmapToSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource retval;

            try
            {
                retval = Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return retval;
        }

        private void OnExitPressed(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void OnRenderPressed(object sender, RoutedEventArgs e)
        {
            DrawScene();
        }

        private void SaveButtonPressed(object sender, RoutedEventArgs e)
        {
            SaveCurrentImage();
        }

        private void SaveCurrentImage()
        {
            var dialog = new SaveFileDialog();
            dialog.DefaultExt = ".jpg";
            dialog.AddExtension = true;
            dialog.FileName = "RayTracedImage";
            dialog.Filter = "Images|*.png;*.bmp;*.jpg";
            var result = dialog.ShowDialog(this);
            if (result == true)
            {
                using (var file = dialog.OpenFile())
                {
                    var extension = System.IO.Path.GetExtension(dialog.FileName);
                    SaveImageToStream(file, extension);
                }
            }
        }

        private void SaveImageToStream(Stream fileStream, string format)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)MainImage.ActualWidth, (int)MainImage.ActualHeight, 96, 96, PixelFormats.Default);
            renderTargetBitmap.Render(MainImage);

            BitmapEncoder encoder;
            switch (format)
            {
                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;
                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;
                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;
                default:
                    System.Diagnostics.Debug.Write("invalid file extension: " + format + ". Using jpg");
                    encoder = new JpegBitmapEncoder();
                    break;
            }

            encoder.Frames = new[] { BitmapFrame.Create(renderTargetBitmap) };
            encoder.Save(fileStream);
        }
    }
}
