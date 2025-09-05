using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
namespace WpfSimpleViewManager.Extensions
{
    internal static class MonitorUtil
    {
        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int X; public int Y; }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        /// <summary>
        /// 전역 커서 좌표와 해당 모니터의 워킹영역을 WPF 단위(DIU)로 반환
        /// </summary>
        public static (Point cursorWpf, Rect workAreaWpf) GetCursorAndWorkAreaWpf(Visual relativeTo)
        {
            if (!GetCursorPos(out var pt))
                return (new Point(0, 0), new Rect(0, 0, SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight));

            var hMon = MonitorFromPoint(pt, MONITOR_DEFAULTTONEAREST);
            var mi = new MONITORINFO { cbSize = Marshal.SizeOf<MONITORINFO>() };
            if (!GetMonitorInfo(hMon, ref mi))
                mi = new MONITORINFO
                {
                    rcWork = new RECT
                    {
                        Left = 0,
                        Top = 0,
                        Right = (int)SystemParameters.PrimaryScreenWidth,
                        Bottom = (int)SystemParameters.PrimaryScreenHeight
                    }
                };

            // px -> WPF(DIU) 변환 (고DPI 대응)
            var dpi = VisualTreeHelper.GetDpi(relativeTo);
            double PxToWpfX(int x) => x / dpi.DpiScaleX;
            double PxToWpfY(int y) => y / dpi.DpiScaleY;

            var cursor = new Point(PxToWpfX(pt.X), PxToWpfY(pt.Y));
            var wa = new Rect(
                PxToWpfX(mi.rcWork.Left),
                PxToWpfY(mi.rcWork.Top),
                PxToWpfX(mi.rcWork.Right - mi.rcWork.Left),
                PxToWpfY(mi.rcWork.Bottom - mi.rcWork.Top)
            );

            return (cursor, wa);
        }
    }

}
