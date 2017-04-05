using System;
using System.Windows.Forms;

namespace HighDpiDemo
{
    public partial class RightAnchor : Form
    {
        public RightAnchor()
        {
            InitializeComponent();
        }

        private void RightAnchor_Load(object sender, EventArgs e)
        {
            currentDpiLabel.Text = $"Current scaling = {(int)Math.Round((DeviceDpi / 96.0) * 100)}%";
            if (MainForm.SettingsCollection != null)
            {
                string value = MainForm.SettingsCollection.Get("AnchorLayout.DisableHighDpiImprovements");
                bool enabled = !string.Equals("true", value, StringComparison.InvariantCultureIgnoreCase);
                label2.Text = "Re-scaling fix is " + (enabled ? "enabled" : "disabled");
            }

        }

        private void RightAnchor_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            currentDpiLabel.Text = $"Current scaling = {(int)Math.Round((DeviceDpi / 96.0) * 100)}%";
        }
    }
}
