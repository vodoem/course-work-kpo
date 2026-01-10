namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Провайдер состояния клавиш для не-Windows платформ через Console.KeyAvailable.
/// </summary>
public sealed class ConsoleKeyStateProvider : IKeyStateProvider
{
  private readonly object _lockObject = new();
  private readonly HashSet<ConsoleKey> _bufferedKeys = new();

  /// <inheritdoc />
  public bool IsKeyDown(ConsoleKey parKey)
  {
    lock (_lockObject)
    {
      CaptureKeys();
      return _bufferedKeys.Remove(parKey);
    }
  }

  private void CaptureKeys()
  {
    while (Console.KeyAvailable)
    {
      var key = Console.ReadKey(true).Key;
      _bufferedKeys.Add(key);
    }
  }
}
