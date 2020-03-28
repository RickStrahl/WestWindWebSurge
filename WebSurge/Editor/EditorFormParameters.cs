using System;
using System.Windows.Forms;

namespace WebSurge.Editor
{
    public class EditorFormParameters
    {
        /// <summary>
        /// The content to edit in the editor. Optionally you can pass a TextBox
        /// instead of this value in which case the .Text property will be used for
        /// the input content.
        ///
        /// This property if set will cause the Textbox to not be updated.
        /// </summary>
        public string Content {get; set; }
            
        /// <summary>
        /// An optional Textbox to pass in. If passed **and** the `Content` property
        /// is not set, the textbox is updated when the editor loses focus.
        /// </summary>
        public TextBox TextBoxToUpdate { get; set; }

        /// <summary>
        /// The syntax to use in the editor. json,xml,http,html,css,js,ts etc.
        /// </summary>
        public string Syntax { get; set; } = "text";


        /// <summary>
        /// An optional event handler you can hook up to be notified when the text is updated
        /// which happens when the editor loses focus.
        /// </summary>
        public Action<string> EditorTextUpdatedAction {get; set;}
    }
}