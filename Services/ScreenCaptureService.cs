using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using PresetSnip.Models;

namespace PresetSnip.Services;

public sealed class ScreenCaptureService
{
    public string CaptureToFile(CaptureRegion region, string saveDirectory)
    {
        if (!region.IsValid)
        {
            throw new InvalidOperationException("Pick two different points before capturing.");
        }

        if (string.IsNullOrWhiteSpace(saveDirectory))
        {
            throw new InvalidOperationException("Set a save directory before capturing.");
        }

        Directory.CreateDirectory(saveDirectory);

        var fileName = $"PresetSnip_{DateTime.Now:yyyy-MM-dd_HHmmss}.png";
        var path = Path.Combine(saveDirectory, fileName);

        using var bitmap = new Bitmap(region.Width, region.Height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(region.X, region.Y, 0, 0, new Size(region.Width, region.Height), CopyPixelOperation.SourceCopy);
        bitmap.Save(path, ImageFormat.Png);

        return path;
    }
}
