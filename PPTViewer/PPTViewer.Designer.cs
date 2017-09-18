namespace PPTViewer
{
    partial class PPTViewer
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
            this.button_open = new System.Windows.Forms.Button();
            this.label_show = new System.Windows.Forms.Label();
            this.button_close = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_open
            // 
            this.button_open.Location = new System.Drawing.Point(59, 38);
            this.button_open.Name = "button_open";
            this.button_open.Size = new System.Drawing.Size(158, 24);
            this.button_open.TabIndex = 0;
            this.button_open.Text = "打开PPT";
            this.button_open.UseVisualStyleBackColor = true;
            this.button_open.Click += new System.EventHandler(this.button_open_Click);
            // 
            // label_show
            // 
            this.label_show.AutoEllipsis = true;
            this.label_show.AutoSize = true;
            this.label_show.Location = new System.Drawing.Point(56, 100);
            this.label_show.Name = "label_show";
            this.label_show.Size = new System.Drawing.Size(82, 15);
            this.label_show.TabIndex = 2;
            this.label_show.Text = "。。。。。";
            // 
            // button_close
            // 
            this.button_close.Location = new System.Drawing.Point(63, 74);
            this.button_close.Name = "button_close";
            this.button_close.Size = new System.Drawing.Size(154, 23);
            this.button_close.TabIndex = 3;
            this.button_close.Text = "关闭";
            this.button_close.UseVisualStyleBackColor = true;
            this.button_close.Click += new System.EventHandler(this.button_close_Click);
            // 
            // ChartID
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.button_close);
            this.Controls.Add(this.label_show);
            this.Controls.Add(this.button_open);
            this.Name = "ChartID";
            this.Text = "ChartID";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_open;
        private System.Windows.Forms.Label label_show;
        private System.Windows.Forms.Button button_close;
    }
}

