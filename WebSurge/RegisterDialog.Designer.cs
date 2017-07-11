namespace WebSurge
{
    partial class RegisterDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterDialog));
            this.StartupTimer = new System.Windows.Forms.Timer(this.components);
            this.PictureLogo = new System.Windows.Forms.PictureBox();
            this.lnkRegister = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblYouAreUsing = new System.Windows.Forms.Label();
            this.lblClickClose = new System.Windows.Forms.Label();
            this.txtUsed = new System.Windows.Forms.Label();
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
            this.PictureLogo.Location = new System.Drawing.Point(1, -1);
            this.PictureLogo.Name = "PictureLogo";
            this.PictureLogo.Size = new System.Drawing.Size(181, 158);
            this.PictureLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureLogo.TabIndex = 0;
            this.PictureLogo.TabStop = false;
            // 
            // lnkRegister
            // 
            this.lnkRegister.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lnkRegister.LinkColor = System.Drawing.SystemColors.ActiveCaption;
            this.lnkRegister.Location = new System.Drawing.Point(202, 275);
            this.lnkRegister.Name = "lnkRegister";
            this.lnkRegister.Size = new System.Drawing.Size(299, 53);
            this.lnkRegister.TabIndex = 13;
            this.lnkRegister.TabStop = true;
            this.lnkRegister.Text = "Register your Copy";
            this.lnkRegister.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkRegister.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkRegister_LinkClicked);
            this.lnkRegister.Click += new System.EventHandler(this.lnkRegister_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label2.ForeColor = System.Drawing.SystemColors.Info;
            this.label2.Location = new System.Drawing.Point(205, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(296, 127);
            this.label2.TabIndex = 12;
            this.label2.Text = "If you\'re using this tool commercially, or you find that you\'re using it frequent" +
    "ly, please purchase a registered version of WebSurge.\r\n\r\nThanks for playing fair" +
    ".";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 21F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label1.Location = new System.Drawing.Point(216, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(293, 41);
            this.label1.TabIndex = 11;
            this.label1.Text = "Westwind WebSurge";
            // 
            // lblYouAreUsing
            // 
            this.lblYouAreUsing.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblYouAreUsing.ForeColor = System.Drawing.SystemColors.Info;
            this.lblYouAreUsing.Location = new System.Drawing.Point(205, 17);
            this.lblYouAreUsing.Name = "lblYouAreUsing";
            this.lblYouAreUsing.Size = new System.Drawing.Size(284, 21);
            this.lblYouAreUsing.TabIndex = 10;
            this.lblYouAreUsing.Text = "You\'ve been using the Free version of";
            // 
            // lblClickClose
            // 
            this.lblClickClose.AutoSize = true;
            this.lblClickClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblClickClose.Font = new System.Drawing.Font("Arial Rounded MT Bold", 14F);
            this.lblClickClose.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblClickClose.Location = new System.Drawing.Point(513, 7);
            this.lblClickClose.Name = "lblClickClose";
            this.lblClickClose.Size = new System.Drawing.Size(20, 22);
            this.lblClickClose.TabIndex = 0;
            this.lblClickClose.Text = "x";
            this.lblClickClose.Click += new System.EventHandler(this.lblClickClose_Click);
            // 
            // txtUsed
            // 
            this.txtUsed.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.txtUsed.ForeColor = System.Drawing.SystemColors.Info;
            this.txtUsed.Location = new System.Drawing.Point(204, 99);
            this.txtUsed.Name = "txtUsed";
            this.txtUsed.Size = new System.Drawing.Size(296, 25);
            this.txtUsed.TabIndex = 15;
            this.txtUsed.Text = "x times";
            // 
            // RegisterDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(540, 364);
            this.Controls.Add(this.txtUsed);
            this.Controls.Add(this.lblClickClose);
            this.Controls.Add(this.lnkRegister);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblYouAreUsing);
            this.Controls.Add(this.PictureLogo);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RegisterDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About West Wind Web Surge";
            this.TransparencyKey = System.Drawing.Color.White;
            this.Load += new System.EventHandler(this.Splash_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Splash_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.PictureLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer StartupTimer;
        private System.Windows.Forms.PictureBox PictureLogo;
        private System.Windows.Forms.LinkLabel lnkRegister;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblYouAreUsing;
        private System.Windows.Forms.Label lblClickClose;
        private System.Windows.Forms.Label txtUsed;
    }
}