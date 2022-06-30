using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VisionClient.Utility
{
    internal static class RunningInstance
    {
        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        public static bool CheckInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);

            if (processes.Length > 1)
            {
                ShowWindow(processes[0].MainWindowHandle, 1);
                Process.GetCurrentProcess().Kill();
                return true;
            }

            return false;
        }
    }
}
