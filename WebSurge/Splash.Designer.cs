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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Splash));
            this.StartupTimer = new System.Windows.Forms.Timer(this.components);
            this.PictureLogo = new System.Windows.Forms.PictureBox();
            this.lblRegisterType = new System.Windows.Forms.LinkLabel();
            this.lblVersionText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PictureLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // StartupTimer
            // 
            this.StartupTimer.Interval = 4000;
            this.StartupTimer.Tick += new System.EventHandler(this.StartupTimer_Tick);
            // 
            // PictureLogo
            // 
            this.PictureLogo.BackColor = System.Drawing.Color.Black;
            this.PictureLogo.Image = ((System.Drawing.Image)(resources.GetObject("PictureLogo.Image")));
            this.PictureLogo.Location = new System.Drawing.Point(-18, -16);
            this.PictureLogo.Name = "PictureLogo";
            this.PictureLogo.Size = new System.Drawing.Size(364, 320);
            this.PictureLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureLogo.TabIndex = 0;
            this.PictureLogo.TabStop = false;
            this.PictureLogo.Click += new System.EventHandler(this.PictureLogo_Click);
            // 
            // lblRegisterType
            // 
            this.lblRegisterType.BackColor = System.Drawing.Color.Black;
            this.lblRegisterType.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblRegisterType.LinkColor = System.Drawing.Color.WhiteSmoke;
            this.lblRegisterType.Location = new System.Drawing.Point(261, 14);
            this.lblRegisterType.Name = "lblRegisterType";
            this.lblRegisterType.Size = new System.Drawing.Size(72, 16);
            this.lblRegisterType.TabIndex = 5;
            this.lblRegisterType.TabStop = true;
            this.lblRegisterType.Text = "Professional";
            this.lblRegisterType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblRegisterType.VisitedLinkColor = System.Drawing.Color.WhiteSmoke;
            this.lblRegisterType.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblRegisterType_LinkClicked);
            // 
            // lblVersionText
            // 
            this.lblVersionText.BackColor = System.Drawing.Color.Black;
            this.lblVersionText.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblVersionText.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblVersionText.Location = new System.Drawing.Point(198, 32);
            this.lblVersionText.Name = "lblVersionText";
            this.lblVersionText.Size = new System.Drawing.Size(136, 13);
            this.lblVersionText.TabIndex = 4;
            this.lblVersionText.Text = "Version 0.10";
            this.lblVersionText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Splash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(345, 304);
            this.Controls.Add(this.lblRegisterType);
            this.Controls.Add(this.lblVersionText);
            this.Controls.Add(this.PictureLogo);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Splash";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About West Wind Web Surge";
            this.TransparencyKey = System.Drawing.Color.White;
            this.Load += new System.EventHandler(this.Splash_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Splash_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.PictureLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer StartupTimer;
        private System.Windows.Forms.PictureBox PictureLogo;
        private System.Windows.Forms.LinkLabel lblRegisterType;
        private System.Windows.Forms.Label lblVersionText;
    }
}