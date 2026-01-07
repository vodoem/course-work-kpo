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
    var config = CreateConfig();
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
    var config = CreateConfig(builder => builder.SpawnMargin = 4);
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
    var config = CreateConfig(builder => builder.MaxActiveCrystals = 1);
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
    var config = CreateConfig(builder => builder.CrystalIntervalTicks = 2);
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
    var config = CreateConfig(builder =>
    {
      builder.CrystalIntervalTicks = 5;
      builder.IntervalDecreasePerLevel = 1;
      builder.CrystalBaseSpeed = 2.0;
    });
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
    var config = CreateConfig(builder =>
    {
      builder.SpawnMargin = 5;
      builder.MaxSpawnAttempts = 3;
    });
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
    var config = CreateConfig(builder => builder.CrystalWeights = weights);
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
    var config = CreateConfig(builder =>
    {
      builder.CrystalIntervalTicks = 0;
      builder.AsteroidIntervalTicks = 0;
      builder.BonusIntervalTicks = 1;
      builder.BonusWeights = weights;
    });
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
    var config = CreateConfig(builder =>
    {
      builder.CrystalIntervalTicks = 0;
      builder.BlackHoleIntervalTicks = 1;
    });
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
    var config = CreateConfig(builder =>
    {
      builder.CrystalIntervalTicks = 0;
      builder.BlackHoleIntervalTicks = 2;
      builder.MaxActiveBlackHoles = 1;
    });
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
    var config = CreateConfig(builder =>
    {
      builder.CrystalIntervalTicks = 0;
      builder.BlackHoleIntervalTicks = 1;
      builder.BlackHoleBaseSpeed = 0;
    });
    var state = CreateGameState(new WorldBounds(0, 0, 100, 100));
    var system = new SpawnSystem(random, config);

    system.TrySpawn(state, 1, 1);

    var snapshot = state.GetSnapshot();
    Xunit.Assert.Single(snapshot.parBlackHoles);
    Xunit.Assert.Equal(0, snapshot.parBlackHoles[0].parVelocity.X);
    Xunit.Assert.Equal(0, snapshot.parBlackHoles[0].parVelocity.Y);
  }

  /// <summary>
  /// Проверяет, что SpawnSystem использует фабрику объектов.
  /// </summary>
  [Xunit.Fact]
  public void Spawn_UsesFactory()
  {
    var random = new FakeRandomProvider(20);
    var factory = new SpyFactory();
    var config = CreateConfig(builder =>
    {
      builder.CrystalIntervalTicks = 1;
    });
    var state = CreateGameState(new WorldBounds(0, 0, 100, 100));
    var system = new SpawnSystem(random, config, factory);

    system.TrySpawn(state, 1, 1);

    Xunit.Assert.Equal(1, factory.CrystalCreatedCount);
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

  private static SpawnConfig CreateConfig(Action<SpawnConfigBuilder>? parConfigure = null)
  {
    var builder = new SpawnConfigBuilder();
    parConfigure?.Invoke(builder);
    return builder.Build();
  }

  private sealed class SpawnConfigBuilder
  {
    public int CrystalIntervalTicks { get; set; } = 1;
    public int AsteroidIntervalTicks { get; set; } = 0;
    public int BonusIntervalTicks { get; set; } = 0;
    public int BlackHoleIntervalTicks { get; set; } = 0;
    public int MaxActiveCrystals { get; set; } = 3;
    public int MaxActiveAsteroids { get; set; } = 3;
    public int MaxActiveBonuses { get; set; } = 3;
    public int MaxActiveBlackHoles { get; set; } = 3;
    public int IntervalDecreasePerLevel { get; set; } = 0;
    public int MaxActiveIncreasePerLevel { get; set; } = 0;
    public double SpawnMargin { get; set; } = 1;
    public double SpawnGap { get; set; } = 2;
    public int MaxSpawnAttempts { get; set; } = 5;
    public double CrystalBaseSpeed { get; set; } = 4.0;
    public double AsteroidBaseSpeed { get; set; } = 6.0;
    public double BonusBaseSpeed { get; set; } = 3.0;
    public double BlackHoleBaseSpeed { get; set; } = 0;
    public double BlackHoleRadius { get; set; } = 220;
    public double BlackHoleCoreRadius { get; set; } = 40;
    public double BonusDurationSec { get; set; } = 5.0;
    public double AsteroidSpeedMultiplier { get; set; } = 1.3;
    public IReadOnlyList<WeightedOption<CrystalType>>? CrystalWeights { get; set; }
    public IReadOnlyList<WeightedOption<BonusType>>? BonusWeights { get; set; }

    public SpawnConfig Build()
    {
      return new SpawnConfig(
        true,
        MaxActiveCrystals,
        MaxActiveAsteroids,
        MaxActiveBonuses,
        MaxActiveBlackHoles,
        CrystalIntervalTicks,
        AsteroidIntervalTicks,
        BonusIntervalTicks,
        BlackHoleIntervalTicks,
        IntervalDecreasePerLevel,
        MaxActiveIncreasePerLevel,
        SpawnMargin,
        SpawnGap,
        MaxSpawnAttempts,
        new Aabb(10, 10),
        new Aabb(12, 12),
        new Aabb(8, 8),
        new Aabb(14, 14),
        CrystalBaseSpeed,
        AsteroidBaseSpeed,
        BonusBaseSpeed,
        BlackHoleBaseSpeed,
        BlackHoleRadius,
        BlackHoleCoreRadius,
        BonusDurationSec,
        AsteroidSpeedMultiplier,
        CrystalWeights ?? new List<WeightedOption<CrystalType>> { new(CrystalType.Blue, 1) },
        BonusWeights ?? new List<WeightedOption<BonusType>> { new(BonusType.Accelerator, 1) });
    }
  }

  private sealed class SpyFactory : IGameObjectFactory
  {
    public int CrystalCreatedCount { get; private set; }

    public Crystal CreateCrystal(Guid parId, Vector2 parPosition, Vector2 parVelocity, Aabb parBounds, CrystalType parType)
    {
      CrystalCreatedCount++;
      return new Crystal(parId, parPosition, parVelocity, parBounds, parType);
    }

    public Asteroid CreateAsteroid(Guid parId, Vector2 parPosition, Vector2 parVelocity, Aabb parBounds)
    {
      return new Asteroid(parId, parPosition, parVelocity, parBounds);
    }

    public Bonus CreateBonus(
      Guid parId,
      Vector2 parPosition,
      Vector2 parVelocity,
      Aabb parBounds,
      BonusType parType,
      double parDurationSec)
    {
      return new Bonus(parId, parPosition, parVelocity, parBounds, parType, parDurationSec);
    }

    public BlackHole CreateBlackHole(
      Guid parId,
      Vector2 parPosition,
      Vector2 parVelocity,
      Aabb parBounds,
      double parRadius,
      double parCoreRadius)
    {
      return new BlackHole(parId, parPosition, parVelocity, parBounds, parRadius, parCoreRadius);
    }
  }
}
