using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PresetSnip.Models;
using PresetSnip.Services;
using Forms = System.Windows.Forms;
using DrawingIcon = System.Drawing.Icon;
using MediaBrush = System.Windows.Media.Brush;

namespace PresetSnip;

public partial class MainWindow : Window
{
    private readonly SettingsStore _settingsStore = new();
    private readonly ScreenCaptureService _captureService = new();
    private readonly MediaBrush _okBrush = new SolidColorBrush(Color.FromRgb(22, 101, 52));
    private readonly MediaBrush _warnBrush = new SolidColorBrush(Color.FromRgb(180, 83, 9));
    private readonly MediaBrush _errorBrush = new SolidColorBrush(Color.FromRgb(185, 28, 28));

    private AppSettings _settings;
    private GlobalHotKey? _globalHotKey;
    private bool _isRecordingHotKey;
    private bool _isLoading;

    public MainWindow()
    {
        InitializeComponent();
        TryUseExecutableIcon();

        _settings = _settingsStore.Load();
        _isLoading = true;
        DirectoryTextBox.Text = _settings.SaveDirectory;
        _isLoading = false;

        LastSavedText.Text = string.IsNullOrWhiteSpace(_settings.LastSavedPath)
            ? "No captures saved yet."
            : _settings.LastSavedPath;
        UpdateUi("Ready.");
    }

    private void Window_SourceInitialized(object? sender, EventArgs e)
    {
        var source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
        if (source is null)
        {
            SetStatus("Could not initialize global hotkey support.", _errorBrush);
            return;
        }

        _globalHotKey = new GlobalHotKey(source);
        _globalHotKey.Pressed += GlobalHotKey_Pressed;
        RegisterHotKey();
    }

    private void Window_Closing(object? sender, CancelEventArgs e)
    {
        _globalHotKey?.Dispose();
        _settingsStore.Save(_settings);
    }

    private void DirectoryTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (_isLoading)
        {
            return;
        }

        _settings.SaveDirectory = DirectoryTextBox.Text.Trim();
        _settingsStore.Save(_settings);
        UpdateUi();
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        using var dialog = new Forms.FolderBrowserDialog
        {
            Description = "Choose where PresetSnip should save screenshots",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = true,
            SelectedPath = DirectoryTextBox.Text
        };

