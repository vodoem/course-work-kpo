using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Randomization;
using CosmicCollector.Core.Services;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет систему спавна.
/// </summary>
public sealed class SpawnSystemTests
{
  /// <summary>
  /// Проверяет, что спавн создаёт объект выше верхней границы.
  /// </summary>
  [Xunit.Fact]
  public void Spawn_CreatesObjectAboveTopBoundary()
  {
    var random = new FakeRandomProvider(new[] { 50, 1 });
    var config = CreateConfig(crystalIntervalTicks: 1);
    var state = CreateGameState(new WorldBounds(0, 0, 100, 100));
    var system = new SpawnSystem(random, config);

    system.TrySpawn(state, 1, 1);

    var snapshot = state.GetSnapshot();
    Xunit.Assert.Single(snapshot.parCrystals);
    var expectedY = 0 - ((config.CrystalBounds.Height / 2.0) + config.SpawnGap);
    Xunit.Assert.Equal(expectedY, snapshot.parCrystals[0].parPosition.Y, 6);
  }

  /// <summary>
  /// Проверяет, что спавн создаёт объект внутри X-границ.
  /// </summary>
  [Xunit.Fact]
  public void Spawn_CreatesObjectWithinXBounds()
  {
    var random = new FakeRandomProvider(new[] { 25, 1 });
    var config = CreateConfig(crystalIntervalTicks: 1, spawnMargin: 4);
    var state = CreateGameState(new WorldBounds(0, 0, 100, 100));
    var system = new SpawnSystem(random, config);

    system.TrySpawn(state, 1, 1);

    var snapshot = state.GetSnapshot();
    Xunit.Assert.Single(snapshot.parCrystals);
    var crystal = snapshot.parCrystals[0];
    var minX = state.WorldBounds.Left + (config.CrystalBounds.Width / 2.0) + config.SpawnMargin;
    var maxX = state.WorldBounds.Right - (config.CrystalBounds.Width / 2.0) - config.SpawnMargin;
    Xunit.Assert.InRange(crystal.parPosition.X, minX, maxX);
  }

  /// <summary>
  /// Проверяет соблюдение лимитов активных объектов.
  /// </summary>
  [Xunit.Fact]
  public void Spawn_RespectsLimits()
  {
    var random = new FakeRandomProvider(new[] { 20, 1, 30, 1 });
    var config = CreateConfig(crystalIntervalTicks: 1, maxActiveCrystals: 1);
    var state = CreateGameState(new WorldBounds(0, 0, 100, 100));
    var system = new SpawnSystem(random, config);

    system.TrySpawn(state, 1, 1);
    system.TrySpawn(state, 1, 2);

    var snapshot = state.GetSnapshot();
    Xunit.Assert.Single(snapshot.parCrystals);
  }

  /// <summary>
  /// Проверяет соблюдение интервалов спавна.
  /// </summary>
  [Xunit.Fact]
  public void Spawn_RespectsIntervalsTicks()
  {
    var random = new FakeRandomProvider(new[] { 20, 1 });
    var config = CreateConfig(crystalIntervalTicks: 2);
    var state = CreateGameState(new WorldBounds(0, 0, 100, 100));
    var system = new SpawnSystem(random, config);

    system.TrySpawn(state, 1, 1);
    Xunit.Assert.Empty(state.GetSnapshot().parCrystals);

    system.TrySpawn(state, 1, 2);
    Xunit.Assert.Single(state.GetSnapshot().parCrystals);
  }

  /// <summary>
  /// Проверяет масштабирование интервалов и скорости по уровню.
  /// </summary>
  [Xunit.Fact]
  public void Spawn_UsesLevelScaling()
  {
    var random = new FakeRandomProvider(new[] { 20, 1 });
    var config = CreateConfig(
      crystalIntervalTicks: 5,
      intervalDecreasePerLevel: 1,
      crystalBaseSpeed: 2.0);
    var state = CreateGameState(new WorldBounds(0, 0, 100, 100));
    var system = new SpawnSystem(random, config);

    system.TrySpawn(state, 3, 3);

    var snapshot = state.GetSnapshot();
    Xunit.Assert.Single(snapshot.parCrystals);
    var expectedSpeed = 2.0 * (1.0 + (0.015 * (3 - 1)));
    Xunit.Assert.Equal(expectedSpeed, snapshot.parCrystals[0].parVelocity.Y, 6);
  }

