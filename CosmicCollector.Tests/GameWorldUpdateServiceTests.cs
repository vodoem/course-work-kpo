using System;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Events;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Randomization;
using CosmicCollector.Core.Services;
using CosmicCollector.MVC.Eventing;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет базовую логику GameWorldUpdateService.
/// </summary>
public sealed class GameWorldUpdateServiceTests
{
  /// <summary>
  /// Проверяет начисление очков за синий кристалл.
  /// </summary>
  [Xunit.Fact]
  public void Update_AwardsBlueCrystalPoints()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.AddCrystal(CreateCrystal(CrystalType.Blue));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal(8, state.Score);
  }

  /// <summary>
  /// Проверяет начисление очков за зелёный кристалл.
  /// </summary>
  [Xunit.Fact]
  public void Update_AwardsGreenCrystalPoints()
  {
    var random = new FakeRandomProvider(5);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.AddCrystal(CreateCrystal(CrystalType.Green));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal(5, state.Score);
  }

  /// <summary>
  /// Проверяет начисление очков за красный кристалл.
  /// </summary>
  [Xunit.Fact]
  public void Update_AwardsRedCrystalPoints()
  {
    var random = new FakeRandomProvider(1);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.AddCrystal(CreateCrystal(CrystalType.Red));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal(1, state.Score);
  }

  /// <summary>
  /// Проверяет, что soft margin 8px учитывается в коллизии.
  /// </summary>
  [Xunit.Fact]
  public void Update_RespectsSoftMargin()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.AddCrystal(CreateCrystalAtX(17.0));
    state.AddCrystal(CreateCrystalAtX(18.0));
    state.AddCrystal(CreateCrystalAtX(19.0));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal(16, state.Score);
  }

  /// <summary>
  /// Проверяет рост скорости по уровню.
  /// </summary>
  [Xunit.Fact]
  public void Update_AppliesLevelSpeedMultiplier()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var crystal = new Crystal(
      Guid.NewGuid(),
      new Vector2(0, 0),
      new Vector2(0, 10),
      new Aabb(10, 10),
      CrystalType.Blue);

    state.AddCrystal(crystal);

    service.Update(state, 1.0, 2, bus);

    Xunit.Assert.InRange(crystal.Position.Y, 10.149, 10.151);
  }

  /// <summary>
  /// Проверяет уменьшение энергии при столкновении с астероидом.
  /// </summary>
  [Xunit.Fact]
  public void Update_AsteroidCollisionReducesEnergy()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var asteroid = new Asteroid(
      Guid.NewGuid(),
      new Vector2(0, 0),
      Vector2.Zero,
      new Aabb(10, 10));

    state.AddAsteroid(asteroid);

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal(90, state.Drone.Energy);
  }

  /// <summary>
  /// Проверяет публикацию событий при сборе кристалла.
  /// </summary>
  [Xunit.Fact]
  public void Update_PublishesCrystalCollected()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var collected = 0;

    bus.Subscribe<CrystalCollected>(_ => collected++);

    state.AddCrystal(CreateCrystal(CrystalType.Blue));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal(1, collected);
  }

  /// <summary>
  /// Проверяет публикацию событий при уроне от астероида.
  /// </summary>
  [Xunit.Fact]
  public void Update_PublishesDamageTaken()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var damageEvents = 0;

    bus.Subscribe<DamageTaken>(_ => damageEvents++);

    state.AddAsteroid(new Asteroid(
      Guid.NewGuid(),
      new Vector2(0, 0),
      Vector2.Zero,
      new Aabb(10, 10)));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal(1, damageEvents);
  }

  /// <summary>
  /// Проверяет увеличение очков от ускорителя.
  /// </summary>
  [Xunit.Fact]
  public void Update_AcceleratorIncreasesScore()
  {
    var baseScore = GetScoreWithoutAccelerator();
    var boostedScore = GetScoreWithAccelerator();

    Xunit.Assert.Equal(baseScore * 2, boostedScore);
  }

  /// <summary>
  /// Проверяет сценарий GetScoreWithoutAccelerator.
  /// </summary>
  private static int GetScoreWithoutAccelerator()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.AddCrystal(CreateCrystal(CrystalType.Blue));
    service.Update(state, 1.0 / 60.0, 1, bus);

    return state.Score;
  }

  /// <summary>
  /// Проверяет сценарий GetScoreWithAccelerator.
  /// </summary>
  private static int GetScoreWithAccelerator()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.AddBonus(CreateBonus(BonusType.Accelerator, 5));
    service.Update(state, 1.0 / 60.0, 1, bus);

    state.AddCrystal(CreateCrystal(CrystalType.Blue));
    service.Update(state, 1.0 / 60.0, 1, bus);

    return state.Score;
  }

  /// <summary>
  /// Проверяет ускорение скорости дрона при активном ускорителе.
  /// </summary>
  [Xunit.Fact]
  public void Update_AcceleratorIncreasesDroneSpeed()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    const double acceleratorMultiplier = 1.6;

    state.AddBonus(CreateBonus(BonusType.Accelerator, 5));

    service.Update(state, 1.0 / 60.0, 1, bus);

    state.Drone.Position = Vector2.Zero;
    state.SetDroneMoveDirection(1);

    service.Update(state, 1.0, 1, bus);

    var expected = GameRules.DroneBaseSpeed * acceleratorMultiplier;
    Xunit.Assert.InRange(state.Drone.Position.X, expected - 0.01, expected + 0.01);
  }

  /// <summary>
  /// Проверяет базовую скорость дрона.
  /// </summary>
  [Xunit.Fact]
  public void Update_DroneMovesWithBaseSpeed()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.Drone.Position = Vector2.Zero;
    state.SetDroneMoveDirection(1);

    service.Update(state, 1.0, 1, bus);

    Xunit.Assert.InRange(state.Drone.Position.X, GameRules.DroneBaseSpeed - 0.01, GameRules.DroneBaseSpeed + 0.01);
  }

  /// <summary>
  /// Проверяет ускорение скорости дрона при активном ускорителе от базовой.
  /// </summary>
  [Xunit.Fact]
  public void Update_AcceleratorScalesBaseDroneSpeed()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    const double acceleratorMultiplier = 1.6;

    state.AddBonus(CreateBonus(BonusType.Accelerator, 5));
    service.Update(state, 1.0 / 60.0, 1, bus);

    state.Drone.Position = Vector2.Zero;
    state.SetDroneMoveDirection(1);

    service.Update(state, 1.0, 1, bus);

    var expected = GameRules.DroneBaseSpeed * acceleratorMultiplier;
    Xunit.Assert.InRange(state.Drone.Position.X, expected - 0.01, expected + 0.01);
  }

  /// <summary>
  /// Проверяет ограничение дрона по левой границе.
  /// </summary>
  [Xunit.Fact]
  public void Update_ClampsDroneToLeftBoundary()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var bounds = new WorldBounds(0, 0, 100, 100);
    var state = CreateStateWithDrone(bounds);
    var bus = new EventBus();

    state.Drone.Position = new Vector2(-100, 50);
    state.Drone.Velocity = Vector2.Zero;

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.InRange(state.Drone.Position.X, 4.99, 5.01);
  }

  /// <summary>
  /// Проверяет ограничение дрона по нижней границе.
  /// </summary>
  [Xunit.Fact]
  public void Update_ClampsDroneToBottomBoundary()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var bounds = new WorldBounds(0, 0, 100, 100);
    var state = CreateStateWithDrone(bounds);
    var bus = new EventBus();

    state.Drone.Position = new Vector2(50, 200);
    state.Drone.Velocity = Vector2.Zero;

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.InRange(state.Drone.Position.Y, 94.99, 95.01);
  }

  /// <summary>
  /// Проверяет замедление астероидов стабилизатором времени.
  /// </summary>
  [Xunit.Fact]
  public void Update_TimeStabilizerSlowsAsteroids()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.AddBonus(CreateBonus(BonusType.TimeStabilizer, 5));
    service.Update(state, 1.0 / 60.0, 1, bus);

    var asteroid = new Asteroid(
      Guid.NewGuid(),
      Vector2.Zero,
      new Vector2(0, 10),
      new Aabb(10, 10));
    state.AddAsteroid(asteroid);

    service.Update(state, 1.0, 1, bus);

    Xunit.Assert.InRange(asteroid.Position.Y, 6.49, 6.51);
  }

  /// <summary>
  /// Проверяет добавление времени уровня стабилизатором.
  /// </summary>
  [Xunit.Fact]
  public void Update_TimeStabilizerAddsLevelTime()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var configProvider = new LevelConfigProvider();
    var levelService = new LevelService(configProvider);
    var config = configProvider.GetConfig(1);
    const double timeStabilizerBonus = 5;

    levelService.InitLevel(state);
    state.LevelTimeRemainingSec = config.parLevelTimeSec;
    state.AddBonus(CreateBonus(BonusType.TimeStabilizer, 5));

    service.Update(state, 1.0, 1, bus);

    Xunit.Assert.Equal(config.parLevelTimeSec + timeStabilizerBonus, state.LevelTimeRemainingSec);
  }

  /// <summary>
  /// Проверяет активацию таймера уровня стабилизатором при выключенном таймере.
  /// </summary>
  [Xunit.Fact]
  public void Update_TimeStabilizerActivatesTimerWhenInactive()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.AddBonus(CreateBonus(BonusType.TimeStabilizer, 5));

    service.Update(state, 1.0, 1, bus);

    Xunit.Assert.Equal(65, state.LevelTimeRemainingSec);
  }

  /// <summary>
  /// Проверяет притяжение кристалла магнитом внутри радиуса.
  /// </summary>
  [Xunit.Fact]
  public void Update_MagnetAttractsCrystalInsideRadius()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.AddBonus(CreateBonus(BonusType.Magnet, 5));
    service.Update(state, 1.0 / 60.0, 1, bus);

    var crystal = new Crystal(
      Guid.NewGuid(),
      new Vector2(100, 0),
      new Vector2(5, 0),
      new Aabb(10, 10),
      CrystalType.Blue);
    state.AddCrystal(crystal);

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.True(crystal.Velocity.X < 0);
  }

  /// <summary>
  /// Проверяет усиление скорости кристалла магнитом.
  /// </summary>
  [Xunit.Fact]
  public void Update_MagnetAcceleratesCrystal()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.AddBonus(CreateBonus(BonusType.Magnet, 5));
    service.Update(state, 1.0 / 60.0, 1, bus);

    var crystal = new Crystal(
      Guid.NewGuid(),
      new Vector2(100, 0),
      new Vector2(5, 0),
      new Aabb(10, 10),
      CrystalType.Blue);
    state.AddCrystal(crystal);

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.True(crystal.Velocity.Length() > 5);
  }

  /// <summary>
  /// Проверяет отсутствие влияния магнита вне радиуса.
  /// </summary>
  [Xunit.Fact]
  public void Update_MagnetDoesNotAffectCrystalOutsideRadius()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.AddBonus(CreateBonus(BonusType.Magnet, 5));
    service.Update(state, 1.0 / 60.0, 1, bus);

    var crystal = new Crystal(
      Guid.NewGuid(),
      new Vector2(130, 0),
      new Vector2(5, 0),
      new Aabb(10, 10),
      CrystalType.Blue);
    state.AddCrystal(crystal);

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal(5, crystal.Velocity.X);
  }

  /// <summary>
  /// Проверяет ускорение дрона к центру чёрной дыры.
  /// </summary>
  [Xunit.Fact]
  public void Update_BlackHoleAcceleratesDroneTowardsCenter()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.Drone.Position = new Vector2(10, 0);
    state.AddBlackHole(new BlackHole(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      220,
      20));

    service.Update(state, 1.0, 1, bus);

    Xunit.Assert.True(state.Drone.Velocity.X < 0);
  }

  /// <summary>
  /// Проверяет урон от чёрной дыры внутри ядра.
  /// </summary>
  [Xunit.Fact]
  public void Update_BlackHoleDealsDamageInsideCore()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.Drone.Position = new Vector2(10, 0);
    state.AddBlackHole(new BlackHole(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      220,
      40));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal(60, state.Drone.Energy);
  }

  /// <summary>
  /// Проверяет отсутствие урона от чёрной дыры вне ядра.
  /// </summary>
  [Xunit.Fact]
  public void Update_BlackHoleDoesNotDamageOutsideCore()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.Drone.Position = new Vector2(100, 0);
    state.AddBlackHole(new BlackHole(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      220,
      40));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal(100, state.Drone.Energy);
  }

  /// <summary>
  /// Проверяет, что чёрная дыра двигается вниз по скорости.
  /// </summary>
  [Xunit.Fact]
  public void BlackHole_MovesDown_WhenSpawned()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    var blackHole = new BlackHole(
      Guid.NewGuid(),
      Vector2.Zero,
      new Vector2(0, 10),
      new Aabb(10, 10),
      220,
      40);
    state.AddBlackHole(blackHole);

    service.Update(state, 1.0, 1, bus);

    Xunit.Assert.True(blackHole.Position.Y > 0);
  }

  /// <summary>
  /// Проверяет удаление чёрной дыры при выходе за нижнюю границу.
  /// </summary>
  [Xunit.Fact]
  public void BlackHole_Despawns_WhenOutOfBounds()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var bounds = new WorldBounds(0, 0, 100, 100);
    var state = CreateStateWithDrone(bounds);
    var bus = new EventBus();
    var reason = string.Empty;

    bus.Subscribe<ObjectDespawned>(evt => reason = evt.parReason);

    var blackHole = new BlackHole(
      Guid.NewGuid(),
      new Vector2(50, 106),
      Vector2.Zero,
      new Aabb(10, 10),
      220,
      40);
    state.AddBlackHole(blackHole);

    service.Update(state, 1.0 / 60.0, 1, bus);

    var snapshot = state.GetSnapshot();
    Xunit.Assert.Empty(snapshot.parBlackHoles);
    Xunit.Assert.Equal("OutOfBounds", reason);
  }

  /// <summary>
  /// Проверяет замедление чёрной дыры стабилизатором времени.
  /// </summary>
  [Xunit.Fact]
  public void TimeStabilizer_SlowsDown_BlackHole()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.AddBonus(CreateBonus(BonusType.TimeStabilizer, 5));
    service.Update(state, 1.0 / 60.0, 1, bus);

    var blackHole = new BlackHole(
      Guid.NewGuid(),
      Vector2.Zero,
      new Vector2(0, 10),
      new Aabb(10, 10),
      220,
      40);
    state.AddBlackHole(blackHole);

    service.Update(state, 1.0, 1, bus);

    Xunit.Assert.InRange(blackHole.Position.Y, 6.49, 6.51);
  }

  /// <summary>
  /// Проверяет притяжение кристаллов и бонусов к чёрной дыре.
  /// </summary>
  [Xunit.Fact]
  public void BlackHole_Pulls_Crystals_And_Bonuses_WithinRadius()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    var blackHole = new BlackHole(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      220,
      40);
    state.AddBlackHole(blackHole);

    var crystal = new Crystal(
      Guid.NewGuid(),
      new Vector2(100, 0),
      Vector2.Zero,
      new Aabb(10, 10),
      CrystalType.Blue);
    state.AddCrystal(crystal);

    var bonus = new Bonus(
      Guid.NewGuid(),
      new Vector2(100, 0),
      Vector2.Zero,
      new Aabb(10, 10),
      BonusType.Magnet,
      5);
    state.AddBonus(bonus);

    service.Update(state, 1.0, 1, bus);

    Xunit.Assert.True(crystal.Velocity.X < 0);
    Xunit.Assert.True(bonus.Velocity.X < 0);
  }

  /// <summary>
  /// Проверяет отсутствие притяжения за пределами уменьшенного радиуса.
  /// </summary>
  [Xunit.Fact]
  public void BlackHole_DoesNotPull_WhenOutsideReducedRadius()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var reducedRadius = 220.0 / 1.5;

    var blackHole = new BlackHole(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      reducedRadius,
      40);
    state.AddBlackHole(blackHole);

    var crystal = new Crystal(
      Guid.NewGuid(),
      new Vector2(180, 0),
      Vector2.Zero,
      new Aabb(10, 10),
      CrystalType.Blue);
    state.AddCrystal(crystal);

    service.Update(state, 1.0, 1, bus);

    Xunit.Assert.Equal(0, crystal.Velocity.X);
  }

  /// <summary>
  /// Проверяет, что дезориентация не активируется вне ядра.
  /// </summary>
  [Xunit.Fact]
  public void Update_BlackHoleDoesNotDisorientOutsideCore()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.Drone.Position = new Vector2(100, 0);
    state.AddBlackHole(new BlackHole(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      220,
      40));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.False(state.IsDisoriented);
  }

  /// <summary>
  /// Проверяет, что урон от ядра не применяется многократно.
  /// </summary>
  [Xunit.Fact]
  public void Update_BlackHoleDoesNotRepeatDamageInsideCore()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.Drone.Position = new Vector2(10, 0);
    state.AddBlackHole(new BlackHole(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      220,
      40));

    service.Update(state, 1.0 / 60.0, 1, bus);
    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal(60, state.Drone.Energy);
  }

  /// <summary>
  /// Проверяет отсутствие вертикальной скорости после влияния чёрной дыры.
  /// </summary>
  [Xunit.Fact]
  public void Update_BlackHoleDoesNotChangeDroneVerticalVelocity()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.Drone.Position = new Vector2(10, 0);
    state.AddBlackHole(new BlackHole(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      220,
      40));

    service.Update(state, 1.0, 1, bus);
    Xunit.Assert.Equal(0, state.Drone.Velocity.Y);

    state.Drone.Position = new Vector2(1000, 0);
    service.Update(state, 1.0, 1, bus);
    Xunit.Assert.Equal(0, state.Drone.Velocity.Y);
  }

  /// <summary>
  /// Проверяет удаление кристалла при касании ядра чёрной дыры.
  /// </summary>
  [Xunit.Fact]
  public void BlackHoleCore_RemovesCrystal()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var reason = string.Empty;

    bus.Subscribe<ObjectDespawned>(evt => reason = evt.parReason);

    state.AddBlackHole(new BlackHole(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      220,
      40));

    var crystal = new Crystal(
      Guid.NewGuid(),
      new Vector2(10, 0),
      Vector2.Zero,
      new Aabb(10, 10),
      CrystalType.Blue);
    state.AddCrystal(crystal);

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Empty(state.GetSnapshot().parCrystals);
    Xunit.Assert.Equal("BlackHoleCore", reason);
  }

  /// <summary>
  /// Проверяет удаление бонуса при касании ядра чёрной дыры.
  /// </summary>
  [Xunit.Fact]
  public void BlackHoleCore_RemovesBonus()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var reason = string.Empty;

    bus.Subscribe<ObjectDespawned>(evt => reason = evt.parReason);

    state.AddBlackHole(new BlackHole(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      220,
      40));

    var bonus = new Bonus(
      Guid.NewGuid(),
      new Vector2(10, 0),
      Vector2.Zero,
      new Aabb(10, 10),
      BonusType.Magnet,
      5);
    state.AddBonus(bonus);

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Empty(state.GetSnapshot().parBonuses);
    Xunit.Assert.Equal("BlackHoleCore", reason);
  }

  /// <summary>
  /// Проверяет удаление астероида при касании ядра чёрной дыры.
  /// </summary>
  [Xunit.Fact]
  public void BlackHoleCore_RemovesAsteroid()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var reason = string.Empty;

    bus.Subscribe<ObjectDespawned>(evt => reason = evt.parReason);

    state.AddBlackHole(new BlackHole(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      220,
      40));

    var asteroid = new Asteroid(
      Guid.NewGuid(),
      new Vector2(10, 0),
      Vector2.Zero,
      new Aabb(10, 10));
    state.AddAsteroid(asteroid);

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Empty(state.GetSnapshot().parAsteroids);
    Xunit.Assert.Equal("BlackHoleCore", reason);
  }

  /// <summary>
  /// Проверяет, что пауза останавливает обновление мира.
  /// </summary>
  [Xunit.Fact]
  public void Update_PauseStopsUpdates()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var crystal = new Crystal(
      Guid.NewGuid(),
      Vector2.Zero,
      new Vector2(0, 10),
      new Aabb(10, 10),
      CrystalType.Blue);
    state.AddCrystal(crystal);
    state.TogglePause();

    service.Update(state, 1.0, 1, bus);

    Xunit.Assert.Equal(0, crystal.Position.Y);
    Xunit.Assert.Equal(0, state.Score);
  }

  /// <summary>
  /// Проверяет публикацию события активации бонуса.
  /// </summary>
  [Xunit.Fact]
  public void Update_PublishesBonusActivated()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    string? bonusType = null;

    bus.Subscribe<BonusActivated>(evt => bonusType = evt.parBonusType);

    state.AddBonus(CreateBonus(BonusType.Accelerator, 5));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal(BonusType.Accelerator.ToString(), bonusType);
  }

  /// <summary>
  /// Проверяет публикацию события удаления при сборе кристалла.
  /// </summary>
  [Xunit.Fact]
  public void Update_PublishesObjectDespawnedForCrystal()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var reason = string.Empty;

    bus.Subscribe<ObjectDespawned>(evt => reason = evt.parReason);

    state.AddCrystal(CreateCrystal(CrystalType.Blue));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal("Collected", reason);
  }

  /// <summary>
  /// Проверяет публикацию события удаления при столкновении с астероидом.
  /// </summary>
  [Xunit.Fact]
  public void Update_PublishesObjectDespawnedForAsteroid()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var reason = string.Empty;

    bus.Subscribe<ObjectDespawned>(evt => reason = evt.parReason);

    state.AddAsteroid(new Asteroid(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10)));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal("Collision", reason);
  }

  /// <summary>
  /// Проверяет величину урона от астероида в событии.
  /// </summary>
  [Xunit.Fact]
  public void Update_PublishesDamageTakenAmount()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var amount = 0;

    bus.Subscribe<DamageTaken>(evt => amount = evt.parAmount);

    state.AddAsteroid(new Asteroid(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10)));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal(10, amount);
  }

  /// <summary>
  /// Проверяет, что событие сбора содержит начисленные очки.
  /// </summary>
  [Xunit.Fact]
  public void Update_CrystalCollectedContainsPoints()
  {
    var random = new FakeRandomProvider(9);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var points = 0;

    bus.Subscribe<CrystalCollected>(evt => points = evt.parPoints);

    state.AddCrystal(CreateCrystal(CrystalType.Blue));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal(9, points);
  }

  /// <summary>
  /// Проверяет переход на следующий уровень при достижении целей.
  /// </summary>
  [Xunit.Fact]
  public void Update_LevelCompleted_AdvancesLevelAndResetsProgress()
  {
    var random = new FakeRandomProvider(10);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var completed = 0;
    var configProvider = new LevelConfigProvider();
    var level1 = configProvider.GetConfig(1);
    var level2 = configProvider.GetConfig(2);

    bus.Subscribe<LevelCompleted>(_ => completed++);

    for (int i = 0; i < level1.parRequiredBlue; i++)
    {
      state.AddCrystal(CreateCrystal(CrystalType.Blue));
    }

    for (int i = 0; i < level1.parRequiredGreen; i++)
    {
      state.AddCrystal(CreateCrystal(CrystalType.Green));
    }

    for (int i = 0; i < level1.parRequiredRed; i++)
    {
      state.AddCrystal(CreateCrystal(CrystalType.Red));
    }

    var points = (level1.parRequiredBlue * 10) + (level1.parRequiredGreen * 7) + (level1.parRequiredRed * 4);
    var extraBlue = Math.Max(0, (int)Math.Ceiling((level1.parRequiredScore - points) / 10.0));

    for (int i = 0; i < extraBlue; i++)
    {
      state.AddCrystal(CreateCrystal(CrystalType.Blue));
    }

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal(2, state.CurrentLevel);
    Xunit.Assert.Equal(level2.parRequiredBlue, state.LevelGoals.RequiredBlue);
    Xunit.Assert.Equal(level2.parRequiredGreen, state.LevelGoals.RequiredGreen);
    Xunit.Assert.Equal(level2.parRequiredRed, state.LevelGoals.RequiredRed);
    Xunit.Assert.Equal(level2.parRequiredScore, state.RequiredScore);
    Xunit.Assert.Equal(0, state.LevelProgress.CollectedBlue);
    Xunit.Assert.Equal(0, state.LevelProgress.CollectedGreen);
    Xunit.Assert.Equal(0, state.LevelProgress.CollectedRed);
    Xunit.Assert.True(state.LevelTimeRemainingSec > 0);
    Xunit.Assert.Equal(1, completed);
  }

  /// <summary>
  /// Проверяет поражение при окончании времени уровня.
  /// </summary>
  [Xunit.Fact]
  public void Update_GameOverWhenTimeExpired()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var gameOver = 0;

    state.LevelTimeRemainingSec = 0;

    bus.Subscribe<GameOver>(_ => gameOver++);

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.True(state.IsGameOver);
    Xunit.Assert.Equal(1, gameOver);
  }

  /// <summary>
  /// Проверяет поражение при нулевой энергии.
  /// </summary>
  [Xunit.Fact]
  public void Update_GameOverWhenEnergyDepleted()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = new GameState(
      new Drone(Guid.NewGuid(), Vector2.Zero, Vector2.Zero, new Aabb(10, 10), 0),
      new WorldBounds(0, 0, 800, 600));
    var bus = new EventBus();
    var gameOver = 0;

    state.LevelTimeRemainingSec = 100;

    bus.Subscribe<GameOver>(_ => gameOver++);

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.True(state.IsGameOver);
    Xunit.Assert.Equal(1, gameOver);
  }

  /// <summary>
  /// Проверяет удаление объекта ниже нижней границы.
  /// </summary>
  [Xunit.Fact]
  public void Update_RemovesObjectBelowBottom()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var bounds = new WorldBounds(0, 0, 100, 100);
    var state = CreateStateWithDrone(bounds);
    var bus = new EventBus();
    var despawned = 0;

    bus.Subscribe<ObjectDespawned>(_ => despawned++);

    state.AddCrystal(new Crystal(
      Guid.NewGuid(),
      new Vector2(0, 106),
      Vector2.Zero,
      new Aabb(10, 10),
      CrystalType.Blue));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Empty(state.GetSnapshot().parCrystals);
    Xunit.Assert.Equal(1, despawned);
  }

  /// <summary>
  /// Проверяет, что объект на границе не удаляется.
  /// </summary>
  [Xunit.Fact]
  public void Update_DoesNotRemoveObjectOnBottomEdge()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var bounds = new WorldBounds(0, 0, 100, 100);
    var state = CreateStateWithDrone(bounds);
    var bus = new EventBus();

    state.AddCrystal(new Crystal(
      Guid.NewGuid(),
      new Vector2(0, 105),
      Vector2.Zero,
      new Aabb(10, 10),
      CrystalType.Blue));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Single(state.GetSnapshot().parCrystals);
  }

  /// <summary>
  /// Проверяет причину удаления объекта за пределами границы.
  /// </summary>
  [Xunit.Fact]
  public void Update_PublishesOutOfBoundsDespawnReason()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var bounds = new WorldBounds(0, 0, 100, 100);
    var state = CreateStateWithDrone(bounds);
    var bus = new EventBus();
    var reason = string.Empty;

    bus.Subscribe<ObjectDespawned>(evt => reason = evt.parReason);

    state.AddCrystal(new Crystal(
      Guid.NewGuid(),
      new Vector2(0, 106),
      Vector2.Zero,
      new Aabb(10, 10),
      CrystalType.Blue));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Equal("OutOfBounds", reason);
  }

  /// <summary>
  /// Проверяет запуск отсчёта при снятии паузы и отсутствие движения мира.
  /// </summary>
  [Xunit.Fact]
  public void Update_ResumeCountdownPreventsWorldUpdate()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var crystal = new Crystal(
      Guid.NewGuid(),
      Vector2.Zero,
      new Vector2(0, 10),
      new Aabb(10, 10),
      CrystalType.Blue);
    state.AddCrystal(crystal);

    state.TogglePause();
    state.TogglePause();

    for (var i = 0; i < 180; i++)
    {
      service.Update(state, 1.0 / 60.0, 1, bus);
    }

    Xunit.Assert.False(state.IsPaused);
    Xunit.Assert.Equal(0, crystal.Position.Y);

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.False(state.IsPaused);
    Xunit.Assert.InRange(crystal.Position.Y, 0.16, 0.17);
  }

  /// <summary>
  /// Проверяет последовательность событий отсчёта 3..1.
  /// </summary>
  [Xunit.Fact]
  public void Update_ResumeCountdownPublishesTicks()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();
    var ticks = new List<int>();

    bus.Subscribe<CountdownTick>(evt => ticks.Add(evt.parValue));

    state.TogglePause();
    state.TogglePause();

    for (var i = 0; i < 180; i++)
    {
      service.Update(state, 1.0 / 60.0, 1, bus);
    }

    Xunit.Assert.Equal(new[] { 3, 2, 1 }, ticks);
  }

  /// <summary>
  /// Проверяет снятие паузы после завершения отсчёта.
  /// </summary>
  [Xunit.Fact]
  public void Update_ResumeCountdownFinishesAndUnpauses()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.TogglePause();
    state.TogglePause();

    for (var i = 0; i < 180; i++)
    {
      service.Update(state, 1.0 / 60.0, 1, bus);
    }

    Xunit.Assert.False(state.IsPaused);
  }

  /// <summary>
  /// Проверяет включение дезориентации при попадании в радиус чёрной дыры.
  /// </summary>
  [Xunit.Fact]
  public void Update_BlackHoleEnablesDisorientation()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.Drone.Position = new Vector2(10, 0);
    state.AddBlackHole(new BlackHole(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      220,
      40));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.True(state.IsDisoriented);
    Xunit.Assert.InRange(state.DisorientationRemainingSec, 2.9, 3.0);
  }

  /// <summary>
  /// Проверяет окончание дезориентации после заданного времени.
  /// </summary>
  [Xunit.Fact]
  public void Update_DisorientationExpiresAfterDuration()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = CreateStateWithDrone();
    var bus = new EventBus();

    state.Drone.Position = new Vector2(10, 0);
    state.AddBlackHole(new BlackHole(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      220,
      40));

    service.Update(state, 1.0 / 60.0, 1, bus);

    state.Drone.Position = new Vector2(1000, 0);

    for (var i = 0; i < 180; i++)
    {
      service.Update(state, 1.0 / 60.0, 1, bus);
    }

    Xunit.Assert.False(state.IsDisoriented);
    Xunit.Assert.Equal(0, state.DisorientationRemainingSec);
  }

  /// <summary>
  /// Проверяет сценарий CreateStateWithDrone.
  /// </summary>
  private static GameState CreateStateWithDrone()
  {
    var drone = new Drone(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      100);

    return new GameState(drone, new WorldBounds(0, 0, 800, 600));
  }

  /// <summary>
  /// Проверяет сценарий CreateStateWithDrone.
  /// </summary>
  private static GameState CreateStateWithDrone(WorldBounds parBounds)
  {
    var drone = new Drone(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      100);

    return new GameState(drone, parBounds);
  }

  /// <summary>
  /// Проверяет сценарий CreateCrystal.
  /// </summary>
  private static Crystal CreateCrystal(CrystalType parType)
  {
    return new Crystal(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      parType);
  }

  /// <summary>
  /// Проверяет сценарий CreateCrystalAtX.
  /// </summary>
  private static Crystal CreateCrystalAtX(double parX)
  {
    return new Crystal(
      Guid.NewGuid(),
      new Vector2(parX, 0),
      Vector2.Zero,
      new Aabb(10, 10),
      CrystalType.Blue);
  }

  /// <summary>
  /// Проверяет сценарий CreateBonus.
  /// </summary>
  private static Bonus CreateBonus(BonusType parType, double parDurationSec)
  {
    return new Bonus(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      parType,
      parDurationSec);
  }
}
