namespace HighDpiDemo
{
    partial class MultipleScalingPasses
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.currentDpiLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.listBoxSizeLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Items.AddRange(new object[] {
            "United States Of America",
            "Canada",
            "New Zealand",
            "Australia",
            "Great Britain"});
            this.listBox1.Location = new System.Drawing.Point(177, 52);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(195, 79);
            this.listBox1.TabIndex = 0;
            this.listBox1.SizeChanged += new System.EventHandler(this.listBox1_SizeChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(309, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "ListBox is designed at 96 DPI and holds exactly 5 items.";
            // 
            // currentDpiLabel
            // 
            this.currentDpiLabel.AutoSize = true;
            this.currentDpiLabel.Location = new System.Drawing.Point(16, 52);
            this.currentDpiLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.currentDpiLabel.Name = "currentDpiLabel";
            this.currentDpiLabel.Size = new System.Drawing.Size(74, 15);
            this.currentDpiLabel.TabIndex = 2;
            this.currentDpiLabel.Text = "current DPI: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(271, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "This text overlays the ListBox if the fix is disabled.";
            // 
            // listBoxSizeLabel
            // 
            this.listBoxSizeLabel.AutoSize = true;
            this.listBoxSizeLabel.Location = new System.Drawing.Point(16, 160);
            this.listBoxSizeLabel.Name = "listBoxSizeLabel";
            this.listBoxSizeLabel.Size = new System.Drawing.Size(107, 15);
            this.listBoxSizeLabel.TabIndex = 5;
            this.listBoxSizeLabel.Text = "Size of the ListBox";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 188);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "label2";
            // 
            // MultipleScalingPasses
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(384, 211);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listBoxSizeLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.currentDpiLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MultipleScalingPasses";
            this.Text = "Control height scales incorrectly";
            this.Load += new System.EventHandler(this.MultipleScalingPasses_Load);
            this.DpiChanged += new System.Windows.Forms.DpiChangedEventHandler(this.MultipleScalingPasses_DpiChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label currentDpiLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label listBoxSizeLabel;
        private System.Windows.Forms.Label label2;
    }
}

