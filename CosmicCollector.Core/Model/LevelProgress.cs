namespace CosmicCollector.Core.Model;

/// <summary>
/// Прогресс уровня по типам кристаллов.
/// </summary>
public sealed class LevelProgress
{
  /// <summary>
  /// Собрано синих кристаллов.
  /// </summary>
  public int CollectedBlue { get; internal set; }

  /// <summary>
  /// Собрано зелёных кристаллов.
  /// </summary>
  public int CollectedGreen { get; internal set; }

  /// <summary>
  /// Собрано красных кристаллов.
  /// </summary>
  public int CollectedRed { get; internal set; }

  /// <summary>
  /// Сбрасывает прогресс уровня.
  /// </summary>
  public void Reset()
  {
    CollectedBlue = 0;
    CollectedGreen = 0;
    CollectedRed = 0;
  }
}
