namespace WebSurge
{
    partial class Splash
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Splash));
            this.PictureLogo = new System.Windows.Forms.PictureBox();
            this.lblVersionText = new System.Windows.Forms.Label();
            this.lblRegisterType = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.PictureLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // PictureLogo
            // 
            this.PictureLogo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PictureLogo.Image = ((System.Drawing.Image)(resources.GetObject("PictureLogo.Image")));
            this.PictureLogo.Location = new System.Drawing.Point(0, 0);
            this.PictureLogo.Name = "PictureLogo";
            this.PictureLogo.Size = new System.Drawing.Size(663, 510);
            this.PictureLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureLogo.TabIndex = 0;
            this.PictureLogo.TabStop = false;
            this.PictureLogo.Click += new System.EventHandler(this.PictureLogo_Click);
            // 
            // lblVersionText
            // 
            this.lblVersionText.BackColor = System.Drawing.Color.Black;
            this.lblVersionText.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblVersionText.Location = new System.Drawing.Point(513, 457);
            this.lblVersionText.Name = "lblVersionText";
            this.lblVersionText.Size = new System.Drawing.Size(117, 27);
            this.lblVersionText.TabIndex = 2;
            this.lblVersionText.Text = "Version 0.10";
            this.lblVersionText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblRegisterType
            // 
            this.lblRegisterType.AutoSize = true;
            this.lblRegisterType.BackColor = System.Drawing.Color.Black;
            this.lblRegisterType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblRegisterType.LinkColor = System.Drawing.Color.WhiteSmoke;
            this.lblRegisterType.Location = new System.Drawing.Point(138, 482);
            this.lblRegisterType.Name = "lblRegisterType";
            this.lblRegisterType.Size = new System.Drawing.Size(85, 19);
            this.lblRegisterType.TabIndex = 3;
            this.lblRegisterType.TabStop = true;
            this.lblRegisterType.Text = "Free Version";
            this.lblRegisterType.VisitedLinkColor = System.Drawing.Color.WhiteSmoke;
            this.lblRegisterType.Click += new System.EventHandler(this.lblRegisterType_Click);
            // 
            // Splash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(663, 510);
            this.Controls.Add(this.lblRegisterType);
            this.Controls.Add(this.lblVersionText);
            this.Controls.Add(this.PictureLogo);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Splash";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About West Wind Web Surge";
            this.TransparencyKey = System.Drawing.Color.White;
            this.Deactivate += new System.EventHandler(this.Splash_Deactivate);
            this.Load += new System.EventHandler(this.Splash_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PictureLogo;
        private System.Windows.Forms.Label lblVersionText;
        private System.Windows.Forms.LinkLabel lblRegisterType;
    }
}