using System;
using System.Runtime.InteropServices;

namespace VisionClient.Helpers
{
    public static class FullScreenHelper
    {
        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("user32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        public static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam, int minWidth, int minHeight)
        {
            var mmi_min = Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            var mmi = mmi_min is not null ? (MINMAXINFO)mmi_min : new MINMAXINFO();

            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = FullScreenHelper.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {

                FullScreenHelper.MONITORINFO monitorInfo = new FullScreenHelper.MONITORINFO();
                FullScreenHelper.GetMonitorInfo(monitor, monitorInfo);
                FullScreenHelper.RECT rcWorkArea = monitorInfo.rcWork;
                FullScreenHelper.RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
                mmi.ptMinTrackSize.x = minWidth;
                mmi.ptMinTrackSize.y = minHeight;
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new();
            public RECT rcWork = new();
            public int dwFlags = 0;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public static readonly RECT Empty = new();

            public int Width
            {
                get { return Math.Abs(right - left); }
            }

            public int Height
            {
                get { return bottom - top; }
            }

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }


            public RECT(RECT rcSrc)
            {
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            public bool IsEmpty
            {
                get
                {
                    return left >= right || top >= bottom;
                }
            }

            public override string ToString()
            {
                if (this == RECT.Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            public override bool Equals(object? obj)
            {
                if (obj is not RECT) { return false; }
                return (this == (RECT)obj);
            }


            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }



            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }


            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }
        }
    }
}
