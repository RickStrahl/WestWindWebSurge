namespace WebSurge
{
    partial class DistributionGraphOptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DistributionGraphOptionsForm));
            this.pnlButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnApply = new System.Windows.Forms.Button();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.pgProperties = new System.Windows.Forms.PropertyGrid();
            this.pnlButtons.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlButtons
            // 
            this.pnlButtons.ColumnCount = 3;
            this.pnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.pnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlButtons.Controls.Add(this.btnApply, 1, 0);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 367);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.RowCount = 1;
            this.pnlButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlButtons.Size = new System.Drawing.Size(544, 46);
            this.pnlButtons.TabIndex = 0;
            // 
            // btnApply
            // 
            this.btnApply.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnApply.Location = new System.Drawing.Point(225, 3);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(94, 40);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // pnlOptions
            // 
            this.pnlOptions.Controls.Add(this.pgProperties);
            this.pnlOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOptions.Location = new System.Drawing.Point(0, 0);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(544, 367);
            this.pnlOptions.TabIndex = 1;
            // 
            // pgProperties
            // 
            this.pgProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgProperties.Location = new System.Drawing.Point(0, 0);
            this.pgProperties.Name = "pgProperties";
            this.pgProperties.Size = new System.Drawing.Size(544, 367);
            this.pgProperties.TabIndex = 0;
            this.pgProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgProperties_PropertyValueChanged);
            // 
            // DistributionGraphOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 413);
            this.Controls.Add(this.pnlOptions);
            this.Controls.Add(this.pnlButtons);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DistributionGraphOptionsForm";
            this.Text = "Distribution Graph Options";
            this.pnlButtons.ResumeLayout(false);
            this.pnlOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel pnlButtons;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.PropertyGrid pgProperties;
    }
}