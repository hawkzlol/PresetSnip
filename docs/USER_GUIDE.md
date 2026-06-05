# User Guide

## First Run

1. Open `PresetSnip.exe`.
2. Choose where captures should be saved.
3. Leave the hotkey as `DEL`, or click **Start Record** and press a new key combination.
4. Click **Pick 2 Points**.
5. After the countdown, click the first corner and then the opposite corner of the capture area.

## Capturing

Once a valid directory and capture area are set, press the configured hotkey while PresetSnip is open. The app captures the saved rectangle immediately and writes a PNG file to the selected directory.

## Changing the Hotkey

1. Click **Start Record**.
2. Press the key or key combination you want to use.
3. Press `Esc` during recording to cancel.

If another app already owns the hotkey, PresetSnip keeps running and shows an error in the status area.

## Changing the Capture Area

Click **Pick 2 Points** again and choose two new corners. The click order does not matter; PresetSnip normalizes the rectangle automatically.

## Settings

Settings are stored locally in:

```text
%AppData%/PresetSnip/settings.json
```

Delete that file to reset PresetSnip to defaults.
