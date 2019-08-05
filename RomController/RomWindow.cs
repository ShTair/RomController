using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RomController
{
    public class RomWindow
    {
        public Rectangle Rectangle { get; }

        public RomWindow(Rectangle rectangle)
        {
            Rectangle = rectangle;
        }

        public Point SellCountBox => GetGamePoint(0.385, 0.48);

        public Point TextOkButton => GetGamePoint(0.925, 0.9);

        public Point SellButton => GetGamePoint(0.415, 0.83);

        private Point GetGamePoint(double px, double py)
        {
            return new Point((int)(Rectangle.X + Rectangle.Width * px), (int)(Rectangle.Y + Rectangle.Height * py));
        }

        #region Find

        public static async Task<RomWindow> FromPointAsync(Point point)
        {
            var rect = await GetWindowRectAsync(point, new CancellationTokenSource(2000).Token);
            return new RomWindow(rect);
        }

        private static async Task<Rectangle> GetWindowRectAsync(Point point, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<Rectangle>();
            cancellationToken.Register(() => tcs.TrySetCanceled());

            EnumWindowsProc onEnumWindows = (IntPtr hWnd, IntPtr lParam) =>
            {
                var csb = new StringBuilder(256);
                GetClassName(hWnd, csb, csb.Capacity);

                var cs = csb.ToString();
                if (cs.IndexOf("BlueStacks.exe") == -1) return true;

                RECT rect;
                GetWindowRect(hWnd, out rect);

                if (rect.Left > point.X || rect.Right < point.X || rect.Top > point.Y || rect.Bottom < point.Y) return true;

                tcs.TrySetResult(new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top));
                return false;
            };

            EnumWindows(onEnumWindows, IntPtr.Zero);
            return await tcs.Task;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        #endregion
    }
}
