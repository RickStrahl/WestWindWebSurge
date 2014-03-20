using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kuhela
{
    public class TabPadding : NativeWindow
    {
        private const int WM_PAINT = 0xF;

        private TabControl tabControl;

        public TabPadding(TabControl tc)
        {
            tabControl = tc;
            tabControl.Selected += new TabControlEventHandler(tabControl_Selected);
            AssignHandle(tc.Handle);
        }

        void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            tabControl.Invalidate();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT)
            {
                using (Graphics g = Graphics.FromHwnd(m.HWnd))
                {

                    //Replace the outside white borders:
                    if (tabControl.Parent != null)
                    {
                        g.SetClip(new Rectangle(0, 0, tabControl.Width - 2, tabControl.Height - 1), CombineMode.Exclude);
                        using (SolidBrush sb = new SolidBrush(tabControl.Parent.BackColor))
                            g.FillRectangle(sb, new Rectangle(0,
                                                              tabControl.ItemSize.Height + 2,
                                                              tabControl.Width,
                                                              tabControl.Height - (tabControl.ItemSize.Height + 2)));
                    }

                    //Replace the inside white borders:
                    if (tabControl.SelectedTab != null)
                    {
                        g.ResetClip();
                        Rectangle r = tabControl.SelectedTab.Bounds;
                        g.SetClip(r, CombineMode.Exclude);
                        using (SolidBrush sb = new SolidBrush(tabControl.SelectedTab.BackColor))
                            g.FillRectangle(sb, new Rectangle(r.Left - 3,
                                                              r.Top - 1,
                                                              r.Width + 4,
                                                              r.Height + 3));
                    }
                }
            }
        }
    }
}
