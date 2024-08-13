using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCubeTrainer
{
    class Drawing
    {
        private const string user32dll = "User32.dll";

        [DllImport(user32dll)]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport(user32dll)]
        private static extern IntPtr ReleaseDC(IntPtr hwnd, IntPtr dc);

        [DllImport(user32dll)]
        private static extern IntPtr GetWindowRect(IntPtr hwnd, out RECT lpRect);
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
            cSize.Width = pRect.Right - pRect.Left-6;
            cSize.Height = pRect.Bottom - pRect.Top-28;
            return cSize;
        }

        public static void DrawRect(IntPtr win, Color color, Rectangle rect)
        {
                IntPtr windowDc = GetDC(win);
                Graphics g = Graphics.FromHdc(windowDc);
                Pen pen = new Pen(color, 2);
                g.DrawRectangle(pen, rect);
                g.Dispose();

                ReleaseDC(win, windowDc);
        }

        public static Size GetSize(string nameProcess)
        {
            Process process = Process.GetProcessesByName(nameProcess).FirstOrDefault();

            if (process == null){
                Console.WriteLine("Process not open!");
                return new Size();
            }

            IntPtr win = process.MainWindowHandle;
            Size size = GetControleSize(win);
            return size;
        }
    }
}
