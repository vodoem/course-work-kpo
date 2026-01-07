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
    new();

  /// <summary>
  /// Конфигурация спавна с параметрами по умолчанию из ТЗ.
  /// </summary>
  public static SpawnConfig Default =>
    new(
      true,
      3,
      3,
      3,
      1,
      60,
      90,
      120,
      180,
      0,
      0,
      4,
      6,
      10,
      new Aabb(10, 10),
      new Aabb(12, 12),
      new Aabb(8, 8),
      new Aabb(14, 14),
      2,
      3,
      2,
      0,
      220,
      40,
      5,
      1.3,
      new List<WeightedOption<CrystalType>>
      {
        new(CrystalType.Blue, 1),
        new(CrystalType.Green, 1),
        new(CrystalType.Red, 1)
      },
      new List<WeightedOption<BonusType>>
      {
        new(BonusType.Accelerator, 1),
        new(BonusType.TimeStabilizer, 1),
        new(BonusType.Magnet, 1)
      });

  private SpawnConfig()
  {
    IsEnabled = false;
    MaxActiveCrystals = 0;
    MaxActiveAsteroids = 0;
    MaxActiveBonuses = 0;
    MaxActiveBlackHoles = 0;
    CrystalIntervalTicks = 0;
    AsteroidIntervalTicks = 0;
    BonusIntervalTicks = 0;
    BlackHoleIntervalTicks = 0;
    IntervalDecreasePerLevel = 0;
    MaxActiveIncreasePerLevel = 0;
    SpawnMargin = 1;
    SpawnGap = 1;
    MaxSpawnAttempts = 1;
    CrystalBounds = new Aabb(10, 10);
    AsteroidBounds = new Aabb(10, 10);
    BonusBounds = new Aabb(10, 10);
    BlackHoleBounds = new Aabb(10, 10);
    CrystalBaseSpeed = 0;
    AsteroidBaseSpeed = 0;
    BonusBaseSpeed = 0;
    BlackHoleBaseSpeed = 0;
    BlackHoleRadius = 0;
    BlackHoleCoreRadius = 0;
    BonusDurationSec = 0;
    AsteroidSpeedMultiplier = 1;
    CrystalTypeWeights = new List<WeightedOption<CrystalType>>();
    BonusTypeWeights = new List<WeightedOption<BonusType>>();
  }

  /// <summary>
  /// Инициализирует конфигурацию спавна.
  /// </summary>
  public SpawnConfig(
    bool parIsEnabled,
    int parMaxActiveCrystals,
    int parMaxActiveAsteroids,
    int parMaxActiveBonuses,
    int parMaxActiveBlackHoles,
    int parCrystalIntervalTicks,
    int parAsteroidIntervalTicks,
    int parBonusIntervalTicks,
    int parBlackHoleIntervalTicks,
    int parIntervalDecreasePerLevel,
    int parMaxActiveIncreasePerLevel,
    double parSpawnMargin,
    double parSpawnGap,
    int parMaxSpawnAttempts,
    Aabb parCrystalBounds,
    Aabb parAsteroidBounds,
    Aabb parBonusBounds,
    Aabb parBlackHoleBounds,
    double parCrystalBaseSpeed,
    double parAsteroidBaseSpeed,
    double parBonusBaseSpeed,
    double parBlackHoleBaseSpeed,
    double parBlackHoleRadius,
    double parBlackHoleCoreRadius,
    double parBonusDurationSec,
    double parAsteroidSpeedMultiplier,
    IReadOnlyList<WeightedOption<CrystalType>> parCrystalTypeWeights,
    IReadOnlyList<WeightedOption<BonusType>> parBonusTypeWeights)
  {
    IsEnabled = parIsEnabled;
    MaxActiveCrystals = parMaxActiveCrystals;
    MaxActiveAsteroids = parMaxActiveAsteroids;
    MaxActiveBonuses = parMaxActiveBonuses;
    MaxActiveBlackHoles = parMaxActiveBlackHoles;
    CrystalIntervalTicks = parCrystalIntervalTicks;
    AsteroidIntervalTicks = parAsteroidIntervalTicks;
    BonusIntervalTicks = parBonusIntervalTicks;
    BlackHoleIntervalTicks = parBlackHoleIntervalTicks;
    IntervalDecreasePerLevel = parIntervalDecreasePerLevel;
    MaxActiveIncreasePerLevel = parMaxActiveIncreasePerLevel;
    SpawnMargin = parSpawnMargin;
    SpawnGap = parSpawnGap;
    MaxSpawnAttempts = parMaxSpawnAttempts;
    CrystalBounds = parCrystalBounds;
    AsteroidBounds = parAsteroidBounds;
    BonusBounds = parBonusBounds;
    BlackHoleBounds = parBlackHoleBounds;
    CrystalBaseSpeed = parCrystalBaseSpeed;
    AsteroidBaseSpeed = parAsteroidBaseSpeed;
    BonusBaseSpeed = parBonusBaseSpeed;
    BlackHoleBaseSpeed = parBlackHoleBaseSpeed;
    BlackHoleRadius = parBlackHoleRadius;
    BlackHoleCoreRadius = parBlackHoleCoreRadius;
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
  /// Максимум активных чёрных дыр.
  /// </summary>
  public int MaxActiveBlackHoles { get; }

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
  /// Интервал спавна чёрных дыр в тиках.
  /// </summary>
  public int BlackHoleIntervalTicks { get; }

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
  /// Габариты чёрной дыры.
  /// </summary>
  public Aabb BlackHoleBounds { get; }

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
  /// Базовая скорость чёрных дыр.
  /// </summary>
  public double BlackHoleBaseSpeed { get; }

  /// <summary>
  /// Радиус влияния чёрной дыры.
  /// </summary>
  public double BlackHoleRadius { get; }

  /// <summary>
  /// Радиус ядра чёрной дыры.
  /// </summary>
  public double BlackHoleCoreRadius { get; }

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
