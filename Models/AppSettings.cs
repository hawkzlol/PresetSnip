using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace PresetSnip.Models;

public sealed class AppSettings
{
    public string SaveDirectory { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
        "PresetSnip");

    public HotKeyGesture HotKey { get; set; } = HotKeyGesture.Default;

    public ScreenPoint? PointA { get; set; }

    public ScreenPoint? PointB { get; set; }

    public string? LastSavedPath { get; set; }

    public bool SoundEnabled { get; set; } = true;

    public double SoundVolume { get; set; } = 0.65;

    public bool NotificationsEnabled { get; set; } = true;
}

public sealed class HotKeyGesture
{
    public static HotKeyGesture Default => new() { Key = Key.Delete, Modifiers = ModifierKeys.None };

    public Key Key { get; set; } = Key.Delete;

    public ModifierKeys Modifiers { get; set; } = ModifierKeys.None;

    public string DisplayText
    {
        get
        {
            var parts = new List<string>();

            if (Modifiers.HasFlag(ModifierKeys.Control))
            {
                parts.Add("CTRL");
            }

            if (Modifiers.HasFlag(ModifierKeys.Shift))
            {
                parts.Add("SHIFT");
            }

            if (Modifiers.HasFlag(ModifierKeys.Alt))
            {
                parts.Add("ALT");
            }

            if (Modifiers.HasFlag(ModifierKeys.Windows))
            {
                parts.Add("WIN");
            }

            parts.Add(Key == Key.Delete ? "DEL" : Key.ToString().ToUpperInvariant());
            return string.Join(" + ", parts);
        }
    }
}

public readonly record struct ScreenPoint(int X, int Y);

public readonly record struct CaptureRegion(int X, int Y, int Width, int Height)
{
    public bool IsValid => Width > 0 && Height > 0;

    public static CaptureRegion FromPoints(ScreenPoint first, ScreenPoint second)
    {
        var x = Math.Min(first.X, second.X);
        var y = Math.Min(first.Y, second.Y);
        var width = Math.Abs(second.X - first.X);
        var height = Math.Abs(second.Y - first.Y);

        return new CaptureRegion(x, y, width, height);
    }
}