  /// <summary>
  /// Проверяет, что спавн избегает коллизии с дроном.
  /// </summary>
  [Xunit.Fact]
  public void Spawn_AvoidsImmediateCollisionWithDrone()
  {
    var random = new FakeRandomProvider(new[] { 10, 1, 10, 1, 10, 1 });
    var config = CreateConfig(crystalIntervalTicks: 1, spawnMargin: 5, maxSpawnAttempts: 3);
    var state = CreateGameState(new WorldBounds(0, 0, 20, 100));
    state.Drone.Position = new Vector2(10, -7);
    var system = new SpawnSystem(random, config);

    system.TrySpawn(state, 1, 1);

    Xunit.Assert.Empty(state.GetSnapshot().parCrystals);
  }

  /// <summary>
  /// Проверяет детерминированный выбор типа кристалла по весам.
  /// </summary>
  [Xunit.Fact]
  public void Spawn_WeightedTypesDeterministic()
  {
    var random = new FakeRandomProvider(new[] { 50, 5 });
    var weights = new List<WeightedOption<CrystalType>>
    {
      new(CrystalType.Blue, 1),
      new(CrystalType.Green, 2),
      new(CrystalType.Red, 3)
    };
    var config = CreateConfig(crystalIntervalTicks: 1, crystalWeights: weights);
    var state = CreateGameState(new WorldBounds(0, 0, 100, 100));
    var system = new SpawnSystem(random, config);

    system.TrySpawn(state, 1, 1);

    var snapshot = state.GetSnapshot();
    Xunit.Assert.Single(snapshot.parCrystals);
    Xunit.Assert.Equal(CrystalType.Red, snapshot.parCrystals[0].parType);
  }

  /// <summary>
  /// Проверяет детерминированный выбор типа бонуса по весам.
  /// </summary>
  [Xunit.Fact]
  public void BonusSpawn_WeightedBonusTypesDeterministic()
  {
    var random = new FakeRandomProvider(new[] { 40, 2 });
    var weights = new List<WeightedOption<BonusType>>
    {
      new(BonusType.Accelerator, 1),
      new(BonusType.TimeStabilizer, 2),
      new(BonusType.Magnet, 3)
    };
    var config = CreateConfig(
      crystalIntervalTicks: 0,
      asteroidIntervalTicks: 0,
      bonusIntervalTicks: 1,
      bonusWeights: weights);
    var state = CreateGameState(new WorldBounds(0, 0, 100, 100));
    var system = new SpawnSystem(random, config);

    system.TrySpawn(state, 1, 1);

    var snapshot = state.GetSnapshot();
    Xunit.Assert.Single(snapshot.parBonuses);
    Xunit.Assert.Equal(BonusType.TimeStabilizer, snapshot.parBonuses[0].parType);
  }

  /// <summary>
  /// Проверяет, что чёрная дыра спавнится выше верхней границы.
  /// </summary>
  [Xunit.Fact]
  public void Spawn_BlackHole_CreatesObjectAboveTopBoundary()
  {
    var random = new FakeRandomProvider(50);
    var config = CreateConfig(blackHoleIntervalTicks: 1);
    var state = CreateGameState(new WorldBounds(0, 0, 100, 100));
    var system = new SpawnSystem(random, config);

    system.TrySpawn(state, 1, 1);

    var snapshot = state.GetSnapshot();
    Xunit.Assert.Single(snapshot.parBlackHoles);
    var expectedY = 0 - ((config.BlackHoleBounds.Height / 2.0) + config.SpawnGap);
    Xunit.Assert.Equal(expectedY, snapshot.parBlackHoles[0].parPosition.Y, 6);
  }

