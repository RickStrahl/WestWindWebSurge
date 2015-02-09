using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using WebSurge;

namespace WebSurge
{
	/// <summary>
	/// Summary description for UnlockForm.
	/// </summary>
	class UnlockKeyForm : Form
    {
        private Label lblNote;
        
		private TextBox txtKey;

		/// <summary>
		/// Required designer variable.
		/// </summary>
        private Container components = null;
        private Label label1;
        private PictureBox PictureLogo;
        private Label label2;
        private LinkLabel btnRegister;
        private Label lblClickClose;

		public string SoftwareName = "WebSurge";

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnlockKeyForm));
            this.txtKey = new System.Windows.Forms.TextBox();
            this.lblNote = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.PictureLogo = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRegister = new System.Windows.Forms.LinkLabel();
            this.lblClickClose = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PictureLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // txtKey
            // 
            this.txtKey.Location = new System.Drawing.Point(151, 132);
            this.txtKey.Name = "txtKey";
            this.txtKey.PasswordChar = '●';
            this.txtKey.Size = new System.Drawing.Size(258, 21);
            this.txtKey.TabIndex = 1;
            // 
            // lblNote
            // 
            this.lblNote.AutoSize = true;
            this.lblNote.ForeColor = System.Drawing.Color.Red;
            this.lblNote.Location = new System.Drawing.Point(152, 156);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(57, 13);
            this.lblNote.TabIndex = 3;
            this.lblNote.Text = "RegStatus";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label1.Location = new System.Drawing.Point(146, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(239, 41);
            this.label1.TabIndex = 12;
            this.label1.Text = "Product Registration";
            // 
            // PictureLogo
            // 
            this.PictureLogo.BackColor = System.Drawing.Color.Black;
            this.PictureLogo.Image = ((System.Drawing.Image)(resources.GetObject("PictureLogo.Image")));
            this.PictureLogo.Location = new System.Drawing.Point(1, 2);
            this.PictureLogo.Name = "PictureLogo";
            this.PictureLogo.Size = new System.Drawing.Size(137, 115);
            this.PictureLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureLogo.TabIndex = 13;
            this.PictureLogo.TabStop = false;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label2.ForeColor = System.Drawing.SystemColors.Info;
            this.label2.Location = new System.Drawing.Point(147, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(275, 79);
            this.label2.TabIndex = 15;
            this.label2.Text = "Thank you for registering your copy of West Wind WebSurge.\r\n\r\nPlease enter your R" +
    "egistration Key below:\r\n";
            // 
            // btnRegister
            // 
            this.btnRegister.Font = new System.Drawing.Font("Segoe UI", 24F);
            this.btnRegister.LinkColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnRegister.Location = new System.Drawing.Point(152, 175);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(257, 45);
            this.btnRegister.TabIndex = 16;
            this.btnRegister.TabStop = true;
            this.btnRegister.Text = "Register";
            this.btnRegister.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnRegister.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnRegister_LinkClicked);
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // lblClickClose
            // 
            this.lblClickClose.AutoSize = true;
            this.lblClickClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblClickClose.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClickClose.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblClickClose.Location = new System.Drawing.Point(411, 3);
            this.lblClickClose.Name = "lblClickClose";
            this.lblClickClose.Size = new System.Drawing.Size(13, 14);
            this.lblClickClose.TabIndex = 17;
            this.lblClickClose.Text = "x";
            this.lblClickClose.Click += new System.EventHandler(this.lblClickClose_Click);
            // 
            // UnlockKeyForm
            // 
            this.AcceptButton = this.btnRegister;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(427, 242);
            this.Controls.Add(this.lblClickClose);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PictureLogo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtKey);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.btnRegister);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "UnlockKeyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Westwind WebSurge Registration";
            this.Load += new System.EventHandler(this.FormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.PictureLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

		public UnlockKeyForm(string ProductName)
		{
			SoftwareName = ProductName;
			InitializeComponent();			
		}

        public static void ShowRegForm(string ProductName)
        {
            UnlockKeyForm Form = new UnlockKeyForm(ProductName);
            Form.TopMost = true;
            Form.ShowDialog();            
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

        private void FormLoad(object sender, EventArgs e)
        {            
            if (UnlockKey.IsRegistered())
                lblNote.Text = "This copy is registered.";
            else
                lblNote.Text = "This copy is not registered.";
        }

		private void btnRegister_Click(object sender, EventArgs e)
		{
            if (UnlockKey.Register(txtKey.Text))
            {
                MessageBox.Show("Thank you for your registration.", ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                MessageBox.Show("Sorry, that's not the right key.\r\nMake sure you entered the value exactly as it\r\n" +
                    "appears in your confirmation.\r\n\r\n", "Software Registration",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                lblNote.Text = "This copy is not registered.";
                UnlockKey.UnRegister();
            }

        }
        private void btnRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            btnRegister_Click(sender, null);
        }

        private void lblClickClose_Click(object sender, EventArgs e)
        {
            Close();
        }

      

	}

}
