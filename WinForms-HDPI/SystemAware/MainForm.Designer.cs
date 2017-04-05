namespace HighDpiDemo
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
            this.showControlsButton = new System.Windows.Forms.Button();
            this.showExceptionDialogButton = new System.Windows.Forms.Button();
            this.currentDpiLabel = new System.Windows.Forms.Label();
            this.dpiAwarenessLabel = new System.Windows.Forms.Label();
            this.formSizeLabel = new System.Windows.Forms.Label();
            this.calendarButton = new System.Windows.Forms.Button();
            this.anchoredControls = new System.Windows.Forms.Button();
            this.dgvHeaders = new System.Windows.Forms.Button();
            this.showLayoutButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // showControlsButton
            // 
            this.showControlsButton.Location = new System.Drawing.Point(67, 76);
            this.showControlsButton.Name = "showControlsButton";
            this.showControlsButton.Size = new System.Drawing.Size(231, 23);
            this.showControlsButton.TabIndex = 2;
            this.showControlsButton.Text = "MenuStrip and CheckedListBox";
            this.showControlsButton.UseVisualStyleBackColor = true;
            this.showControlsButton.Click += new System.EventHandler(this.showControlsButton_Click);
            // 
            // showExceptionDialogButton
            // 
            this.showExceptionDialogButton.Location = new System.Drawing.Point(67, 14);
            this.showExceptionDialogButton.Name = "showExceptionDialogButton";
            this.showExceptionDialogButton.Size = new System.Drawing.Size(231, 23);
            this.showExceptionDialogButton.TabIndex = 0;
            this.showExceptionDialogButton.Text = "Exception Dialog Scaling";
            this.showExceptionDialogButton.UseVisualStyleBackColor = true;
            this.showExceptionDialogButton.Click += new System.EventHandler(this.showExceptionDialogButton_Click);
            // 
            // currentDpiLabel
            // 
            this.currentDpiLabel.AutoSize = true;
            this.currentDpiLabel.Location = new System.Drawing.Point(10, 214);
            this.currentDpiLabel.Name = "currentDpiLabel";
            this.currentDpiLabel.Size = new System.Drawing.Size(65, 13);
            this.currentDpiLabel.TabIndex = 11;
            this.currentDpiLabel.Text = "Current DPI:";
            // 
            // dpiAwarenessLabel
            // 
            this.dpiAwarenessLabel.AutoSize = true;
            this.dpiAwarenessLabel.Location = new System.Drawing.Point(10, 262);
            this.dpiAwarenessLabel.Name = "dpiAwarenessLabel";
            this.dpiAwarenessLabel.Size = new System.Drawing.Size(83, 13);
            this.dpiAwarenessLabel.TabIndex = 3;
            this.dpiAwarenessLabel.Text = "DPI Awareness:";
            // 
            // formSizeLabel
            // 
            this.formSizeLabel.AutoSize = true;
            this.formSizeLabel.Location = new System.Drawing.Point(10, 238);
            this.formSizeLabel.Name = "formSizeLabel";
            this.formSizeLabel.Size = new System.Drawing.Size(56, 13);
            this.formSizeLabel.TabIndex = 10;
            this.formSizeLabel.Text = "Form Size:";
            // 
            // calendarButton
            // 
            this.calendarButton.Location = new System.Drawing.Point(67, 105);
            this.calendarButton.Name = "calendarButton";
            this.calendarButton.Size = new System.Drawing.Size(231, 23);
            this.calendarButton.TabIndex = 3;
            this.calendarButton.Text = "Calendar";
            this.calendarButton.UseVisualStyleBackColor = true;
            this.calendarButton.Click += new System.EventHandler(this.calendarButton_Click);
            // 
            // anchoredControls
            // 
            this.anchoredControls.Location = new System.Drawing.Point(67, 136);
            this.anchoredControls.Name = "anchoredControls";
            this.anchoredControls.Size = new System.Drawing.Size(231, 23);
            this.anchoredControls.TabIndex = 4;
            this.anchoredControls.Text = "Anchored Controls";
            this.anchoredControls.UseVisualStyleBackColor = true;
            this.anchoredControls.Click += new System.EventHandler(this.anchoredControls_Click);
            // 
            // dgvHeaders
            // 
            this.dgvHeaders.Location = new System.Drawing.Point(67, 167);
            this.dgvHeaders.Name = "dgvHeaders";
            this.dgvHeaders.Size = new System.Drawing.Size(231, 23);
            this.dgvHeaders.TabIndex = 5;
            this.dgvHeaders.Text = "DataGridView Headers";
            this.dgvHeaders.UseVisualStyleBackColor = true;
            this.dgvHeaders.Click += new System.EventHandler(this.dgvHeaders_Click);
            // 
            // showLayoutButton
            // 
            this.showLayoutButton.Location = new System.Drawing.Point(67, 43);
            this.showLayoutButton.Name = "showLayoutButton";
            this.showLayoutButton.Size = new System.Drawing.Size(231, 23);
            this.showLayoutButton.TabIndex = 1;
            this.showLayoutButton.Text = "Improved Layout Support";
            this.showLayoutButton.UseVisualStyleBackColor = true;
            this.showLayoutButton.Click += new System.EventHandler(this.showLayoutButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(364, 286);
            this.Controls.Add(this.showLayoutButton);
            this.Controls.Add(this.dgvHeaders);
            this.Controls.Add(this.anchoredControls);
            this.Controls.Add(this.calendarButton);
            this.Controls.Add(this.showExceptionDialogButton);
            this.Controls.Add(this.showControlsButton);
            this.Controls.Add(this.dpiAwarenessLabel);
            this.Controls.Add(this.formSizeLabel);
            this.Controls.Add(this.currentDpiLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MainForm";
            this.Text = "HDPI Demo";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.DpiChanged += new System.Windows.Forms.DpiChangedEventHandler(this.MainForm_DpiChanged);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button showControlsButton;
        private System.Windows.Forms.Button showExceptionDialogButton;
        private System.Windows.Forms.Label currentDpiLabel;
        private System.Windows.Forms.Label dpiAwarenessLabel;
        private System.Windows.Forms.Label formSizeLabel;
        private System.Windows.Forms.Button calendarButton;
        private System.Windows.Forms.Button anchoredControls;
        private System.Windows.Forms.Button dgvHeaders;
        private System.Windows.Forms.Button showLayoutButton;
    }
}

