using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Westwind.Utilities;

namespace WebSurge.Editor

{

    /// <summary>
    /// Interop class that interacts with the JavaScript via COM Interop
    /// </summary>
    [ComVisible(true)]
    public class AceEditorInterop
    {
        public object EditorInstance;
        public Type EditorInteropType;

        /// <summary>
        /// Notifies you when text is updated:
        /// `TextUpdated = text => this.editorText = text;`
        /// </summary>
        public Action<string> TextUpdated {get; set;}


        public AceEditorInterop(WebBrowser webBrowser)
        {
            EditorInstance = InitializeInterop(webBrowser);
            EditorInteropType = EditorInstance.GetType();
        }

        #region Call Into Editor

        public void SetSyntax(string syntax)
        {
            Invoke("setlanguage", syntax);
        }

        public string GetValue()
        {
            return Invoke("getvalue",false) as string;
        }

        public void SetValue(string text)
        {
            Invoke("setvalue", text);
        }

      

        protected void OnTextUpdated(string text)
        {
            TextUpdated?.Invoke(text);
        }
        #endregion


        #region Callbacks from Editor
        
        /// <summary>
        /// ACE Editor Notification when focus is lost
        /// </summary>
        public void LostFocus()
        {
            TextUpdated?.Invoke(GetValue());
        }

        /// <summary>
        /// ACE Editor Notification when focus is set to the editor
        /// </summary>
        public void GotFocus()
        {

        }

        /// <summary>
        /// Sets the Markdown Document as having changes
        /// </summary>
        /// <param name="value">ignored</param>
        public bool SetDirty(bool value)
        {
            TextUpdated?.Invoke(GetValue());
            return false;
        }


        /// <summary>
        /// Determines whether current document is dirty and requires saving
        /// </summary>
        /// <returns></returns>
        public bool IsDirty(bool previewIfDirty = false)
        {
            return false;
        }

        public void KeyboardCommand(string key, string action = null)
        {
        }

        public void UpdateDocumentStats(object stats)
        {

        }

        public void CopyOperation()
        {
            var text = GetSelection();
            text =StringUtils.NormalizeLineFeeds(text, LineFeedTypes.CrLf);
            Clipboard.SetText(text);
        }


        public void PasteOperation()
        {
        }

        
        /// <summary>
        /// Sets focus to the editor
        /// </summary>
        public void SetFocus()
        {
            Invoke("setfocus", false);
        }

        /// <summary>
        /// Gets the current selection of the editor
        /// </summary>
        /// <returns></returns>
        public string GetSelection()
        {
            return Invoke("getselection", false) as string;
        }

        
        public void SetSelection(string text)
        {
            Invoke("setselection", text);
        }

        
        public void SetTheme(string theme)
        {
            Invoke("setTheme", theme);
        }
        #endregion


        #region initialization and Interop Helpers

        public object InitializeInterop(WebBrowser webBrowser)
        {
            try
            {
                var jsEditor = webBrowser.Document.InvokeScript("initializeinteropsimple", new object[] {this});
                return jsEditor;
            }
            catch 
            {

            }

            return null;
        }
        

        private const BindingFlags flags =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static | BindingFlags.Instance |
            BindingFlags.IgnoreCase;

        /// <summary>
        /// Invokes a method on the editor by name with parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object Invoke(string method, params object[] parameters)
        {
            // Instance methods have to have a parameter to be found (arguments array)
            if (parameters == null)
                parameters = new object[] {false};
#if DEBUG
            try
            {
#endif
                return EditorInteropType.InvokeMember(method, flags | BindingFlags.InvokeMethod, null, EditorInstance, parameters );
#if DEBUG
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
                throw ex;
            }
#endif
            //return ReflectionUtils.CallMethod(Instance, method, parameters);
        }

        #endregion

    }
}