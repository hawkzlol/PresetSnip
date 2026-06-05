using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using PresetSnip.Models;
using PresetSnip.Services;

namespace PresetSnip;

public partial class SelectionOverlayWindow : Window
{
    private const int SWP_NOZORDER = 0x0004;
    private const int SWP_NOACTIVATE = 0x0010;

    private readonly VirtualScreenBounds _bounds = VirtualScreen.GetBounds();
    private ScreenPoint? _firstPoint;

    public SelectionOverlayWindow()
    {
        InitializeComponent();

        Left = _bounds.X;
        Top = _bounds.Y;
        Width = _bounds.Width;
        Height = _bounds.Height;
    }

    public ScreenPoint? PointA { get; private set; }

    public ScreenPoint? PointB { get; private set; }

    private void Window_SourceInitialized(object? sender, EventArgs e)
    {
        var handle = new WindowInteropHelper(this).Handle;
        SetWindowPos(handle, IntPtr.Zero, _bounds.X, _bounds.Y, _bounds.Width, _bounds.Height, SWP_NOZORDER | SWP_NOACTIVATE);
        Activate();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var screenPoint = NativeCursor.GetPosition();

        if (_firstPoint is null)
        {
            _firstPoint = screenPoint;
            PointA = screenPoint;
            ShowMarker(PointAMarker, screenPoint);
            InstructionTitle.Text = "Click point B";
            InstructionText.Text = "Click the opposite corner of the capture area. Press Esc to cancel.";
            return;
        }

        PointB = screenPoint;
        ShowMarker(PointBMarker, screenPoint);
        DialogResult = true;
        Close();
    }

    private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            DialogResult = false;
            Close();
        }
    }

    private void ShowMarker(FrameworkElement marker, ScreenPoint screenPoint)
    {
        var localPoint = PointFromScreen(new Point(screenPoint.X, screenPoint.Y));
        marker.Visibility = Visibility.Visible;
        Canvas.SetLeft(marker, localPoint.X - marker.Width / 2);
        Canvas.SetTop(marker, localPoint.Y - marker.Height / 2);

        if (_firstPoint is not null && marker == PointBMarker)
        {
            var firstLocal = PointFromScreen(new Point(_firstPoint.Value.X, _firstPoint.Value.Y));
            SelectionRectangle.Visibility = Visibility.Visible;
            Canvas.SetLeft(SelectionRectangle, Math.Min(firstLocal.X, localPoint.X));
            Canvas.SetTop(SelectionRectangle, Math.Min(firstLocal.Y, localPoint.Y));
            SelectionRectangle.Width = Math.Abs(localPoint.X - firstLocal.X);
            SelectionRectangle.Height = Math.Abs(localPoint.Y - firstLocal.Y);
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int width, int height, uint flags);
}
