namespace PerMonitorDemo
{
    partial class ScaleButtonImages
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScaleButtonImages));
            this.currentDPILabel1 = new PerMonitorDemo.CurrentDPILabel();
            this.errorButton = new PerMonitorDemo.ImageButton();
            this.propertiesButton = new PerMonitorDemo.ImageButton();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // currentDPILabel1
            // 
            this.currentDPILabel1.Location = new System.Drawing.Point(9, 211);
            this.currentDPILabel1.Name = "currentDPILabel1";
            this.currentDPILabel1.Size = new System.Drawing.Size(245, 16);
            this.currentDPILabel1.TabIndex = 1;
            this.currentDPILabel1.Text = "Current scaling is 100%";
            // 
            // errorButton
            // 
            this.errorButton.Location = new System.Drawing.Point(64, 123);
            this.errorButton.Name = "errorButton";
            this.errorButton.Size = new System.Drawing.Size(36, 36);
            this.errorButton.TabIndex = 3;
            this.errorButton.UseVisualStyleBackColor = true;
            // 
            // propertiesButton
            // 
            this.propertiesButton.Location = new System.Drawing.Point(167, 123);
            this.propertiesButton.Name = "propertiesButton";
            this.propertiesButton.Size = new System.Drawing.Size(36, 36);
            this.propertiesButton.TabIndex = 4;
            this.propertiesButton.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(245, 47);
            this.label2.TabIndex = 6;
            this.label2.Text = "Buttons that select bitmap based on the current DPI.";
            // 
            // ScaleButtonImages
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.propertiesButton);
            this.Controls.Add(this.errorButton);
            this.Controls.Add(this.currentDPILabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScaleButtonImages";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Image Scaling";
            this.ResumeLayout(false);

        }

        #endregion
        private CurrentDPILabel currentDPILabel1;
        private ImageButton errorButton;
        private ImageButton propertiesButton;
        private System.Windows.Forms.Label label2;
    }
}