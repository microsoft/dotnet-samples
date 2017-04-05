using System;
using System.Drawing;
using System.Windows.Forms;

namespace HighDpiDemo
{
    public partial class MenuStripAndCheckedListBox : Form
    {
        public MenuStripAndCheckedListBox()
        {
            InitializeComponent();
        }

        private void MenuStripScaling_Load(object sender, EventArgs e)
        {
            checkedListBox1.Items.Add("Pennsylvania", CheckState.Checked);
            currentDpiLabel.Text = $"Current scaling = {(int)Math.Round((DeviceDpi / 96.0) * 100)}%";
            if (MainForm.SettingsCollection != null)
            {
                string value = MainForm.SettingsCollection.Get("ToolStrip.DisableHighDpiImprovements");
                bool enabled = !string.Equals("true", value, StringComparison.InvariantCultureIgnoreCase);
                label1.Text = "ToolStrip fix is " + (enabled ? "enabled" : "disabled");

                value = MainForm.SettingsCollection.Get("CheckedListBox.DisableHighDpiImprovements");
                enabled = !string.Equals("true", value, StringComparison.InvariantCultureIgnoreCase);
                label1.Text += " CheckedListBox fix is " + (enabled ? "enabled" : "disabled");

            }
        }

        private void MenuStripAndCheckedListBox_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            currentDpiLabel.Text = $"Current scaling = {(int)Math.Round((DeviceDpi / 96.0) * 100)}%";

            this.menuStrip1.SuspendLayout();

            float factor = (float)e.DeviceDpiNew / e.DeviceDpiOld;

            //foreach (ToolStripMenuItem item in menuStrip1.Items)
            //{
            //    item.Size = new Size((int)Math.Round(factor * item.Width), (int)Math.Round(factor * item.Height));
            //}

            //int width = menuStrip1.Width;
            //int height = menuStrip1.Height;
            //menuStrip1.Size = new Size((int)Math.Round(factor * menuStrip1.Width), (int)Math.Round(factor * menuStrip1.Height));

            Font f = menuStrip1.Font;
            menuStrip1.Font = new Font(f.FontFamily, f.Size * factor, f.Style);

            this.menuStrip1.ResumeLayout();

        }
    }
}
