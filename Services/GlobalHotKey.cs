using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using PresetSnip.Models;

namespace PresetSnip.Services;

public sealed class GlobalHotKey : IDisposable
{
    private const int HotKeyMessage = 0x0312;
    private const int HotKeyId = 0x5331;

    private readonly HwndSource _source;
    private bool _registered;

    public GlobalHotKey(HwndSource source)
    {
        _source = source;
        _source.AddHook(WndProc);
    }

    public event EventHandler? Pressed;

    public void Register(HotKeyGesture gesture)
    {
        Unregister();

        var virtualKey = KeyInterop.VirtualKeyFromKey(gesture.Key);
        if (virtualKey == 0)
        {
            throw new InvalidOperationException("That key cannot be used as a hotkey.");
        }

        var modifiers = ToNativeModifiers(gesture.Modifiers);
        if (!RegisterHotKey(_source.Handle, HotKeyId, modifiers, (uint)virtualKey))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), $"Could not register {gesture.DisplayText}. Another app may already be using it.");
        }

        _registered = true;
    }

    public void Unregister()
    {
        if (!_registered)
        {
            return;
        }

        UnregisterHotKey(_source.Handle, HotKeyId);
        _registered = false;
    }

    public void Dispose()
    {
        Unregister();
        _source.RemoveHook(WndProc);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == HotKeyMessage && wParam.ToInt32() == HotKeyId)
        {
            handled = true;
            Pressed?.Invoke(this, EventArgs.Empty);
        }

        return IntPtr.Zero;
    }

    private static uint ToNativeModifiers(ModifierKeys modifiers)
    {
        uint value = 0;

        if (modifiers.HasFlag(ModifierKeys.Alt))
        {
            value |= 0x0001;
        }

        if (modifiers.HasFlag(ModifierKeys.Control))
        {
            value |= 0x0002;
        }

        if (modifiers.HasFlag(ModifierKeys.Shift))
        {
            value |= 0x0004;
        }

        if (modifiers.HasFlag(ModifierKeys.Windows))
        {
            value |= 0x0008;
        }

        return value;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
}
