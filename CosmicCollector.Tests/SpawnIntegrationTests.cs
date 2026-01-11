using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Events;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Randomization;
using CosmicCollector.Core.Services;
using CosmicCollector.MVC.Eventing;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет интеграцию спавна в обновление мира.
/// </summary>
public sealed class SpawnIntegrationTests
{
  /// <summary>
  /// Проверяет публикацию событий при спавне.
  /// </summary>
  [Xunit.Fact]
  public void Spawn_PublishesObjectSpawned()
  {
    var random = new FakeRandomProvider(new[] { 20, 1 });
    var config = CreateConfig();
    var service = new GameWorldUpdateService(random, config);
    var state = new GameState(
      new Drone(Guid.NewGuid(), Vector2.Zero, Vector2.Zero, new Aabb(10, 10), 100),
      new WorldBounds(0, 0, 100, 100));
    var bus = new EventBus();
    var spawned = new List<ObjectSpawned>();

    bus.Subscribe<ObjectSpawned>(evt => spawned.Add(evt));

    service.Update(state, 1.0 / 60.0, 1, bus);

    Xunit.Assert.Single(spawned);
    Xunit.Assert.Equal("Crystal", spawned[0].parObjectType);
  }

  /// <summary>
  /// Проверяет сценарий CreateConfig.
  /// </summary>
  private static SpawnConfig CreateConfig()
  {
    return new SpawnConfig(
      true,
      3,
      0,
      0,
      0,
      1,
      0,
      0,
      0,
      0,
      0,
      1,
      2,
      3,
      new Aabb(10, 10),
      new Aabb(10, 10),
      new Aabb(10, 10),
      new Aabb(10, 10),
      4.0,
      6.0,
      3.0,
      0,
      0,
      0,
      5.0,
      1.2,
      new List<WeightedOption<CrystalType>> { new(CrystalType.Blue, 1) },
      new List<WeightedOption<BonusType>> { new(BonusType.Accelerator, 1) });
  }
}
