using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using WebSurge;

namespace WebSurge
{
	/// <summary>
	/// Summary description for UnlockForm.
	/// </summary>
	class UnlockKeyForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblRegisterText;
        private System.Windows.Forms.Label lblNote;
		private System.Windows.Forms.Button btnRegister;
        
		private System.Windows.Forms.TextBox txtKey;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private PictureBox pictureBox1;

		public string SoftwareName = "WebSurge";

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnlockKeyForm));
            this.lblRegisterText = new System.Windows.Forms.Label();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.btnRegister = new System.Windows.Forms.Button();
            this.lblNote = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblRegisterText
            // 
            this.lblRegisterText.Location = new System.Drawing.Point(83, 15);
            this.lblRegisterText.Name = "lblRegisterText";
            this.lblRegisterText.Size = new System.Drawing.Size(245, 15);
            this.lblRegisterText.TabIndex = 0;
            this.lblRegisterText.Text = "Please enter the key to unlock ";
            // 
            // txtKey
            // 
            this.txtKey.Location = new System.Drawing.Point(84, 33);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(239, 21);
            this.txtKey.TabIndex = 1;
            // 
            // btnRegister
            // 
            this.btnRegister.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRegister.Location = new System.Drawing.Point(248, 63);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(75, 23);
            this.btnRegister.TabIndex = 2;
            this.btnRegister.Text = "&Register";
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // lblNote
            // 
            this.lblNote.AutoSize = true;
            this.lblNote.ForeColor = System.Drawing.Color.Red;
            this.lblNote.Location = new System.Drawing.Point(84, 60);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(57, 13);
            this.lblNote.TabIndex = 3;
            this.lblNote.Text = "RegStatus";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(13, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(58, 72);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // UnlockKeyForm
            // 
            this.AcceptButton = this.btnRegister;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.ClientSize = new System.Drawing.Size(334, 96);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.txtKey);
            this.Controls.Add(this.lblRegisterText);
            this.Controls.Add(this.lblNote);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "UnlockKeyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WebSurge Registration";
            this.Load += new System.EventHandler(this.FormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

		public UnlockKeyForm(string ProductName)
		{
			this.SoftwareName = ProductName;
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
            this.lblRegisterText.Text = "Please enter the key to unlock " + this.SoftwareName;
            

            if (UnlockKey.IsRegistered())
                this.lblNote.Text = "This copy is registered.";
            else
                this.lblNote.Text = "This copy is not registered.";
        }

		private void btnRegister_Click(object sender, System.EventArgs e)
		{
            if (UnlockKey.Register(this.txtKey.Text))
            {
                MessageBox.Show("Thank you for your registration.", this.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Sorry, that's not the right key.\r\nMake sure you entered the value exactly as it\r\n" +
                    "appears in your confirmation.\r\n\r\n", "Software Registration",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.lblNote.Text = "This copy is not registered.";
            }

        }

	}

}
