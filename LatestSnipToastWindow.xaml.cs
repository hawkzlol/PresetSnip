using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Forms = System.Windows.Forms;

namespace PresetSnip;

public partial class LatestSnipToastWindow : Window
{
    private readonly DispatcherTimer _hideTimer = new()
    {
        Interval = TimeSpan.FromSeconds(3)
    };

    public LatestSnipToastWindow()
    {
        InitializeComponent();
        _hideTimer.Tick += HideTimer_Tick;
    }

    public void ShowLatest(string path)
    {
        FileNameText.Text = Path.GetFileName(path);
        FolderText.Text = Path.GetDirectoryName(path) ?? string.Empty;

        PositionNearTaskbar();

        if (!IsVisible)
        {
            Show();
        }

        Topmost = false;
        Topmost = true;
        _hideTimer.Stop();
        _hideTimer.Start();
    }

    public void Stop()
    {
        _hideTimer.Stop();
        Hide();
    }

    private void HideTimer_Tick(object? sender, EventArgs e)
    {
        Stop();
    }

    private void PositionNearTaskbar()
    {
        var screen = Forms.Screen.FromPoint(Forms.Cursor.Position);
        var area = screen.WorkingArea;

        Left = area.Right - Width - 18;
        Top = area.Bottom - Height - 18;
    }
}
