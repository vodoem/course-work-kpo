using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Events;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Randomization;
using CosmicCollector.Core.Services;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Loop;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет работу игрового цикла и снапшотов.
/// </summary>
public sealed class GameLoopTests
{
  /// <summary>
  /// Проверяет, что игровой цикл выполняет заданное число тиков.
  /// </summary>
  [Xunit.Fact]
  public void ManualRunner_ProducesTicks()
  {
    var state = new GameState();
    var queue = new CommandQueue();
    var bus = new EventBus();
    var ticks = 0;

    bus.Subscribe<GameTick>(_ => ticks++);

    var runner = new ManualGameLoopRunner(state, queue, bus, _ => { });

    runner.TickOnce();
    runner.TickOnce();
    runner.TickOnce();

    Xunit.Assert.Equal(3, ticks);
  }

  /// <summary>
  /// Проверяет, что снимок состояния доступен при параллельных обновлениях.
  /// </summary>
  [Xunit.Fact]
  public async Task GetSnapshot_IsSafeDuringUpdates()
  {
    var state = new GameState();
    var queue = new CommandQueue();
    var bus = new EventBus();
    var runner = new ManualGameLoopRunner(state, queue, bus, _ => { });

    var updateTask = Task.Run(() =>
    {
      for (var i = 0; i < 100; i++)
      {
        runner.TickOnce();
      }
    });

    for (var i = 0; i < 100; i++)
    {
      var snapshot = state.GetSnapshot();
      Xunit.Assert.True(snapshot.parTickNo >= 0);
    }

    await updateTask;
  }

  /// <summary>
  /// Проверяет, что команда паузы переключает состояние и публикует событие.
  /// </summary>
  [Xunit.Fact]
  public void TogglePauseCommand_PublishesPauseToggled()
  {
    var state = new GameState();
    var queue = new CommandQueue();
    var bus = new EventBus();
    var toggledValues = new List<bool>();

    bus.Subscribe<PauseToggled>(evt => toggledValues.Add(evt.parIsPaused));

    var runner = new ManualGameLoopRunner(state, queue, bus, _ => { });

    queue.Enqueue(new TogglePauseCommand());
    runner.TickOnce();

    Xunit.Assert.True(state.IsPaused);
    Xunit.Assert.Single(toggledValues);
    Xunit.Assert.True(toggledValues[0]);
  }

  /// <summary>
  /// Проверяет инверсию команд движения при дезориентации.
  /// </summary>
  [Xunit.Fact]
  public void ProcessCommands_InvertsMoveWhenDisoriented()
  {
    var random = new FakeRandomProvider(8);
    var service = new GameWorldUpdateService(random);
    var state = new GameState(
      new Drone(Guid.NewGuid(), Vector2.Zero, Vector2.Zero, new Aabb(10, 10), 100),
      new WorldBounds(0, 0, 800, 600));
    var queue = new CommandQueue();
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

    var runner = new ManualGameLoopRunner(state, queue, bus, dt => service.Update(state, dt, 1, bus));
    var startX = state.Drone.Position.X;

    queue.Enqueue(new SetMoveDirectionCommand(-1));
    runner.TickOnce();

    Xunit.Assert.True(state.Drone.Position.X > startX);
  }
}
