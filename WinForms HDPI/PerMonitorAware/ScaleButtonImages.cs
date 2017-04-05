using PerMonitorDemo.Properties;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace PerMonitorDemo
{
    public partial class ScaleButtonImages : Form
    {
        public ScaleButtonImages()
        {
            InitializeComponent();
            errorButton.OriginalIcon = Resources.Error;
            propertiesButton.OriginalIcon = Resources.Wrench;

        }
    }

    internal class ImageButton : Button
    {
        public ImageButton() : base()
        {
        }

        private Icon originalIcon;
        public Icon OriginalIcon 
        {
            private get 
            {
                return originalIcon;
            }

            set 
            {
                originalIcon = value;
                Image = GetScaledBitmapFromIcon(originalIcon);
            }
        }

        private Bitmap GetScaledBitmapFromIcon(Icon icon)
        {
            Icon scaledIcon = new Icon(icon, GetScaledSize());
            Bitmap bitmap = scaledIcon.ToBitmap();
            scaledIcon.Dispose();

            return bitmap;
        }

        private Size GetScaledSize()
        {
            int dimention = LogicalToDeviceUnits(32);
            return new Size(dimention, dimention);
        }

        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            base.OnDpiChangedAfterParent(e);
            Image = GetScaledBitmapFromIcon(originalIcon);
        }
    }
}
