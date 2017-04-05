namespace PerMonitorDemo
{
    partial class MainForm
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
            this.showLayoutButton = new System.Windows.Forms.Button();
            this.buttonImages = new System.Windows.Forms.Button();
            this.dpiAwarenessLabel = new System.Windows.Forms.Label();
            this.formSizeLabel = new System.Windows.Forms.Label();
            this.customDrawing = new System.Windows.Forms.Button();
            this.currentDPILabel1 = new PerMonitorDemo.CurrentDPILabel();
            this.SuspendLayout();
            // 
            // showLayoutButton
            // 
            this.showLayoutButton.Location = new System.Drawing.Point(66, 19);
            this.showLayoutButton.Name = "showLayoutButton";
            this.showLayoutButton.Size = new System.Drawing.Size(231, 23);
            this.showLayoutButton.TabIndex = 0;
            this.showLayoutButton.Text = "Simple Layout";
            this.showLayoutButton.UseVisualStyleBackColor = true;
            this.showLayoutButton.Click += new System.EventHandler(this.showLayoutButton_Click);
            // 
            // buttonImages
            // 
            this.buttonImages.Location = new System.Drawing.Point(66, 50);
            this.buttonImages.Name = "buttonImages";
            this.buttonImages.Size = new System.Drawing.Size(231, 23);
            this.buttonImages.TabIndex = 1;
            this.buttonImages.Text = "Scale Button Images";
            this.buttonImages.UseVisualStyleBackColor = true;
            this.buttonImages.Click += new System.EventHandler(this.buttonImages_Click);
            // 
            // dpiAwarenessLabel
            // 
            this.dpiAwarenessLabel.AutoSize = true;
            this.dpiAwarenessLabel.Location = new System.Drawing.Point(66, 219);
            this.dpiAwarenessLabel.Name = "dpiAwarenessLabel";
            this.dpiAwarenessLabel.Size = new System.Drawing.Size(83, 13);
            this.dpiAwarenessLabel.TabIndex = 3;
            this.dpiAwarenessLabel.Text = "DPI Awareness:";
            // 
            // formSizeLabel
            // 
            this.formSizeLabel.AutoSize = true;
            this.formSizeLabel.Location = new System.Drawing.Point(66, 197);
            this.formSizeLabel.Name = "formSizeLabel";
            this.formSizeLabel.Size = new System.Drawing.Size(56, 13);
            this.formSizeLabel.TabIndex = 10;
            this.formSizeLabel.Text = "Form Size:";
            // 
            // customDrawing
            // 
            this.customDrawing.Location = new System.Drawing.Point(66, 79);
            this.customDrawing.Name = "customDrawing";
            this.customDrawing.Size = new System.Drawing.Size(231, 23);
            this.customDrawing.TabIndex = 14;
            this.customDrawing.Text = "Custom Drawing";
            this.customDrawing.UseVisualStyleBackColor = true;
            this.customDrawing.Click += new System.EventHandler(this.customDrawing_Click);
            // 
            // currentDPILabel1
            // 
            this.currentDPILabel1.Location = new System.Drawing.Point(66, 173);
            this.currentDPILabel1.Name = "currentDPILabel1";
            this.currentDPILabel1.Size = new System.Drawing.Size(231, 13);
            this.currentDPILabel1.TabIndex = 16;
            this.currentDPILabel1.Text = "Current scaling is 100%";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(363, 251);
            this.Controls.Add(this.currentDPILabel1);
            this.Controls.Add(this.customDrawing);
            this.Controls.Add(this.buttonImages);
            this.Controls.Add(this.showLayoutButton);
            this.Controls.Add(this.dpiAwarenessLabel);
            this.Controls.Add(this.formSizeLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MainForm";
            this.Text = "PerMonitor Demo";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button showLayoutButton;
        private System.Windows.Forms.Button buttonImages;
        private System.Windows.Forms.Label dpiAwarenessLabel;
        private System.Windows.Forms.Label formSizeLabel;
        private System.Windows.Forms.Button customDrawing;
        private CurrentDPILabel currentDPILabel1;
    }
}

