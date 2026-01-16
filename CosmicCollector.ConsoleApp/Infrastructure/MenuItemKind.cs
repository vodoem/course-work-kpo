namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Идентификатор пункта главного меню.
/// </summary>
public enum MenuItemKind
{
  /// <summary>
  /// Запуск игры.
  /// </summary>
  Play,

  /// <summary>
  /// Переход к экрану рекордов.
  /// </summary>
  Records,

  /// <summary>
  /// Переход к экрану правил.
  /// </summary>
  Rules,

  /// <summary>
  /// Выход из приложения.
  /// </summary>
  Exit
}
