using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HighDpiDemo
{
    public partial class DataGridViewHeadersScaling : Form
    {
        public DataGridViewHeadersScaling()
        {
            InitializeComponent();
            dataGridView1.Rows[0].ErrorText = "error text";
            dataGridView1.Rows.Add(new object[] { "aaa", "bbbb" });
            dataGridView1.Rows.Add(new object[] { "baaa", "abbbb" });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.RightToLeft = (dataGridView1.RightToLeft == RightToLeft.Yes ? RightToLeft.No: RightToLeft.Yes);
        }

        private void DataGridViewHeadersScaling_Load(object sender, EventArgs e)
        {
            currentDpiLabel.Text = $"Current scalling = {(int)Math.Round((DeviceDpi / 96.0) * 100)}%";
            if (!"true".Equals(MainForm.SettingsCollection["DataGridView.DisableHighDpiImprovements"], StringComparison.InvariantCultureIgnoreCase))
            {
                infoLabel.Text = "Row header resizing improvement is enabled.";
            }
            else
            {
                infoLabel.Text = "Row header resizing improvement is disabled.";
            }
        }

        private void DataGridViewHeadersScaling_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            currentDpiLabel.Text = $"Current scalling = {(int)Math.Round((DeviceDpi / 96.0) * 100)}%";
        }
    }
}
