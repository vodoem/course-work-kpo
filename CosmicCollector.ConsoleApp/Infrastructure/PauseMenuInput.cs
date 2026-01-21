namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Снимок ввода для меню паузы.
/// </summary>
/// <param name="parUp">Переход вверх.</param>
/// <param name="parDown">Переход вниз.</param>
/// <param name="parEnter">Выбор пункта.</param>
/// <param name="parEscape">Отмена/назад.</param>
/// <param name="parConfirmYes">Подтвердить да.</param>
/// <param name="parConfirmNo">Подтвердить нет.</param>
public sealed record PauseMenuInput(
  bool parUp,
  bool parDown,
  bool parEnter,
  bool parEscape,
  bool parConfirmYes,
  bool parConfirmNo);
