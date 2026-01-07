using System.Collections.Generic;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Geometry;

namespace CosmicCollector.Core.Services;

/// <summary>
/// Описывает параметры и ограничения системы спавна.
/// </summary>
public sealed class SpawnConfig
{
  /// <summary>
  /// Конфигурация с отключённым спавном.
  /// </summary>
  public static SpawnConfig Disabled =>
    new(
      false,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      1,
      1,
      1,
      new Aabb(10, 10),
      new Aabb(10, 10),
      new Aabb(10, 10),
      0,
      0,
      0,
      1,
      1,
      new List<WeightedOption<CrystalType>>(),
      new List<WeightedOption<BonusType>>());

  /// <summary>
  /// Инициализирует конфигурацию спавна.
  /// </summary>
  public SpawnConfig(
    bool parIsEnabled,
    int parMaxActiveCrystals,
    int parMaxActiveAsteroids,
    int parMaxActiveBonuses,
    int parCrystalIntervalTicks,
    int parAsteroidIntervalTicks,
    int parBonusIntervalTicks,
    int parIntervalDecreasePerLevel,
    int parMaxActiveIncreasePerLevel,
    double parSpawnMargin,
    double parSpawnGap,
    int parMaxSpawnAttempts,
    Aabb parCrystalBounds,
    Aabb parAsteroidBounds,
    Aabb parBonusBounds,
    double parCrystalBaseSpeed,
    double parAsteroidBaseSpeed,
    double parBonusBaseSpeed,
    double parBonusDurationSec,
    double parAsteroidSpeedMultiplier,
    IReadOnlyList<WeightedOption<CrystalType>> parCrystalTypeWeights,
    IReadOnlyList<WeightedOption<BonusType>> parBonusTypeWeights)
  {
    IsEnabled = parIsEnabled;
    MaxActiveCrystals = parMaxActiveCrystals;
    MaxActiveAsteroids = parMaxActiveAsteroids;
    MaxActiveBonuses = parMaxActiveBonuses;
    CrystalIntervalTicks = parCrystalIntervalTicks;
    AsteroidIntervalTicks = parAsteroidIntervalTicks;
    BonusIntervalTicks = parBonusIntervalTicks;
    IntervalDecreasePerLevel = parIntervalDecreasePerLevel;
    MaxActiveIncreasePerLevel = parMaxActiveIncreasePerLevel;
    SpawnMargin = parSpawnMargin;
    SpawnGap = parSpawnGap;
    MaxSpawnAttempts = parMaxSpawnAttempts;
    CrystalBounds = parCrystalBounds;
    AsteroidBounds = parAsteroidBounds;
    BonusBounds = parBonusBounds;
    CrystalBaseSpeed = parCrystalBaseSpeed;
    AsteroidBaseSpeed = parAsteroidBaseSpeed;
    BonusBaseSpeed = parBonusBaseSpeed;
    BonusDurationSec = parBonusDurationSec;
    AsteroidSpeedMultiplier = parAsteroidSpeedMultiplier;
    CrystalTypeWeights = parCrystalTypeWeights;
    BonusTypeWeights = parBonusTypeWeights;
  }

  /// <summary>
  /// Признак включённого спавна.
  /// </summary>
  public bool IsEnabled { get; }

  /// <summary>
  /// Максимум активных кристаллов.
  /// </summary>
  public int MaxActiveCrystals { get; }

  /// <summary>
  /// Максимум активных астероидов.
  /// </summary>
  public int MaxActiveAsteroids { get; }

  /// <summary>
  /// Максимум активных бонусов.
  /// </summary>
  public int MaxActiveBonuses { get; }

  /// <summary>
  /// Интервал спавна кристаллов в тиках.
  /// </summary>
  public int CrystalIntervalTicks { get; }

  /// <summary>
  /// Интервал спавна астероидов в тиках.
  /// </summary>
  public int AsteroidIntervalTicks { get; }

  /// <summary>
  /// Интервал спавна бонусов в тиках.
  /// </summary>
  public int BonusIntervalTicks { get; }

  /// <summary>
  /// Уменьшение интервала на уровень.
  /// </summary>
  public int IntervalDecreasePerLevel { get; }

  /// <summary>
  /// Увеличение лимита активных объектов на уровень.
  /// </summary>
  public int MaxActiveIncreasePerLevel { get; }

  /// <summary>
  /// Отступ по X от границ мира.
  /// </summary>
  public double SpawnMargin { get; }

  /// <summary>
  /// Разрыв спавна над верхней границей.
  /// </summary>
  public double SpawnGap { get; }

  /// <summary>
  /// Максимальное число попыток выбора позиции.
  /// </summary>
  public int MaxSpawnAttempts { get; }

  /// <summary>
  /// Габариты кристалла.
  /// </summary>
  public Aabb CrystalBounds { get; }

  /// <summary>
  /// Габариты астероида.
  /// </summary>
  public Aabb AsteroidBounds { get; }

  /// <summary>
  /// Габариты бонуса.
  /// </summary>
  public Aabb BonusBounds { get; }

  /// <summary>
  /// Базовая скорость кристаллов.
  /// </summary>
  public double CrystalBaseSpeed { get; }

  /// <summary>
  /// Базовая скорость астероидов.
  /// </summary>
  public double AsteroidBaseSpeed { get; }

  /// <summary>
  /// Базовая скорость бонусов.
  /// </summary>
  public double BonusBaseSpeed { get; }

  /// <summary>
  /// Длительность бонуса в секундах.
  /// </summary>
  public double BonusDurationSec { get; }

  /// <summary>
  /// Коэффициент скорости астероидов относительно базовой.
  /// </summary>
  public double AsteroidSpeedMultiplier { get; }

  /// <summary>
  /// Веса типов кристаллов.
  /// </summary>
  public IReadOnlyList<WeightedOption<CrystalType>> CrystalTypeWeights { get; }

  /// <summary>
  /// Веса типов бонусов.
  /// </summary>
  public IReadOnlyList<WeightedOption<BonusType>> BonusTypeWeights { get; }
}
