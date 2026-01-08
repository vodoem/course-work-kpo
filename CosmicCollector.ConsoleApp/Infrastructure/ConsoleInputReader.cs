namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Реализация чтения ввода на базе System.Console.
/// </summary>
public sealed class ConsoleInputReader : IConsoleInputReader
{
  /// <inheritdoc />
  public ConsoleKeyInfo ReadKey()
  {
    return Console.ReadKey(true);
  }
}
