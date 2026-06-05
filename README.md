# PresetSnip

![Platform](https://img.shields.io/badge/platform-Windows-6D28D9)
![.NET](https://img.shields.io/badge/.NET-6.0-512BD4)
![License](https://img.shields.io/badge/license-MIT-16A34A)

PresetSnip is a small Windows desktop screenshot utility for capturing the same screen region repeatedly. Instead of dragging a snipping rectangle every time, you choose two fixed points once, set a save folder, and press a global hotkey whenever you want a new PNG capture.

<p align="center">
  <img src="Assets/PresetSnipIcon.png" alt="PresetSnip icon" width="160" />
</p>

## Features

- Dark purple WPF interface.
- Global hotkey while the app is open, defaulting to `DEL`.
- Hotkey recorder for changing the shortcut without editing config files.
- Save directory field with paste support and a native folder picker.
- 3-second delayed point picker overlay for selecting point A and point B.
- Immediate PNG capture of the saved rectangle.
- Settings persisted across restarts:
  - save directory
  - hotkey
  - point A
  - point B
  - last saved path
- Self-contained Windows publish option for a portable `.exe`.

## Requirements

- Windows 10 or newer.
- .NET 6 SDK for building from source.

The app targets `net6.0-windows` and uses WPF, Windows Forms folder picking, and Win32 hotkey/screen APIs.

## Quick Start

1. Download or build `PresetSnip.exe`.
2. Launch the app.
3. Choose a save directory.
4. Click **Pick 2 Points**.
5. Wait for the countdown, then click the first and second corners of the area to capture.
6. Press the configured hotkey from any focused app while PresetSnip is running.

Captures are saved as timestamped PNG files, for example:

```text
PresetSnip_2026-06-05_143012.png
```

## Build

Restore and build:

```powershell
dotnet restore
dotnet build PresetSnip.sln -c Release
```

Publish a portable self-contained Windows executable:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
```

The published executable is created under:

```text
bin/Release/net6.0-windows/win-x64/publish/
```

## Documentation

- [User guide](docs/USER_GUIDE.md)
- [Development guide](docs/DEVELOPMENT.md)
- [Architecture](docs/ARCHITECTURE.md)
- [Privacy](docs/PRIVACY.md)
- [Resources used](docs/RESOURCES.md)

## Project Layout

```text
Assets/                     App icon source and .ico file
Models/                     Persisted settings and capture-region models
Services/                   Hotkey, screenshot, settings, cursor, DPI, and screen helpers
docs/                       User, development, privacy, architecture, and resource docs
.github/                    GitHub Actions and issue templates
```

## Configuration

PresetSnip stores local user settings in:

```text
%AppData%/PresetSnip/settings.json
```

This file is intentionally outside the repository and should not be committed.

## License

PresetSnip is released under the MIT License. See [LICENSE](LICENSE).
