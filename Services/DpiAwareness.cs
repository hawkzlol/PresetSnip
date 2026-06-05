using System;
using System.Runtime.InteropServices;

namespace PresetSnip.Services;

public static class DpiAwareness
{
    private static readonly IntPtr PerMonitorAwareV2 = new(-4);

    public static void TryEnablePerMonitorV2()
    {
        try
        {
            SetProcessDpiAwarenessContext(PerMonitorAwareV2);
        }
        catch
        {
            // Best effort only. The app still works with Windows' default WPF DPI mode.
        }
    }

    [DllImport("user32.dll")]
    private static extern bool SetProcessDpiAwarenessContext(IntPtr dpiContext);
}
