using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace PerMonitorDemo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void showLayoutButton_Click(object sender, EventArgs e)
        {
            (new SimpleLayout()).Show();
        }

        // Install targeting pack from the appropriate build
        // \\vsufile\patches\sign\NETFX\4.7\S112.2\02032.00\MTPack\NDP463-TargetingPack-KB3183844.exe

        private void MainForm_Load(object sender, EventArgs e)
        {
            NameValueCollection settingsCollection = null;
            try
            {
                settingsCollection = ConfigurationManager.GetSection("System.Windows.Forms.ApplicationConfigurationSection") as NameValueCollection;
            }
            catch
            {
            }

            if (settingsCollection != null)
            {
                dpiAwarenessLabel.Text = $"Application DPI Awareness is {settingsCollection.Get("DpiAwareness")}.";
            }
            formSizeLabel.Text = $"Form size is {Size.Width.ToString()} x {Size.Height.ToString()}";
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            formSizeLabel.Text = $"Form size is {Size.Width.ToString()} x {Size.Height.ToString()}";
        }

        private void formSizeTextBox_DpiChangedAfterParent(object sender, EventArgs e)
        {
            formSizeLabel.Text = $"Form size is {Size.Width.ToString()} x {Size.Height.ToString()}";
        }

        private void customDrawing_Click(object sender, EventArgs e)
        {
            (new CustomDrawing()).Show();
        }

        private void buttonImages_Click(object sender, EventArgs e)
        {
            (new ScaleButtonImages()).Show();
        }
    }
}