  /// <summary>
  /// Проверяет лимиты и интервалы спавна чёрных дыр.
  /// </summary>
  [Xunit.Fact]
  public void Spawn_BlackHole_RespectsLimitsAndIntervals()
  {
    var random = new FakeRandomProvider(new[] { 20, 30 });
    var config = CreateConfig(blackHoleIntervalTicks: 2, maxActiveBlackHoles: 1);
    var state = CreateGameState(new WorldBounds(0, 0, 100, 100));
    var system = new SpawnSystem(random, config);

    system.TrySpawn(state, 1, 1);
    Xunit.Assert.Empty(state.GetSnapshot().parBlackHoles);

    system.TrySpawn(state, 1, 2);
    Xunit.Assert.Single(state.GetSnapshot().parBlackHoles);

    system.TrySpawn(state, 1, 4);
    Xunit.Assert.Single(state.GetSnapshot().parBlackHoles);
  }

  /// <summary>
  /// Проверяет нулевую скорость чёрной дыры.
  /// </summary>
  [Xunit.Fact]
  public void Spawn_BlackHole_HasZeroVelocity()
  {
    var random = new FakeRandomProvider(20);
    var config = CreateConfig(blackHoleIntervalTicks: 1, blackHoleBaseSpeed: 0);
    var state = CreateGameState(new WorldBounds(0, 0, 100, 100));
    var system = new SpawnSystem(random, config);

    system.TrySpawn(state, 1, 1);

    var snapshot = state.GetSnapshot();
    Xunit.Assert.Single(snapshot.parBlackHoles);
    Xunit.Assert.Equal(0, snapshot.parBlackHoles[0].parVelocity.X);
    Xunit.Assert.Equal(0, snapshot.parBlackHoles[0].parVelocity.Y);
  }

  private static GameState CreateGameState(WorldBounds parBounds)
  {
    var drone = new Drone(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      100);

    return new GameState(drone, parBounds);
  }

  private static SpawnConfig CreateConfig(
    int crystalIntervalTicks,
    int asteroidIntervalTicks = 0,
    int bonusIntervalTicks = 0,
    int blackHoleIntervalTicks = 0,
    int maxActiveCrystals = 3,
    int maxActiveAsteroids = 3,
    int maxActiveBonuses = 3,
    int maxActiveBlackHoles = 3,
    int intervalDecreasePerLevel = 0,
    int maxActiveIncreasePerLevel = 0,
    double spawnMargin = 1,
    double spawnGap = 2,
    int maxSpawnAttempts = 5,
    double crystalBaseSpeed = 4.0,
    double asteroidBaseSpeed = 6.0,
    double bonusBaseSpeed = 3.0,
    double blackHoleBaseSpeed = 0,
    double blackHoleRadius = 220,
    double blackHoleCoreRadius = 40,
    double bonusDurationSec = 5.0,
    double asteroidSpeedMultiplier = 1.3,
    IReadOnlyList<WeightedOption<CrystalType>>? crystalWeights = null,
    IReadOnlyList<WeightedOption<BonusType>>? bonusWeights = null)
  {
    return new SpawnConfig(
      true,
      maxActiveCrystals,
      maxActiveAsteroids,
      maxActiveBonuses,
      maxActiveBlackHoles,
      crystalIntervalTicks,
      asteroidIntervalTicks,
      bonusIntervalTicks,
      blackHoleIntervalTicks,
      intervalDecreasePerLevel,
      maxActiveIncreasePerLevel,
      spawnMargin,
      spawnGap,
      maxSpawnAttempts,
      new Aabb(10, 10),
      new Aabb(12, 12),
      new Aabb(8, 8),
      new Aabb(14, 14),
      crystalBaseSpeed,
      asteroidBaseSpeed,
      bonusBaseSpeed,
      blackHoleBaseSpeed,
      blackHoleRadius,
      blackHoleCoreRadius,
      bonusDurationSec,
      asteroidSpeedMultiplier,
      crystalWeights ?? new List<WeightedOption<CrystalType>> { new(CrystalType.Blue, 1) },
      bonusWeights ?? new List<WeightedOption<BonusType>> { new(BonusType.Accelerator, 1) });
  }
}
