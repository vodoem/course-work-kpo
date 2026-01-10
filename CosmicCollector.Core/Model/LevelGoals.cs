namespace CosmicCollector.Core.Model;

/// <summary>
/// Цели уровня по типам кристаллов.
/// </summary>
public sealed class LevelGoals
{
  /// <summary>
  /// Инициализирует цели уровня.
  /// </summary>
  /// <param name="parRequiredBlue">Требуемое число синих кристаллов.</param>
  /// <param name="parRequiredGreen">Требуемое число зелёных кристаллов.</param>
  /// <param name="parRequiredRed">Требуемое число красных кристаллов.</param>
  public LevelGoals(int parRequiredBlue, int parRequiredGreen, int parRequiredRed)
  {
    RequiredBlue = parRequiredBlue;
    RequiredGreen = parRequiredGreen;
    RequiredRed = parRequiredRed;
  }

  /// <summary>
  /// Требуемое число синих кристаллов.
  /// </summary>
  public int RequiredBlue { get; }

  /// <summary>
  /// Требуемое число зелёных кристаллов.
  /// </summary>
  public int RequiredGreen { get; }

  /// <summary>
  /// Требуемое число красных кристаллов.
  /// </summary>
  public int RequiredRed { get; }
}