        if (dialog.ShowDialog() == Forms.DialogResult.OK)
        {
            DirectoryTextBox.Text = dialog.SelectedPath;
        }
    }

    private void RecordButton_Click(object sender, RoutedEventArgs e)
    {
        _isRecordingHotKey = true;
        RecordButton.Content = "Press keys...";
        SetStatus("Recording hotkey. Press the key combination to use, or Esc to cancel.", _warnBrush);
        Focus();
    }

    private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (!_isRecordingHotKey)
        {
            return;
        }

        e.Handled = true;

        var key = e.Key == Key.System ? e.SystemKey : e.Key;
        if (key == Key.Escape)
        {
            StopRecordingHotKey();
            UpdateUi("Hotkey recording cancelled.");
            return;
        }

        if (IsModifierKey(key) || key == Key.None)
        {
            return;
        }

        _settings.HotKey = new HotKeyGesture
        {
            Key = key,
            Modifiers = Keyboard.Modifiers
        };

        _settingsStore.Save(_settings);
        StopRecordingHotKey();
        RegisterHotKey();
        UpdateUi($"Hotkey set to {_settings.HotKey.DisplayText}.");
    }

    private async void PickPointsButton_Click(object sender, RoutedEventArgs e)
    {
        PickPointsButton.IsEnabled = false;
        CaptureNowButton.IsEnabled = false;

        try
        {
            for (var seconds = 3; seconds >= 1; seconds--)
            {
                CountdownText.Text = seconds.ToString();
                SetStatus($"Point picker opens in {seconds}...", _warnBrush);
                await Task.Delay(1000);
            }

            CountdownText.Text = string.Empty;

            var overlay = new SelectionOverlayWindow { Owner = this };
            var accepted = overlay.ShowDialog() == true;

            if (!accepted || overlay.PointA is null || overlay.PointB is null)
            {
                UpdateUi("Point picking cancelled.");
                return;
            }

            var region = CaptureRegion.FromPoints(overlay.PointA.Value, overlay.PointB.Value);
            if (!region.IsValid)
            {
                UpdateUi("Point picking failed: choose two different points.", _errorBrush);
                return;
            }

            _settings.PointA = overlay.PointA;
            _settings.PointB = overlay.PointB;
            _settingsStore.Save(_settings);
            UpdateUi("Capture area saved.");
        }
        finally
        {
            CountdownText.Text = string.Empty;
            PickPointsButton.IsEnabled = true;
            UpdateUi();
        }
    }

    private void CaptureNowButton_Click(object sender, RoutedEventArgs e)
    {
        CapturePresetRegion();
    }

    private void GlobalHotKey_Pressed(object? sender, EventArgs e)
    {
        CapturePresetRegion();
    }

    private void CapturePresetRegion()
    {
        var region = GetRegion();
        if (region is null || !region.Value.IsValid)
        {
            UpdateUi("Pick two points before capturing.", _errorBrush);
            return;
        }

        if (!TryGetSaveDirectory(out var saveDirectory, out var directoryError))
        {
            UpdateUi(directoryError, _errorBrush);
            return;
        }

        try
        {
            var path = _captureService.CaptureToFile(region.Value, saveDirectory);
            _settings.LastSavedPath = path;
            _settingsStore.Save(_settings);
            LastSavedText.Text = path;
            UpdateUi($"Saved {Path.GetFileName(path)}.", _okBrush);
        }
        catch (Exception ex)
        {
            UpdateUi($"Capture failed: {ex.Message}", _errorBrush);
        }
    }

    private void RegisterHotKey()
    {
        if (_globalHotKey is null)
        {
            return;
        }

        try
        {
            _globalHotKey.Register(_settings.HotKey);
            UpdateUi($"Global hotkey active: {_settings.HotKey.DisplayText}.", _okBrush);
        }
        catch (Exception ex)
        {
            SetStatus(ex.Message, _errorBrush);
        }
    }

    private void StopRecordingHotKey()
    {
        _isRecordingHotKey = false;
        RecordButton.Content = "Start Record";
    }

    private void UpdateUi(string? status = null, MediaBrush? statusBrush = null)
    {
        HotKeyText.Text = _settings.HotKey.DisplayText;

        var region = GetRegion();
        if (region is { IsValid: true })
        {
            RegionText.Text = $"{region.Value.Width} x {region.Value.Height} pixels";
            PointText.Text = $"A: {_settings.PointA!.Value.X}, {_settings.PointA.Value.Y}    B: {_settings.PointB!.Value.X}, {_settings.PointB.Value.Y}";
        }
        else
        {
            RegionText.Text = "No capture area selected";
            PointText.Text = "Use Pick 2 Points to set the screenshot rectangle.";
        }

        CaptureNowButton.IsEnabled = region is { IsValid: true } && TryGetSaveDirectory(out _, out _);

        if (status is not null)
        {
            SetStatus(status, statusBrush ?? Brushes.Black);
        }
        else if (!TryGetSaveDirectory(out _, out var directoryError))
        {
            SetStatus(directoryError, _warnBrush);
        }
        else if (region is not { IsValid: true })
        {
            SetStatus("Set your capture area before using the hotkey.", _warnBrush);
        }
        else
        {
            SetStatus("Ready.", _okBrush);
        }
    }

    private void SetStatus(string message, MediaBrush brush)
    {
        StatusText.Text = message;
        StatusText.Foreground = brush;
    }

    private CaptureRegion? GetRegion()
    {
        if (_settings.PointA is null || _settings.PointB is null)
        {
            return null;
        }

        return CaptureRegion.FromPoints(_settings.PointA.Value, _settings.PointB.Value);
    }

    private bool TryGetSaveDirectory(out string saveDirectory, out string error)
    {
        saveDirectory = DirectoryTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(saveDirectory))
        {
            error = "Set a save directory before capturing.";
            return false;
        }

        try
        {
            saveDirectory = Path.GetFullPath(saveDirectory);
            error = string.Empty;
            return true;
        }
        catch (Exception ex)
        {
            error = $"Save directory is invalid: {ex.Message}";
            return false;
        }
    }

    private static bool IsModifierKey(Key key)
    {
        return key is Key.LeftCtrl or Key.RightCtrl
            or Key.LeftShift or Key.RightShift
            or Key.LeftAlt or Key.RightAlt
            or Key.LWin or Key.RWin;
    }

    private void TryUseExecutableIcon()
    {
        try
        {
            using var icon = DrawingIcon.ExtractAssociatedIcon(System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty);
            if (icon is null)
            {
                return;
            }

            Icon = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
        catch
        {
            // The embedded EXE icon is cosmetic; never block startup if Windows cannot provide it.
        }
    }
}
