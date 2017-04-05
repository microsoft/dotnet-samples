using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace HighDpiDemo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void showLayoutButton_Click(object sender, EventArgs e)
        {
            MultipleScalingPasses frm = new MultipleScalingPasses();
            frm.Show();
        }

        private void showControlsButton_Click(object sender, EventArgs e)
        {
            MenuStripAndCheckedListBox frm = new MenuStripAndCheckedListBox();
            frm.Show();
        }

        private void showExceptionDialogButton_Click(object sender, EventArgs e)
        {
            (new ThreadExceptionDialog(new Exception("Really long exception description string, because we want to see if it properly wraps around or is truncated."))).Show();
        }


        private void calendarButton_Click(object sender, EventArgs e)
        {
            (new Calendar()).Show();
        }

        private void anchoredControls_Click(object sender, EventArgs e)
        {
            (new RightAnchor()).Show();
        }

        private void dgvHeaders_Click(object sender, EventArgs e)
        {
            (new DataGridViewHeadersScaling()).Show();
        }

        // Install targeting pack from the appropriate build
        // \\vsufile\patches\sign\NETFX\4.7\S112.2\02032.00\MTPack\NDP463-TargetingPack-KB3183844.exe
        private void MainForm_DpiChanged(Object sender, DpiChangedEventArgs e)
        {
            currentDpiLabel.Text = $"Current scaling = {(int)Math.Round((DeviceDpi / 96.0) * 100)}%";
        }

        public static NameValueCollection SettingsCollection = null;

        private void MainForm_Load(object sender, EventArgs e)
        {
            SettingsCollection = null;
            try
            {
                SettingsCollection = ConfigurationManager.GetSection("System.Windows.Forms.ApplicationConfigurationSection") as NameValueCollection;
            }
            catch
            {
            }

            if (SettingsCollection != null)
            {
                dpiAwarenessLabel.Text = $"DPI Awareness = {SettingsCollection.Get("DpiAwareness")}";
            }
            currentDpiLabel.Text = $"Current scaling = {(int)(Math.Round(DeviceDpi / 96.0) * 100)}%";
            formSizeLabel.Text = $"Form size = {Size.Width.ToString()} x {Size.Height.ToString()}";
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            formSizeLabel.Text = $"Form size = {Size.Width.ToString()} x {Size.Height.ToString()}";
        }

    }
}
