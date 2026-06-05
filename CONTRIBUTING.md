# Contributing

Thanks for improving PresetSnip. This project is intentionally small and Windows-focused, so changes should keep the app easy to build, easy to understand, and dependency-light.

## Local Setup

1. Install the .NET 6 SDK.
2. Restore dependencies:

   ```powershell
   dotnet restore
   ```

3. Build:

   ```powershell
   dotnet build -c Release
   ```

## Development Guidelines

- Keep user settings local and outside the repository.
- Do not commit screenshots, captures, personal paths, or machine-specific config.
- Prefer built-in .NET, WPF, Windows Forms, and Win32 APIs over new dependencies.
- Keep UI behavior direct: set directory, set hotkey, pick points, capture.
- If a change touches capture coordinates, test with normal and reversed point order.

## Pull Request Checklist

- Build passes with `dotnet build -c Release`.
- Public docs are updated if behavior changes.
- No personal paths or local settings are committed.
- New errors show clear user-facing messages instead of crashing.
