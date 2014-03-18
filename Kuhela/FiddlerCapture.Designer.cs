namespace Kuhela
{
    partial class FiddlerCapture
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FiddlerCapture));
            this.txtCapture = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tbCapture = new System.Windows.Forms.ToolStripButton();
            this.tbStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tbSave = new System.Windows.Forms.ToolStripButton();
            this.tbClear = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbIgnoreResources = new System.Windows.Forms.ToolStripButton();
            this.tbtxtProcessId = new System.Windows.Forms.ToolStripTextBox();
            this.tblblProcessId = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtCapture
            // 
            this.txtCapture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCapture.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCapture.Location = new System.Drawing.Point(-1, 28);
            this.txtCapture.Multiline = true;
            this.txtCapture.Name = "txtCapture";
            this.txtCapture.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCapture.Size = new System.Drawing.Size(782, 544);
            this.txtCapture.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbCapture,
            this.tbStop,
            this.toolStripSeparator2,
            this.tbSave,
            this.tbClear,
            this.toolStripSeparator1,
            this.tbIgnoreResources,
            this.tblblProcessId,
            this.tbtxtProcessId});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(780, 25);
            this.toolStrip1.TabIndex = 5;
            // 
            // tbCapture
            // 
            this.tbCapture.Image = ((System.Drawing.Image)(resources.GetObject("tbCapture.Image")));
            this.tbCapture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbCapture.Name = "tbCapture";
            this.tbCapture.Size = new System.Drawing.Size(69, 22);
            this.tbCapture.Text = "Capture";
            this.tbCapture.ToolTipText = "Start capturing HTTP requests.";
            this.tbCapture.Click += new System.EventHandler(this.ButtonHandler);
            // 
            // tbStop
            // 
            this.tbStop.Image = ((System.Drawing.Image)(resources.GetObject("tbStop.Image")));
            this.tbStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbStop.Name = "tbStop";
            this.tbStop.Size = new System.Drawing.Size(96, 22);
            this.tbStop.Text = "Stop Capture";
            this.tbStop.ToolTipText = "Stop capturing HTTP Requests";
            this.tbStop.Click += new System.EventHandler(this.ButtonHandler);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tbSave
            // 
            this.tbSave.Image = ((System.Drawing.Image)(resources.GetObject("tbSave.Image")));
            this.tbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSave.Name = "tbSave";
            this.tbSave.Size = new System.Drawing.Size(51, 22);
            this.tbSave.Text = "Save";
            this.tbSave.ToolTipText = "Saves captured URLs to a file";
            this.tbSave.Click += new System.EventHandler(this.ButtonHandler);
            // 
            // tbClear
            // 
            this.tbClear.Image = ((System.Drawing.Image)(resources.GetObject("tbClear.Image")));
            this.tbClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbClear.Name = "tbClear";
            this.tbClear.Size = new System.Drawing.Size(54, 22);
            this.tbClear.Text = "Clear";
            this.tbClear.ToolTipText = "Clears the captured Urls";
            this.tbClear.Click += new System.EventHandler(this.ButtonHandler);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tbIgnoreResources
            // 
            this.tbIgnoreResources.CheckOnClick = true;
            this.tbIgnoreResources.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbIgnoreResources.Image = ((System.Drawing.Image)(resources.GetObject("tbIgnoreResources.Image")));
            this.tbIgnoreResources.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbIgnoreResources.Name = "tbIgnoreResources";
            this.tbIgnoreResources.Size = new System.Drawing.Size(23, 22);
            this.tbIgnoreResources.ToolTipText = "Ignore Images, CSS and JavaScript links";
            // 
            // tbtxtProcessId
            // 
            this.tbtxtProcessId.Name = "tbtxtProcessId";
            this.tbtxtProcessId.Size = new System.Drawing.Size(60, 25);
            // 
            // tblblProcessId
            // 
            this.tblblProcessId.Name = "tblblProcessId";
            this.tblblProcessId.Size = new System.Drawing.Size(63, 22);
            this.tblblProcessId.Text = "Process Id:";
            // 
            // FiddlerCapture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 571);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.txtCapture);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FiddlerCapture";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Kuhela Url Capture";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FiddlerCapture_FormClosing);
            this.Load += new System.EventHandler(this.FiddlerCapture_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCapture;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tbCapture;
        private System.Windows.Forms.ToolStripButton tbStop;
        private System.Windows.Forms.ToolStripButton tbSave;
        private System.Windows.Forms.ToolStripButton tbIgnoreResources;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tbClear;
        private System.Windows.Forms.ToolStripLabel tblblProcessId;
        private System.Windows.Forms.ToolStripTextBox tbtxtProcessId;
    }
}