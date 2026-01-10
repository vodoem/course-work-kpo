namespace CosmicCollector.Core.Model;

/// <summary>
/// Состояние уровня.
/// </summary>
public sealed class LevelState
{
  /// <summary>
  /// Инициализирует состояние уровня.
  /// </summary>
  public LevelState()
  {
    Goals = new LevelGoals(0, 0, 0);
    Progress = new LevelProgress();
  }

  /// <summary>
  /// Номер текущего уровня.
  /// </summary>
  public int CurrentLevel { get; set; }

  /// <summary>
  /// Цели уровня.
  /// </summary>
  public LevelGoals Goals { get; set; }

  /// <summary>
  /// Прогресс уровня.
  /// </summary>
  public LevelProgress Progress { get; }

  /// <summary>
  /// Оставшееся время уровня в секундах.
  /// </summary>
  public double TimeRemainingSec { get; set; }

  /// <summary>
  /// Признак наличия таймера уровня.
  /// </summary>
  public bool HasTimer { get; set; }

  /// <summary>
  /// Признак инициализации уровня.
  /// </summary>
  public bool IsInitialized { get; set; }
}
