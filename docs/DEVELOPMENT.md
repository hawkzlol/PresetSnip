# Development Guide

## Stack

- C#
- .NET 6
- WPF
- Windows Forms folder picker
- Win32 APIs for global hotkeys, cursor position, DPI awareness, and virtual screen bounds
- `System.Drawing` for screen capture on Windows

## Common Commands

Restore:

```powershell
dotnet restore
```

Build:

```powershell
dotnet build -c Release
```

Publish:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
```

## Notes

- The app uses physical screen coordinates from Win32 cursor APIs so the chosen points line up with `Graphics.CopyFromScreen`.
- The overlay covers the Windows virtual screen, including negative coordinates for monitors arranged to the left or above the primary monitor.
- `RegisterHotKey` is scoped to the app window handle and is released when the window closes.
- Settings load failures intentionally fall back to defaults so a corrupt settings file does not prevent launch.

## Manual Test Scenarios

- Start with no settings file and confirm defaults load.
- Record a new hotkey, close, reopen, and confirm it persists.
- Pick point B above/left of point A and confirm capture still works.
- Set an invalid directory and confirm capture is blocked with an error.
- Press the hotkey while another app is focused and confirm a PNG is saved.
- Launch after changing the app icon and confirm startup does not depend on external asset files.
