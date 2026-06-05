using System.Runtime.InteropServices;

namespace PresetSnip.Services;

public readonly record struct VirtualScreenBounds(int X, int Y, int Width, int Height);

public static class VirtualScreen
{
    private const int SM_XVIRTUALSCREEN = 76;
    private const int SM_YVIRTUALSCREEN = 77;
    private const int SM_CXVIRTUALSCREEN = 78;
    private const int SM_CYVIRTUALSCREEN = 79;

    public static VirtualScreenBounds GetBounds()
    {
        return new VirtualScreenBounds(
            GetSystemMetrics(SM_XVIRTUALSCREEN),
            GetSystemMetrics(SM_YVIRTUALSCREEN),
            GetSystemMetrics(SM_CXVIRTUALSCREEN),
            GetSystemMetrics(SM_CYVIRTUALSCREEN));
    }

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);
}
