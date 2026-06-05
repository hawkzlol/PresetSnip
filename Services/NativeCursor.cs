using System.Runtime.InteropServices;
using PresetSnip.Models;

namespace PresetSnip.Services;

public static class NativeCursor
{
    public static ScreenPoint GetPosition()
    {
        if (!GetCursorPos(out var point))
        {
            return new ScreenPoint(0, 0);
        }

        return new ScreenPoint(point.X, point.Y);
    }

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out NativePoint point);

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct NativePoint
    {
        public readonly int X;
        public readonly int Y;
    }
}
