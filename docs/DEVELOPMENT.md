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
- The snip sound is synthesized in memory to avoid bundling third-party sound assets.
- Notifications use `System.Windows.Forms.NotifyIcon` for the tray indicator and a reusable WPF toast for realtime latest-snip display. Notification failures should never block capture.

## Manual Test Scenarios

- Start with no settings file and confirm defaults load.
- Record a new hotkey, close, reopen, and confirm it persists.
- Pick point B above/left of point A and confirm capture still works.
- Set an invalid directory and confirm capture is blocked with an error.
- Press the hotkey while another app is focused and confirm a PNG is saved.
- Toggle sound on/off and confirm capture behavior still succeeds.
- Click Preview Sound and confirm it plays at the current slider volume.
- Move the volume slider and confirm the setting persists after restart.
- Toggle notifications and confirm latest-snip status still updates in the app.
- Take multiple captures quickly and confirm the notification updates in place to the newest file.
- Launch after changing the app icon and confirm startup does not depend on external asset files.
