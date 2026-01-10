namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Реализация рендерера на базе System.Console.
/// </summary>
public sealed class ConsoleRenderer : IConsoleRenderer
{
  /// <inheritdoc />
  public void Clear()
  {
    Console.Clear();
  }

  /// <inheritdoc />
  public void WriteLine(string parText)
  {
    Console.WriteLine(parText);
  }

  /// <inheritdoc />
  public ConsoleKeyInfo ReadKey()
  {
    return Console.ReadKey(true);
  }

  /// <inheritdoc />
  public void SetCursorPosition(int parLeft, int parTop)
  {
    Console.SetCursorPosition(parLeft, parTop);
  }

  /// <inheritdoc />
  public void Write(string parText)
  {
    Console.Write(parText);
  }

  /// <inheritdoc />
  public void SetForegroundColor(ConsoleColor parColor)
  {
    Console.ForegroundColor = parColor;
  }

  /// <inheritdoc />
  public void ResetColor()
  {
    Console.ResetColor();
  }

  /// <inheritdoc />
  public void SetBufferSize(int parWidth, int parHeight)
  {
    if (!OperatingSystem.IsWindows())
    {
      return;
    }

    try
    {
      Console.SetBufferSize(parWidth, parHeight);
    }
    catch (ArgumentOutOfRangeException)
    {
    }
    catch (IOException)
    {
    }
  }

  /// <inheritdoc />
  public int BufferWidth => Console.BufferWidth;

  /// <inheritdoc />
  public int BufferHeight => Console.BufferHeight;

  /// <inheritdoc />
  public int WindowWidth => Console.WindowWidth;

  /// <inheritdoc />
  public int WindowHeight => Console.WindowHeight;
}
