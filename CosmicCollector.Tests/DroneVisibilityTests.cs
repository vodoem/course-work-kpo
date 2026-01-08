using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.Core.Snapshots;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет видимость дрона на игровом поле.
/// </summary>
public sealed class DroneVisibilityTests
{
  /// <summary>
  /// Проверяет, что дрон после инициализации попадает в видимую область поля.
  /// </summary>
  [Xunit.Fact]
  public void Drone_IsVisible_WithDefaultSession()
  {
    var session = new GameSessionFactory().Create();
    GameSnapshot snapshot = session.SnapshotProvider.GetSnapshot();
    var mapper = new WorldToScreenMapper(session.WorldBounds, 40, 20, RenderConfig.PixelsPerCell);

    int x = mapper.MapX(snapshot.parDrone.parPosition.X);
    int y = mapper.MapY(snapshot.parDrone.parPosition.Y);

    Xunit.Assert.InRange(x, 0, 39);
    Xunit.Assert.InRange(y, 0, 19);
  }
}
