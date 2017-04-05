using System.Windows.Forms;

namespace HighDpiDemo
{
    partial class RightAnchor
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.currentDpiLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(1376, 61);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(400, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "This button is anchored to the right";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(698, 29);
            this.label1.TabIndex = 1;
            this.label1.Text = "This form is designed to be wider than the screen, but AutoSize is set to true, a" +
    "nd it is re-sized at runtime. Before the fix the button was moving right.";
            // 
            // currentDpiLabel
            // 
            this.currentDpiLabel.AutoSize = true;
            this.currentDpiLabel.Location = new System.Drawing.Point(15, 38);
            this.currentDpiLabel.Name = "currentDpiLabel";
            this.currentDpiLabel.Size = new System.Drawing.Size(62, 13);
            this.currentDpiLabel.TabIndex = 2;
            this.currentDpiLabel.Text = "Current DPI";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "label2";
            // 
            // RightAnchor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1920, 96);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.currentDpiLabel);
            this.Name = "RightAnchor";
            this.Text = "Anchor Layout Improvement ";
            this.Load += new System.EventHandler(this.RightAnchor_Load);
            this.DpiChanged += new System.Windows.Forms.DpiChangedEventHandler(this.RightAnchor_DpiChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label currentDpiLabel;
        private Label label2;
    }
}