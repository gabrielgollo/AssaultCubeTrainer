using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AssaultCubeTrainer.Rendering
{
    /// <summary>
    /// Provides drawing functionality for overlays on game window
    /// </summary>
    public class Drawing
    {
        private const string user32dll = "User32.dll";
        private static readonly object OverlaySync = new object();
        private static OverlayWindow? _overlay;
        private static Thread? _overlayThread;
        private static readonly ManualResetEvent OverlayReady = new ManualResetEvent(false);
        private static readonly System.Collections.Generic.List<(Rectangle Rect, Color Color)> PendingRects = new();

        private static string NormalizeProcessLookupName(string processName)
        {
            if (string.IsNullOrWhiteSpace(processName))
            {
                return string.Empty;
            }

            return processName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                ? processName[..^4]
                : processName;
        }

        [DllImport(user32dll)]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport(user32dll)]
        private static extern IntPtr ReleaseDC(IntPtr hwnd, IntPtr dc);

        [DllImport(user32dll)]
        private static extern IntPtr GetWindowRect(IntPtr hwnd, out RECT lpRect);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport(user32dll)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport(user32dll)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport(user32dll)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private static Size GetControleSize(IntPtr hwnd)
        {
            RECT pRect;
            Size cSize = new Size();
            GetWindowRect(hwnd, out pRect);
            cSize.Width = pRect.Right - pRect.Left - 6;
            cSize.Height = pRect.Bottom - pRect.Top - 28;
            return cSize;
        }

        /// <summary>
        /// Draw rectangle overlay on game window
        /// </summary>
        public static void DrawRect(IntPtr win, Color color, Rectangle rect)
        {
                if (!IsRectDrawable(rect))
                {
                    return;
                }

                lock (OverlaySync)
                {
                    if (_overlay != null)
                    {
                        PendingRects.Add((rect, color));
                        return;
                    }
                }

                if (win == IntPtr.Zero)
                {
                    return;
                }

                IntPtr windowDc = GetDC(win);
                if (windowDc == IntPtr.Zero)
                {
                    return;
                }

                Graphics g = Graphics.FromHdc(windowDc);
                Pen pen = new Pen(color, 2);
                try
                {
                    g.DrawRectangle(pen, rect);
                }
                catch
                {
                    // Ignore transient GDI failures.
                }
                g.Dispose();

                ReleaseDC(win, windowDc);
        }

        public static void BeginOverlayFrame(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
            {
                return;
            }

            EnsureOverlay();
            Rectangle bounds = GetWindowBounds(windowHandle);
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            lock (OverlaySync)
            {
                PendingRects.Clear();
                _overlay?.UpdateBoundsSafe(bounds);
                _overlay?.EnsureTopMostSafe();
            }
        }

        public static void EndOverlayFrame()
        {
            lock (OverlaySync)
            {
                if (_overlay == null)
                {
                    return;
                }

                _overlay.SetRectsSafe(new System.Collections.Generic.List<(Rectangle Rect, Color Color)>(PendingRects));
            }
        }

        public static void ShutdownOverlay()
        {
            lock (OverlaySync)
            {
                if (_overlay == null)
                {
                    return;
                }

                _overlay.CloseSafe();
                _overlay = null;
                PendingRects.Clear();
            }
        }

        /// <summary>
        /// Get game window size by process name
        /// </summary>
        public static Size GetSize(string nameProcess)
        {
            IntPtr win = GetWindowHandle(nameProcess);
            return win == IntPtr.Zero ? new Size() : GetControleSize(win);
        }

        public static Size GetSize(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
            {
                return new Size();
            }

            return GetControleSize(windowHandle);
        }

        public static IntPtr GetWindowHandle(string processName)
        {
            string lookupName = NormalizeProcessLookupName(processName);
            if (string.IsNullOrWhiteSpace(lookupName))
            {
                return IntPtr.Zero;
            }

            Process process = Process.GetProcessesByName(lookupName).FirstOrDefault();
            if (process == null)
            {
                return IntPtr.Zero;
            }

            process.Refresh();
            IntPtr handle = process.MainWindowHandle;
            if (handle != IntPtr.Zero)
            {
                return handle;
            }

            Process withWindow = Process
                .GetProcessesByName(lookupName)
                .Select(p =>
                {
                    p.Refresh();
                    return p;
                })
                .FirstOrDefault(p => p.MainWindowHandle != IntPtr.Zero);

            return withWindow?.MainWindowHandle ?? IntPtr.Zero;
        }

        public static IntPtr GetWindowHandle(int processId)
        {
            if (processId <= 0)
            {
                return IntPtr.Zero;
            }

            IntPtr found = IntPtr.Zero;
            EnumWindows((hWnd, _) =>
            {
                if (!IsWindowVisible(hWnd))
                {
                    return true;
                }

                GetWindowThreadProcessId(hWnd, out uint pid);
                if (pid == (uint)processId)
                {
                    found = hWnd;
                    return false;
                }

                return true;
            }, IntPtr.Zero);

            return found;
        }

        private static Rectangle GetWindowBounds(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
            {
                return Rectangle.Empty;
            }

            GetWindowRect(hwnd, out RECT rect);
            return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        private static void EnsureOverlay()
        {
            lock (OverlaySync)
            {
                if (_overlay != null)
                {
                    return;
                }

                OverlayReady.Reset();
                _overlayThread = new Thread(() =>
                {
                    _overlay = new OverlayWindow();
                    _overlay.Load += (_, _) => OverlayReady.Set();
                    Application.Run(_overlay);
                })
                {
                    IsBackground = true,
                    Name = "OverlayThread"
                };
                _overlayThread.SetApartmentState(ApartmentState.STA);
                _overlayThread.Start();

                OverlayReady.WaitOne(2000);
            }
        }

        private static bool IsRectDrawable(Rectangle rect)
        {
            if (rect.Width <= 1 || rect.Height <= 1)
            {
                return false;
            }

            const int MaxSize = 10000;
            if (rect.Width > MaxSize || rect.Height > MaxSize)
            {
                return false;
            }

            return true;
        }
    }
}
