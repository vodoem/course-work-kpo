using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Services;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет фабрику игровых объектов по умолчанию.
/// </summary>
public sealed class GameObjectFactoryTests
{
  /// <summary>
  /// Проверяет создание кристалла с ожидаемыми параметрами.
  /// </summary>
  [Xunit.Fact]
  public void Factory_CreatesCrystalWithExpectedValues()
  {
    var factory = new DefaultGameObjectFactory();
    var id = Guid.NewGuid();
    var position = new Vector2(1, 2);
    var velocity = new Vector2(3, 4);
    var bounds = new Aabb(5, 6);

    var crystal = factory.CreateCrystal(id, position, velocity, bounds, CrystalType.Green);

    Xunit.Assert.Equal(id, crystal.Id);
    Xunit.Assert.Equal(position, crystal.Position);
    Xunit.Assert.Equal(velocity, crystal.Velocity);
    Xunit.Assert.Equal(bounds, crystal.Bounds);
    Xunit.Assert.Equal(CrystalType.Green, crystal.Type);
  }

  /// <summary>
  /// Проверяет создание чёрной дыры с ожидаемыми параметрами.
  /// </summary>
  [Xunit.Fact]
  public void Factory_CreatesBlackHoleWithExpectedValues()
  {
    var factory = new DefaultGameObjectFactory();
    var id = Guid.NewGuid();
    var position = new Vector2(7, 8);
    var velocity = new Vector2(0, 0);
    var bounds = new Aabb(9, 10);
    var radius = 220.0;
    var coreRadius = 40.0;

    var blackHole = factory.CreateBlackHole(id, position, velocity, bounds, radius, coreRadius);

    Xunit.Assert.Equal(id, blackHole.Id);
    Xunit.Assert.Equal(position, blackHole.Position);
    Xunit.Assert.Equal(velocity, blackHole.Velocity);
    Xunit.Assert.Equal(bounds, blackHole.Bounds);
    Xunit.Assert.Equal(radius, blackHole.Radius);
    Xunit.Assert.Equal(coreRadius, blackHole.CoreRadius);
  }
}
