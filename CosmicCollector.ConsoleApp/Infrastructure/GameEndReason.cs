namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Причина завершения игры.
/// </summary>
public enum GameEndReason
{
  /// <summary>
  /// Игра окончена.
  /// </summary>
  GameOver,

  /// <summary>
  /// Уровень пройден.
  /// </summary>
  LevelCompleted
}
