using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Randomization;
using CosmicCollector.Core.Services;
using CosmicCollector.MVC.Eventing;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет управление дроном и паузу.
/// </summary>
public sealed class DroneControlTests
{
  /// <summary>
  /// Проверяет, что при удержании направления X уменьшается каждый тик.
  /// </summary>
  [Xunit.Fact]
  public void Update_DroneMovesLeftWhileHeld()
  {
    var state = CreateState();
    var service = CreateService();
    var bus = new EventBus();

    state.Drone.Position = new Vector2(400, 300);
    state.SetDroneMoveDirection(-1);

    var startX = state.Drone.Position.X;
    service.Update(state, 1.0 / 60.0, 1, bus);
    var afterFirst = state.Drone.Position.X;
    service.Update(state, 1.0 / 60.0, 1, bus);
    var afterSecond = state.Drone.Position.X;

    Xunit.Assert.True(afterFirst < startX);
    Xunit.Assert.True(afterSecond < afterFirst);
  }

  /// <summary>
  /// Проверяет, что при отпускании направления движение прекращается.
  /// </summary>
  [Xunit.Fact]
  public void Update_DroneStopsAfterRelease()
  {
    var state = CreateState();
    var service = CreateService();
    var bus = new EventBus();

    state.Drone.Position = new Vector2(400, 300);
    state.SetDroneMoveDirection(-1);
    service.Update(state, 1.0 / 60.0, 1, bus);

    state.SetDroneMoveDirection(0);
    var stopX = state.Drone.Position.X;
    service.Update(state, 1.0 / 60.0, 1, bus);
    var afterStop = state.Drone.Position.X;

    Xunit.Assert.InRange(afterStop, stopX - 0.0001, stopX + 0.0001);
  }

  /// <summary>
  /// Проверяет, что двойное переключение паузы возвращает игру к активному состоянию.
  /// </summary>
  [Xunit.Fact]
  public void TogglePause_Twice_ReturnsToRunning()
  {
    var state = CreateState();
    var service = CreateService();
    var bus = new EventBus();

    state.TogglePause();
    state.TogglePause();

    service.Update(state, 1.0, 1, bus);
    service.Update(state, 1.0, 1, bus);
    service.Update(state, 1.0, 1, bus);

    Xunit.Assert.False(state.IsPaused);
  }

  /// <summary>
  /// Проверяет, что таймер уровня не уменьшается на паузе.
  /// </summary>
  [Xunit.Fact]
  public void Update_DoesNotDecreaseTimerWhilePaused()
  {
    var state = CreateState();
    var service = CreateService();
    var bus = new EventBus();

    state.LevelTimeRemainingSec = 10;
    state.TogglePause();

    service.Update(state, 1.0, 1, bus);

    Xunit.Assert.Equal(10, state.LevelTimeRemainingSec);
  }

  /// <summary>
  /// Проверяет сценарий CreateState.
  /// </summary>
  private static GameState CreateState()
  {
    return new GameState(
      new Drone(Guid.NewGuid(), Vector2.Zero, Vector2.Zero, new Aabb(10, 10), 100),
      new WorldBounds(0, 0, 800, 600));
  }

  /// <summary>
  /// Проверяет сценарий CreateService.
  /// </summary>
  private static GameWorldUpdateService CreateService()
  {
    return new GameWorldUpdateService(new FakeRandomProvider(1));
  }
}
