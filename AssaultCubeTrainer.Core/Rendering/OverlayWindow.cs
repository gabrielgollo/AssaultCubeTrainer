using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AssaultCubeTrainer.Rendering
{
    internal sealed class OverlayWindow : Form
    {
        private const int WsExTransparent = 0x20;
        private const int WsExToolWindow = 0x80;
        private const int WsExNoActivate = 0x08000000;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        private readonly object _sync = new object();
        private List<(Rectangle Rect, Color Color)> _rects = new();

        public OverlayWindow()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            DoubleBuffered = true;
            BackColor = Color.Black;
            TransparencyKey = Color.Black;
            StartPosition = FormStartPosition.Manual;
        }

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WsExTransparent | WsExToolWindow | WsExNoActivate;
                return cp;
            }
        }

        public void UpdateBoundsSafe(Rectangle bounds)
        {
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action<Rectangle>(UpdateBoundsSafe), bounds);
                return;
            }

            if (Bounds != bounds)
            {
                Bounds = bounds;
                EnsureTopMostCore();
            }
        }

        public void EnsureTopMostSafe()
        {
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action(EnsureTopMostSafe));
                return;
            }

            EnsureTopMostCore();
        }

        private void EnsureTopMostCore()
        {
            TopMost = true;
            SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
        }

        public void SetRectsSafe(List<(Rectangle Rect, Color Color)> rects)
        {
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action<List<(Rectangle Rect, Color Color)>>(SetRectsSafe), rects);
                return;
            }

            lock (_sync)
            {
                _rects = rects;
            }

            Invalidate();
        }

        public void CloseSafe()
        {
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action(CloseSafe));
                return;
            }

            Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                List<(Rectangle Rect, Color Color)> copy;
                lock (_sync)
                {
                    copy = new List<(Rectangle Rect, Color Color)>(_rects);
                }

                foreach ((Rectangle rect, Color color) in copy)
                {
                    if (rect.Width <= 0 || rect.Height <= 0)
                    {
                        continue;
                    }

                    using Pen pen = new Pen(color, 2);
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }
            catch
            {
                // Never let overlay paint exceptions crash the UI thread.
            }
        }
    }
}
