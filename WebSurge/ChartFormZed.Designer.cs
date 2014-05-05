namespace WebSurge
{
    partial class ChartFormZed
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartFormZed));
            this.Chart = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // Chart
            // 
            this.Chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Chart.Location = new System.Drawing.Point(0, 0);
            this.Chart.Name = "Chart";
            this.Chart.ScrollGrace = 0D;
            this.Chart.ScrollMaxX = 0D;
            this.Chart.ScrollMaxY = 0D;
            this.Chart.ScrollMaxY2 = 0D;
            this.Chart.ScrollMinX = 0D;
            this.Chart.ScrollMinY = 0D;
            this.Chart.ScrollMinY2 = 0D;
            this.Chart.Size = new System.Drawing.Size(1034, 471);
            this.Chart.TabIndex = 0;
            this.Chart.Load += new System.EventHandler(this.Chart_Load);
            // 
            // ChartFormZed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1034, 471);
            this.Controls.Add(this.Chart);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChartFormZed";
            this.Text = "Request Time Taken";
            this.Load += new System.EventHandler(this.RequestTimeTakenChart_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl Chart;

    }
}