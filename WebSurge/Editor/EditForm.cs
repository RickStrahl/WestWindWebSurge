using System;
using System.Windows.Forms;

namespace WebSurge.Editor
{

    /// <summary>
    /// A code Editor control that lets you edit code with a numer of different syntaxes
    /// </summary>
    public partial class EditForm : Form
    {
        /// <summary>
        /// The original text that was passed into the form
        /// </summary>
        public string OriginalText { get; }
        
        /// <summary>
        /// The result text when the form is closed or whenever the editor loses focus
        /// </summary>
        public string EditorText { get; set; }

        /// <summary>
        /// Determines whether the user saved the actual data entry
        /// </summary>
        public bool Cancelled { get; set; } = false;

        EditorFormParameters Parameters {get; }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textBoxToUpdate">A textbox to update the value if passed. If `content` is passed this property is not updated, otherwise the Text is updated.</param>
        /// <param name="syntax">Optional syntax to display for the the editor: xml,json,html,http,js,ts,css etc.</param>
        /// <param name="content">Optionally pass in the content. If content is passed textBoxToUpdate is **not** updated and only `EditorText` is set on return.</param>
        public EditForm(EditorFormParameters parms)
        {
            Parameters = parms;
            
            
            if(!string.IsNullOrEmpty(parms.Content))
                OriginalText = parms.Content;
            else if (parms.TextBoxToUpdate != null)
                OriginalText = parms.TextBoxToUpdate.Text;

            InitializeComponent();
            
            WebBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
            
            Load += EditForm_Load;
        }

        private void EditForm_Load(object sender, EventArgs e)
        {
            var url = Environment.ExpandEnvironmentVariables(@"%appdata%\West Wind WebSurge\html\editor.htm");
            WebBrowser.Url = new Uri(url);

        }

        private AceEditorInterop AceEditorInterop;

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var url = e.Url;
            if (url.ToString().Contains("editor.htm"))
            {
                AceEditorInterop = new AceEditorInterop(WebBrowser);
                AceEditorInterop.TextUpdated = (text) =>
                {
                    // if an action was passed call it
                    Parameters.EditorTextUpdatedAction?.Invoke(text);

                    // always update the editor text
                    EditorText = text;

                    // if a textbox was passed but no content
                    if (Parameters.Content == null && Parameters.TextBoxToUpdate != null)
                        Parameters.TextBoxToUpdate.Text = text;
                };


                var theme = Program.WebSurgeForm.StressTester?.Options?.FormattedPreviewTheme;
                if (!string.IsNullOrEmpty(theme))
                    AceEditorInterop.SetTheme(theme);

                AceEditorInterop.SetSyntax(Parameters.Syntax);
                AceEditorInterop.SetValue(OriginalText);
                
                // set cursor position
                if (Parameters.TextBoxToUpdate != null)
                    AceEditorInterop.Invoke("setselposition", 
                        Parameters.TextBoxToUpdate.SelectionStart,
                        Parameters.TextBoxToUpdate.SelectionLength);

                AceEditorInterop.SetFocus();

            }
        }
        


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            EditorText = OriginalText;
        }        

        private void toolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var item = e.ClickedItem;
            ButtonHandler(item);
            
        }

        private void ButtonHandler(ToolStripItem item)
        {
            if (item == tbSave)
            {
                EditorText = AceEditorInterop.GetValue();
                if(Parameters.TextBoxToUpdate != null)
                    Parameters.TextBoxToUpdate.Text = EditorText;
                Hide();
                Cancelled = false;
            }
            if (item == tbCancel)
            {
                EditorText = OriginalText;
                if (Parameters.TextBoxToUpdate != null)
                    Parameters.TextBoxToUpdate.Text = OriginalText;
                Hide();
                Cancelled = true;
            }
        }


        private void tbReload_Click(object sender, EventArgs e)
        {
            EditorText = OriginalText;
            AceEditorInterop.SetSyntax(Parameters.Syntax);
            AceEditorInterop.SetValue(OriginalText);
            AceEditorInterop.SetFocus();
        }


        private void EditForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Control)
            {
                ButtonHandler(tbSave);
            }

            if (e.KeyCode == Keys.Escape)
            {
                ButtonHandler(tbCancel);
            }
        }
    }
}
