using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebSurge
{
    public partial class EditForm : Form
    {
        string OriginalText { get; set; }
        public string EditorText { get; set; }
        public TextBox TextBoxToUpdate { get; set; }
        
        public EditForm(string text)
        {
            OriginalText = text;            
            InitializeComponent();
        }
        public EditForm(TextBox textBoxToUpdate)
        {
            OriginalText = textBoxToUpdate.Text;
            TextBoxToUpdate = textBoxToUpdate;
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            EditorText = OriginalText;
            txtEditor.Text = OriginalText;
            
            txtEditor.SelectionStart = 0;
            txtEditor.SelectionLength = 0;
        }        

        private void toolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var item = e.ClickedItem;
            if (item == tbSave)
            {
                EditorText = txtEditor.Text;                
                Hide();
            }
            if (item == tbCancel)
            {
                EditorText = OriginalText;
                TextBoxToUpdate.Text = OriginalText;
                Hide();
            }
        }

        private void txtEditor_TextChanged(object sender, EventArgs e)
        {
            EditorText = txtEditor.Text;
            if (TextBoxToUpdate != null)
            {
                TextBoxToUpdate.Text = EditorText;
            }
        }

        private void tbReload_Click(object sender, EventArgs e)
        {
            EditorText = OriginalText;
            txtEditor.Text = OriginalText;
        }
    }
}
