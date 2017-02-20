using System;
using System.Windows.Forms;

namespace WebSurge
{
    public class WindowSettings
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Split { get; set; }
        public int HeadersContentSplit { get; set; }

        public int Accesses { get; set; }

        public WindowSettings()
        {
            Left = -1;
            Top = -1;
            Width = 1000;
            Height = 700;
            Split = 490;
            HeadersContentSplit = 155;
        }

        /// <summary>
        /// Loads settings from this structure to a form.
        /// Applies Window settings to a Windows or WPF Form
        /// object - anything with Top, Left, Height, Width
        /// properties really
        /// </summary>
        /// <param name="form"></param>
        public void Load(dynamic form)
        {
            if (Left != -1)
            {
                form.Left = Left;
                form.Top = Top;
            }
            form.Width = Width;
            form.Height = Height;

            try
            {
                form.BottomSplitContainer.SplitterDistance = Split;
                form.HeadersContentSplitter.SplitterDistance = HeadersContentSplit;
            }
            catch 
            {
               
            }
        }

        /// <summary>
        /// Saves settings from a form to this structure.
        /// Works with any form type object that has
        /// Top,Left,Height,Width properties
        /// </summary>
        /// <param name="form"></param>
        public void Save(dynamic form)
        {
            Top = form.Top;
            Left = form.Left;
            Width = form.Width;
            Height = form.Height;

            if (Top < 0)
                Top = 0;
            if (Left < 0)
                Left = 0;

            try
            {
                Split = form.BottomSplitContainer.SplitterDistance;
            }
            catch
            {
            }
        }

    }
}