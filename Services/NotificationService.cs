using System;
using System.IO;
using PresetSnip;
using Forms = System.Windows.Forms;
using DrawingIcon = System.Drawing.Icon;
using SystemIcons = System.Drawing.SystemIcons;

namespace PresetSnip.Services;

public sealed class NotificationService : IDisposable
{
    private readonly Forms.NotifyIcon _notifyIcon;
    private LatestSnipToastWindow? _toastWindow;

    public NotificationService()
    {
        _notifyIcon = new Forms.NotifyIcon
        {
            Icon = LoadIcon(),
            Text = "PresetSnip",
            Visible = false
        };
    }

    public void SetEnabled(bool enabled, string? latestPath)
    {
        _notifyIcon.Visible = enabled;
        UpdateLatestPath(latestPath);

        if (!enabled)
        {
            _toastWindow?.Stop();
        }
    }

    public void ShowLatestSnip(string path, bool enabled)
    {
        SetEnabled(enabled, path);
        if (!enabled)
        {
            return;
        }

        _toastWindow ??= new LatestSnipToastWindow();
        _toastWindow.ShowLatest(path);
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _toastWindow?.Close();
    }

    private void UpdateLatestPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            _notifyIcon.Text = "PresetSnip";
            return;
        }

        var text = $"Latest snip: {Path.GetFileName(path)}";
        _notifyIcon.Text = text.Length <= 63 ? text : text[..63];
    }

    private static DrawingIcon LoadIcon()
    {
        try
        {
            var processPath = Environment.ProcessPath;
            if (!string.IsNullOrWhiteSpace(processPath))
            {
                return DrawingIcon.ExtractAssociatedIcon(processPath) ?? SystemIcons.Information;
            }
        }
        catch
        {
            // Cosmetic only. Startup should never depend on icon extraction.
        }

        return SystemIcons.Information;
    }
}
