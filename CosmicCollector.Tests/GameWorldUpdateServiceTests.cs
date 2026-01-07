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

  private static GameState CreateStateWithDrone()
  {
    var drone = new Drone(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      100);

    return new GameState(drone);
  }

  private static Crystal CreateCrystal(CrystalType parType)
  {
    return new Crystal(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      parType);
  }

  private static Crystal CreateCrystalAtX(double parX)
  {
    return new Crystal(
      Guid.NewGuid(),
      new Vector2(parX, 0),
      Vector2.Zero,
      new Aabb(10, 10),
      CrystalType.Blue);
  }
}
