namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Абстракция чтения ввода.
/// </summary>
public interface IConsoleInputReader
{
  /// <summary>
  /// Считывает нажатие клавиши.
  /// </summary>
  /// <returns>Сведения о нажатой клавише.</returns>
  ConsoleKeyInfo ReadKey();
}
