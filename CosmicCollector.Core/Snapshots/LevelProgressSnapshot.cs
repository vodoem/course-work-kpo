namespace CosmicCollector.Core.Snapshots;

/// <summary>
/// Снимок прогресса уровня.
/// </summary>
/// <param name="parCollectedBlue">Собрано синих кристаллов.</param>
/// <param name="parCollectedGreen">Собрано зелёных кристаллов.</param>
/// <param name="parCollectedRed">Собрано красных кристаллов.</param>
public sealed record LevelProgressSnapshot(
  int parCollectedBlue,
  int parCollectedGreen,
  int parCollectedRed);
