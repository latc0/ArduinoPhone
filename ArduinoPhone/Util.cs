using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

public static class Util
{
    public enum Effect { Roll, Slide, Center, Blend }

    public static void Animate(Control ctl, Effect effect, int msec, int angle)
    {
        int flags = effmap[(int)effect];
        if (ctl.Visible) { flags |= 0x10000; angle += 180; }
        else
        {
            if (ctl.TopLevelControl == ctl) flags |= 0x20000;
            else if (effect == Effect.Blend) throw new ArgumentException();
        }
        flags |= dirmap[(angle % 360) / 45];
        bool ok = AnimateWindow(ctl.Handle, msec, flags);
        if (!ok) throw new Exception("Animation failed");
        ctl.Visible = !ctl.Visible;
    }

    private static int[] dirmap = { 1, 5, 4, 6, 2, 10, 8, 9 };
    private static int[] effmap = { 0, 0x40000, 0x10, 0x80000 };

    [DllImport("user32.dll")]
    private static extern bool AnimateWindow(IntPtr handle, int msec, int flags);

    // To support flashing.
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

    //Flash both the window caption and taskbar button.
    //This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags. 
    private const UInt32 FLASHW_ALL = 3;

    // Flash continuously until the window comes to the foreground. 
    private const UInt32 FLASHW_TIMERNOFG = 12;

    [StructLayout(LayoutKind.Sequential)]
    private struct FLASHWINFO
    {
        public UInt32 cbSize;
        public IntPtr hwnd;
        public UInt32 dwFlags;
        public UInt32 uCount;
        public UInt32 dwTimeout;
    }

    // Do the flashing - this does not involve a raincoat.
    public static bool FlashWindowEx(Form form)
    {
        IntPtr hWnd = form.Handle;
        FLASHWINFO fInfo = new FLASHWINFO();

        fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
        fInfo.hwnd = hWnd;
        fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
        fInfo.uCount = UInt32.MaxValue;
        fInfo.dwTimeout = 0;

        return FlashWindowEx(ref fInfo);
    }
}