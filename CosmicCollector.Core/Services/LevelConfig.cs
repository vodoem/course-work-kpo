namespace CosmicCollector.Core.Services;

/// <summary>
/// Конфигурация уровня.
/// </summary>
/// <param name="parRequiredBlue">Требуемое число синих кристаллов.</param>
/// <param name="parRequiredGreen">Требуемое число зелёных кристаллов.</param>
/// <param name="parRequiredRed">Требуемое число красных кристаллов.</param>
/// <param name="parLevelTimeSec">Время уровня в секундах.</param>
public sealed record LevelConfig(
  int parRequiredBlue,
  int parRequiredGreen,
  int parRequiredRed,
  double parLevelTimeSec);
