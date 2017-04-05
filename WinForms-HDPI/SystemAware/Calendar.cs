using System;
using System.Windows.Forms;

namespace HighDpiDemo
{
    public partial class Calendar : Form
    {
        public Calendar()
        {
            InitializeComponent();
            if (MainForm.SettingsCollection != null)
            {
                string value = MainForm.SettingsCollection.Get("MonthCalendar.DisableHighDpiImprovements");
                bool enabled = !string.Equals("true", value, StringComparison.InvariantCultureIgnoreCase);
                label1.Text = "Calendar HDPI Improvement is " + (enabled?"enabled":"disabled");
            }
        }

        private void Calendar_Load(object sender, EventArgs e)
        {
            currentDpiLabel.Text = $"Current scaling = {(int)Math.Round((DeviceDpi / 96.0) * 100)}%";
        }

        private void Calendar_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            currentDpiLabel.Text = $"Current scaling = {(int)Math.Round((DeviceDpi / 96.0) * 100)}%";
        }
    }
}
