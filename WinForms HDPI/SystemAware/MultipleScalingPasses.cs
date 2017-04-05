using System;
using System.Windows.Forms;

namespace HighDpiDemo
{
    public partial class MultipleScalingPasses : Form
    {
        public MultipleScalingPasses()
        {
            InitializeComponent();
        }

        private void MultipleScalingPasses_Load(object sender, EventArgs e)
        {
            currentDpiLabel.Text = $"Current scaling = {(int)Math.Round((DeviceDpi / 96.0) * 100)}%";
            listBoxSizeLabel.Text = $"Height of the ListBox = {listBox1.Size.Height}";
            if (MainForm.SettingsCollection != null)
            {
                string value = MainForm.SettingsCollection.Get("Form.DisableSinglePassScalingOfDpiForms");
                bool enabled = !string.Equals("true", value, StringComparison.InvariantCultureIgnoreCase);
                label2.Text = "Re-scaling fix is " + (enabled ? "enabled" : "disabled");
            }
        }

        private void listBox1_SizeChanged(object sender, EventArgs e)
        {
            listBoxSizeLabel.Text = $"Height of the ListBox = {listBox1.Size.Height}";
        }

        private void MultipleScalingPasses_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            currentDpiLabel.Text = $"Current scaling = {(int)Math.Round((DeviceDpi / 96.0) * 100)}%";
            listBoxSizeLabel.Text = $"Height of the ListBox = {listBox1.Size.Height}";
        }
    }
}
