using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PerMonitorDemo
{
    public partial class CurrentDPILabel : Label
    {

        public CurrentDPILabel()
        {
            DpiChangedAfterParent += CurrentDPILabel_DpiChangedAfterParent;
            HandleCreated += CurrentDPILabel_HandleCreated;
        }


        [DefaultValue(false)]
        public override bool AutoSize {
            get { return base.AutoSize; }
            set { base.AutoSize = value; }
        }

        private void CurrentDPILabel_HandleCreated(object sender, EventArgs e)
        {
            SetText();
        }

        private void CurrentDPILabel_DpiChangedAfterParent(object sender, EventArgs e)
        {
            SetText();
        }

        private void SetText()
        {
            Text = $"Current scaling is {(int)Math.Round((DeviceDpi / 96.0) * 100)}%";
        }
    }
}
