namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Действие после экрана завершения игры.
/// </summary>
public enum GameEndAction
{
  /// <summary>
  /// Вернуться в главное меню.
  /// </summary>
  ReturnToMenu,

  /// <summary>
  /// Запустить новую игру.
  /// </summary>
  RestartGame
}
