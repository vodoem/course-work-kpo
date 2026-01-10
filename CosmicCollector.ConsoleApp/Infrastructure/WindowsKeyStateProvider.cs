using System;
using System.Runtime.InteropServices;

namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Провайдер состояния клавиш через WinAPI GetAsyncKeyState.
/// </summary>
public sealed class WindowsKeyStateProvider : IKeyStateProvider
{
  /// <inheritdoc />
  public bool IsKeyDown(ConsoleKey parKey)
  {
    if (!OperatingSystem.IsWindows())
    {
      return false;
    }

    int virtualKey = MapToVirtualKey(parKey);
    short state = GetAsyncKeyState(virtualKey);
    return (state & 0x8000) != 0;
  }

  private static int MapToVirtualKey(ConsoleKey parKey)
  {
    return parKey switch
    {
      ConsoleKey.LeftArrow => 0x25,
      ConsoleKey.UpArrow => 0x26,
      ConsoleKey.RightArrow => 0x27,
      ConsoleKey.DownArrow => 0x28,
      ConsoleKey.A => 0x41,
      ConsoleKey.D => 0x44,
      ConsoleKey.P => 0x50,
      _ => (int)parKey
    };
  }

  [DllImport("user32.dll")]
  private static extern short GetAsyncKeyState(int parVirtualKey);
}
