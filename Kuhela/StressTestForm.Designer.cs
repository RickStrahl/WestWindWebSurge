namespace Kuhela
{
    partial class StressTestForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StressTestForm));
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.lblStatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatusFilename = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtProcessingTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.HorizontalSplitContainer = new System.Windows.Forms.SplitContainer();
            this.TopSplitContainer = new System.Windows.Forms.SplitContainer();
            this.txtTimeToRun = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtThreads = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.OptionsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.BottomSplitContainer = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbListDisplayMode = new System.Windows.Forms.ComboBox();
            this.ListResults = new System.Windows.Forms.ListView();
            this.Status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Request = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ErrorMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Images = new System.Windows.Forms.ImageList(this.components);
            this.TabsResult = new System.Windows.Forms.TabControl();
            this.tabOutput = new System.Windows.Forms.TabPage();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.tabPreview = new System.Windows.Forms.TabPage();
            this.PreViewBrowser = new System.Windows.Forms.WebBrowser();
            this.ProcessToolstrip = new System.Windows.Forms.ToolStrip();
            this.tbStart = new System.Windows.Forms.ToolStripButton();
            this.tbStop = new System.Windows.Forms.ToolStripButton();
            this.MainToolStrip = new System.Windows.Forms.ToolStrip();
            this.tbOpen = new System.Windows.Forms.ToolStripButton();
            this.tbEditFile = new System.Windows.Forms.ToolStripButton();
            this.tbExit = new System.Windows.Forms.ToolStripButton();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripContainer.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.statusBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HorizontalSplitContainer)).BeginInit();
            this.HorizontalSplitContainer.Panel1.SuspendLayout();
            this.HorizontalSplitContainer.Panel2.SuspendLayout();
            this.HorizontalSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TopSplitContainer)).BeginInit();
            this.TopSplitContainer.Panel1.SuspendLayout();
            this.TopSplitContainer.Panel2.SuspendLayout();
            this.TopSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BottomSplitContainer)).BeginInit();
            this.BottomSplitContainer.Panel1.SuspendLayout();
            this.BottomSplitContainer.Panel2.SuspendLayout();
            this.BottomSplitContainer.SuspendLayout();
            this.TabsResult.SuspendLayout();
            this.tabOutput.SuspendLayout();
            this.tabPreview.SuspendLayout();
            this.ProcessToolstrip.SuspendLayout();
            this.MainToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.BottomToolStripPanel
            // 
            this.toolStripContainer.BottomToolStripPanel.Controls.Add(this.statusBar);
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Controls.Add(this.HorizontalSplitContainer);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(1272, 728);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.Size = new System.Drawing.Size(1272, 773);
            this.toolStripContainer.TabIndex = 0;
            this.toolStripContainer.Text = "toolStripContainer1";
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.MainToolStrip);
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.ProcessToolstrip);
            // 
            // statusBar
            // 
            this.statusBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusBar.Dock = System.Windows.Forms.DockStyle.None;
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatusText,
            this.lblStatusFilename,
            this.txtProcessingTime});
            this.statusBar.Location = new System.Drawing.Point(0, 0);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(1272, 22);
            this.statusBar.TabIndex = 0;
            this.statusBar.Text = "statusStrip1";
            // 
            // lblStatusText
            // 
            this.lblStatusText.AutoSize = false;
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.Size = new System.Drawing.Size(300, 17);
            this.lblStatusText.Text = "Ready";
            this.lblStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatusFilename
            // 
            this.lblStatusFilename.DoubleClickEnabled = true;
            this.lblStatusFilename.Name = "lblStatusFilename";
            this.lblStatusFilename.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblStatusFilename.Size = new System.Drawing.Size(947, 17);
            this.lblStatusFilename.Spring = true;
            this.lblStatusFilename.Text = "No Fiddler Session File selected";
            this.lblStatusFilename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtProcessingTime
            // 
            this.txtProcessingTime.DoubleClickEnabled = true;
            this.txtProcessingTime.Name = "txtProcessingTime";
            this.txtProcessingTime.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.txtProcessingTime.Size = new System.Drawing.Size(10, 17);
            this.txtProcessingTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.txtProcessingTime.Click += new System.EventHandler(this.ButtonHandler_Click);
            // 
            // HorizontalSplitContainer
            // 
            this.HorizontalSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HorizontalSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.HorizontalSplitContainer.Name = "HorizontalSplitContainer";
            this.HorizontalSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // HorizontalSplitContainer.Panel1
            // 
            this.HorizontalSplitContainer.Panel1.BackColor = System.Drawing.Color.White;
            this.HorizontalSplitContainer.Panel1.Controls.Add(this.TopSplitContainer);
            this.HorizontalSplitContainer.Panel1MinSize = 100;
            // 
            // HorizontalSplitContainer.Panel2
            // 
            this.HorizontalSplitContainer.Panel2.Controls.Add(this.BottomSplitContainer);
            this.HorizontalSplitContainer.Panel2MinSize = 100;
            this.HorizontalSplitContainer.Size = new System.Drawing.Size(1272, 728);
            this.HorizontalSplitContainer.SplitterDistance = 201;
            this.HorizontalSplitContainer.SplitterWidth = 7;
            this.HorizontalSplitContainer.TabIndex = 0;
            // 
            // TopSplitContainer
            // 
            this.TopSplitContainer.BackColor = System.Drawing.Color.White;
            this.TopSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TopSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.TopSplitContainer.Name = "TopSplitContainer";
            // 
            // TopSplitContainer.Panel1
            // 
            this.TopSplitContainer.Panel1.BackColor = System.Drawing.Color.White;
            this.TopSplitContainer.Panel1.Controls.Add(this.txtTimeToRun);
            this.TopSplitContainer.Panel1.Controls.Add(this.label8);
            this.TopSplitContainer.Panel1.Controls.Add(this.txtThreads);
            this.TopSplitContainer.Panel1.Controls.Add(this.label9);
            this.TopSplitContainer.Panel1.Controls.Add(this.label3);
            // 
            // TopSplitContainer.Panel2
            // 
            this.TopSplitContainer.Panel2.Controls.Add(this.OptionsPropertyGrid);
            this.TopSplitContainer.Size = new System.Drawing.Size(1272, 201);
            this.TopSplitContainer.SplitterDistance = 894;
            this.TopSplitContainer.TabIndex = 11;
            // 
            // txtTimeToRun
            // 
            this.txtTimeToRun.Location = new System.Drawing.Point(121, 52);
            this.txtTimeToRun.Name = "txtTimeToRun";
            this.txtTimeToRun.Size = new System.Drawing.Size(67, 23);
            this.txtTimeToRun.TabIndex = 7;
            this.txtTimeToRun.Text = "10";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label8.Location = new System.Drawing.Point(191, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 15);
            this.label8.TabIndex = 9;
            this.label8.Text = "secs";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtThreads
            // 
            this.txtThreads.Location = new System.Drawing.Point(121, 22);
            this.txtThreads.Name = "txtThreads";
            this.txtThreads.Size = new System.Drawing.Size(67, 23);
            this.txtThreads.TabIndex = 5;
            this.txtThreads.Text = "2";
            // 
            // label9
            // 
            this.label9.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label9.Location = new System.Drawing.Point(43, 56);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 15);
            this.label9.TabIndex = 8;
            this.label9.Text = "Time to run:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label3.Location = new System.Drawing.Point(57, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 19);
            this.label3.TabIndex = 6;
            this.label3.Text = "Threads:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // OptionsPropertyGrid
            // 
            this.OptionsPropertyGrid.BackColor = System.Drawing.Color.White;
            this.OptionsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OptionsPropertyGrid.HelpBackColor = System.Drawing.Color.White;
            this.OptionsPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.OptionsPropertyGrid.Name = "OptionsPropertyGrid";
            this.OptionsPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.OptionsPropertyGrid.Size = new System.Drawing.Size(374, 201);
            this.OptionsPropertyGrid.TabIndex = 0;
            this.OptionsPropertyGrid.ToolbarVisible = false;
            // 
            // BottomSplitContainer
            // 
            this.BottomSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BottomSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.BottomSplitContainer.Name = "BottomSplitContainer";
            // 
            // BottomSplitContainer.Panel1
            // 
            this.BottomSplitContainer.Panel1.Controls.Add(this.label1);
            this.BottomSplitContainer.Panel1.Controls.Add(this.cmbListDisplayMode);
            this.BottomSplitContainer.Panel1.Controls.Add(this.ListResults);
            // 
            // BottomSplitContainer.Panel2
            // 
            this.BottomSplitContainer.Panel2.Controls.Add(this.TabsResult);
            this.BottomSplitContainer.Size = new System.Drawing.Size(1272, 520);
            this.BottomSplitContainer.SplitterDistance = 625;
            this.BottomSplitContainer.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.label1.Location = new System.Drawing.Point(204, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "1000 items max";
            // 
            // cmbListDisplayMode
            // 
            this.cmbListDisplayMode.FormattingEnabled = true;
            this.cmbListDisplayMode.Items.AddRange(new object[] {
            "Errors",
            "Success"});
            this.cmbListDisplayMode.Location = new System.Drawing.Point(4, 3);
            this.cmbListDisplayMode.Name = "cmbListDisplayMode";
            this.cmbListDisplayMode.Size = new System.Drawing.Size(195, 23);
            this.cmbListDisplayMode.TabIndex = 1;
            this.cmbListDisplayMode.SelectedIndexChanged += new System.EventHandler(this.cmbListDisplayMode_SelectedIndexChanged);
            // 
            // ListResults
            // 
            this.ListResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Status,
            this.Request,
            this.ErrorMessage});
            this.ListResults.FullRowSelect = true;
            this.ListResults.GridLines = true;
            this.ListResults.Location = new System.Drawing.Point(0, 27);
            this.ListResults.Name = "ListResults";
            this.ListResults.ShowItemToolTips = true;
            this.ListResults.Size = new System.Drawing.Size(625, 493);
            this.ListResults.SmallImageList = this.Images;
            this.ListResults.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.ListResults.TabIndex = 0;
            this.ListResults.UseCompatibleStateImageBehavior = false;
            this.ListResults.View = System.Windows.Forms.View.Details;
            this.ListResults.VirtualListSize = 50;
            this.ListResults.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListResults_ItemSelectionChanged);
            // 
            // Status
            // 
            this.Status.Text = "Status";
            // 
            // Request
            // 
            this.Request.Text = "Request";
            this.Request.Width = 356;
            // 
            // ErrorMessage
            // 
            this.ErrorMessage.Text = "Error Message";
            this.ErrorMessage.Width = 209;
            // 
            // Images
            // 
            this.Images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("Images.ImageStream")));
            this.Images.TransparentColor = System.Drawing.Color.Transparent;
            this.Images.Images.SetKeyName(0, "ok");
            this.Images.Images.SetKeyName(1, "error");
            // 
            // TabsResult
            // 
            this.TabsResult.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.TabsResult.Controls.Add(this.tabOutput);
            this.TabsResult.Controls.Add(this.tabPreview);
            this.TabsResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabsResult.ItemSize = new System.Drawing.Size(90, 22);
            this.TabsResult.Location = new System.Drawing.Point(0, 0);
            this.TabsResult.Margin = new System.Windows.Forms.Padding(0);
            this.TabsResult.Name = "TabsResult";
            this.TabsResult.Padding = new System.Drawing.Point(0, 0);
            this.TabsResult.SelectedIndex = 0;
            this.TabsResult.Size = new System.Drawing.Size(643, 520);
            this.TabsResult.TabIndex = 1;
            // 
            // tabOutput
            // 
            this.tabOutput.Controls.Add(this.txtConsole);
            this.tabOutput.Location = new System.Drawing.Point(4, 4);
            this.tabOutput.Margin = new System.Windows.Forms.Padding(0);
            this.tabOutput.Name = "tabOutput";
            this.tabOutput.Size = new System.Drawing.Size(635, 490);
            this.tabOutput.TabIndex = 0;
            this.tabOutput.Text = "Output";
            this.tabOutput.UseVisualStyleBackColor = true;
            // 
            // txtConsole
            // 
            this.txtConsole.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConsole.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsole.Location = new System.Drawing.Point(0, 0);
            this.txtConsole.Margin = new System.Windows.Forms.Padding(0);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConsole.Size = new System.Drawing.Size(635, 490);
            this.txtConsole.TabIndex = 2;
            this.txtConsole.WordWrap = false;
            // 
            // tabPreview
            // 
            this.tabPreview.Controls.Add(this.PreViewBrowser);
            this.tabPreview.Location = new System.Drawing.Point(4, 4);
            this.tabPreview.Margin = new System.Windows.Forms.Padding(0);
            this.tabPreview.Name = "tabPreview";
            this.tabPreview.Size = new System.Drawing.Size(635, 490);
            this.tabPreview.TabIndex = 1;
            this.tabPreview.Text = "Preview";
            this.tabPreview.UseVisualStyleBackColor = true;
            // 
            // PreViewBrowser
            // 
            this.PreViewBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PreViewBrowser.IsWebBrowserContextMenuEnabled = false;
            this.PreViewBrowser.Location = new System.Drawing.Point(0, 0);
            this.PreViewBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.PreViewBrowser.Name = "PreViewBrowser";
            this.PreViewBrowser.Size = new System.Drawing.Size(635, 490);
            this.PreViewBrowser.TabIndex = 2;
            // 
            // ProcessToolstrip
            // 
            this.ProcessToolstrip.Dock = System.Windows.Forms.DockStyle.None;
            this.ProcessToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbStart,
            this.tbStop});
            this.ProcessToolstrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.ProcessToolstrip.Location = new System.Drawing.Point(195, 0);
            this.ProcessToolstrip.Name = "ProcessToolstrip";
            this.ProcessToolstrip.Size = new System.Drawing.Size(103, 23);
            this.ProcessToolstrip.TabIndex = 2;
            // 
            // tbStart
            // 
            this.tbStart.Image = ((System.Drawing.Image)(resources.GetObject("tbStart.Image")));
            this.tbStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbStart.Name = "tbStart";
            this.tbStart.Size = new System.Drawing.Size(51, 20);
            this.tbStart.Text = "Start";
            this.tbStart.Click += new System.EventHandler(this.ButtonHandler_Click);
            // 
            // tbStop
            // 
            this.tbStop.Image = ((System.Drawing.Image)(resources.GetObject("tbStop.Image")));
            this.tbStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbStop.Name = "tbStop";
            this.tbStop.Size = new System.Drawing.Size(51, 20);
            this.tbStop.Text = "Stop";
            this.tbStop.Click += new System.EventHandler(this.ButtonHandler_Click);
            // 
            // MainToolStrip
            // 
            this.MainToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.MainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbOpen,
            this.tbEditFile,
            this.tbExit});
            this.MainToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.MainToolStrip.Location = new System.Drawing.Point(3, 0);
            this.MainToolStrip.Name = "MainToolStrip";
            this.MainToolStrip.Size = new System.Drawing.Size(191, 23);
            this.MainToolStrip.TabIndex = 0;
            // 
            // tbOpen
            // 
            this.tbOpen.Image = ((System.Drawing.Image)(resources.GetObject("tbOpen.Image")));
            this.tbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbOpen.Name = "tbOpen";
            this.tbOpen.Size = new System.Drawing.Size(77, 20);
            this.tbOpen.Text = "Open File";
            this.tbOpen.Click += new System.EventHandler(this.ButtonHandler_Click);
            // 
            // tbEditFile
            // 
            this.tbEditFile.Image = ((System.Drawing.Image)(resources.GetObject("tbEditFile.Image")));
            this.tbEditFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbEditFile.Name = "tbEditFile";
            this.tbEditFile.Size = new System.Drawing.Size(68, 20);
            this.tbEditFile.Text = "Edit File";
            this.tbEditFile.Click += new System.EventHandler(this.ButtonHandler_Click);
            // 
            // tbExit
            // 
            this.tbExit.Image = ((System.Drawing.Image)(resources.GetObject("tbExit.Image")));
            this.tbExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbExit.Name = "tbExit";
            this.tbExit.Size = new System.Drawing.Size(45, 20);
            this.tbExit.Text = "E&xit";
            this.tbExit.Click += new System.EventHandler(this.ButtonHandler_Click);
            // 
            // StressTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1272, 773);
            this.Controls.Add(this.toolStripContainer);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StressTestForm";
            this.Text = "Fiddler Session Load Tester";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.StressTestForm_FormClosed);
            this.Load += new System.EventHandler(this.StressTestForm_Load);
            this.toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.PerformLayout();
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.HorizontalSplitContainer.Panel1.ResumeLayout(false);
            this.HorizontalSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.HorizontalSplitContainer)).EndInit();
            this.HorizontalSplitContainer.ResumeLayout(false);
            this.TopSplitContainer.Panel1.ResumeLayout(false);
            this.TopSplitContainer.Panel1.PerformLayout();
            this.TopSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TopSplitContainer)).EndInit();
            this.TopSplitContainer.ResumeLayout(false);
            this.BottomSplitContainer.Panel1.ResumeLayout(false);
            this.BottomSplitContainer.Panel1.PerformLayout();
            this.BottomSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BottomSplitContainer)).EndInit();
            this.BottomSplitContainer.ResumeLayout(false);
            this.TabsResult.ResumeLayout(false);
            this.tabOutput.ResumeLayout(false);
            this.tabOutput.PerformLayout();
            this.tabPreview.ResumeLayout(false);
            this.ProcessToolstrip.ResumeLayout(false);
            this.ProcessToolstrip.PerformLayout();
            this.MainToolStrip.ResumeLayout(false);
            this.MainToolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusText;
        private System.Windows.Forms.SplitContainer HorizontalSplitContainer;
        private System.Windows.Forms.ToolStrip MainToolStrip;
        private System.Windows.Forms.ToolStripButton tbOpen;
        private System.Windows.Forms.ToolStripButton tbExit;
        private System.Windows.Forms.ToolStripStatusLabel txtProcessingTime;
        private System.Windows.Forms.SplitContainer BottomSplitContainer;
        private System.Windows.Forms.ListView ListResults;
        private System.Windows.Forms.ToolStrip ProcessToolstrip;
        private System.Windows.Forms.ToolStripButton tbStart;
        private System.Windows.Forms.ToolStripButton tbStop;
        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.ImageList Images;
        private System.Windows.Forms.ColumnHeader Request;
        private System.Windows.Forms.ColumnHeader ErrorMessage;
        private System.Windows.Forms.ColumnHeader Status;
        private System.Windows.Forms.ToolStripButton tbEditFile;
        private System.Windows.Forms.SplitContainer TopSplitContainer;
        private System.Windows.Forms.TextBox txtTimeToRun;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtThreads;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PropertyGrid OptionsPropertyGrid;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusFilename;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbListDisplayMode;
        private System.Windows.Forms.TabControl TabsResult;
        private System.Windows.Forms.TabPage tabOutput;
        private System.Windows.Forms.TabPage tabPreview;
        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.WebBrowser PreViewBrowser;
    }
}

