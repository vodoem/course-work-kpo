namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Абстракция для отрисовки в консоли.
/// </summary>
public interface IConsoleRenderer
{
  /// <summary>
  /// Очищает экран.
  /// </summary>
  void Clear();

  /// <summary>
  /// Пишет строку.
  /// </summary>
  /// <param name="parText">Текст строки.</param>
  void WriteLine(string parText);

  /// <summary>
  /// Читает нажатие клавиши.
  /// </summary>
  /// <returns>Сведения о нажатой клавише.</returns>
  ConsoleKeyInfo ReadKey();

  /// <summary>
  /// Возвращает ширину буфера.
  /// </summary>
  int BufferWidth { get; }

  /// <summary>
  /// Возвращает высоту буфера.
  /// </summary>
  int BufferHeight { get; }
}
