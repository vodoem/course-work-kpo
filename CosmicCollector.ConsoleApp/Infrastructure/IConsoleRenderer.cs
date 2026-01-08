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
  /// Устанавливает позицию курсора.
  /// </summary>
  /// <param name="parLeft">Колонка.</param>
  /// <param name="parTop">Строка.</param>
  void SetCursorPosition(int parLeft, int parTop);

  /// <summary>
  /// Пишет текст без перевода строки.
  /// </summary>
  /// <param name="parText">Текст.</param>
  void Write(string parText);

  /// <summary>
  /// Возвращает ширину буфера.
  /// </summary>
  int BufferWidth { get; }

  /// <summary>
  /// Возвращает высоту буфера.
  /// </summary>
  int BufferHeight { get; }
}
