# Architecture

PresetSnip is a small WPF application with UI code in `MainWindow` and OS-specific behavior split into services.

## Main Components

- `MainWindow`: user workflow, status updates, settings save points, and hotkey recording.
- `SelectionOverlayWindow`: full-screen point picker overlay.
- `SettingsStore`: JSON persistence under the current user's app data directory.
- `GlobalHotKey`: `RegisterHotKey` / `UnregisterHotKey` wrapper.
- `ScreenCaptureService`: rectangle screenshot capture and PNG output.
- `SnipSoundService`: generated snip sound playback with volume scaling.
- `NotificationService`: latest-snip tray indicator and reusable realtime toast.
- `VirtualScreen`: virtual monitor bounds.
- `NativeCursor`: physical cursor position.
- `DpiAwareness`: best-effort per-monitor DPI awareness.

## Data Flow

1. User sets directory, hotkey, and capture points.
2. `MainWindow` writes those values through `SettingsStore`.
3. `GlobalHotKey` raises an event when the configured key is pressed.
4. `ScreenCaptureService` captures the normalized rectangle and saves a timestamped PNG.
5. Optional feedback runs: generated sound playback and latest-snip notification; newer captures update the same toast immediately.
6. The last saved path is persisted for display on the next launch.

## Persistence Model

The persisted settings file contains only local app state:

- save directory
- hotkey key and modifiers
- point A
- point B
- last saved path
- sound enabled
- sound volume
- notifications enabled

No screenshots are stored in the settings file.
