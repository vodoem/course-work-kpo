namespace CosmicCollector.Core.Snapshots;

/// <summary>
/// Снимок целей уровня.
/// </summary>
/// <param name="parRequiredBlue">Требуемое число синих кристаллов.</param>
/// <param name="parRequiredGreen">Требуемое число зелёных кристаллов.</param>
/// <param name="parRequiredRed">Требуемое число красных кристаллов.</param>
public sealed record LevelGoalsSnapshot(
  int parRequiredBlue,
  int parRequiredGreen,
  int parRequiredRed);
