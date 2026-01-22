using System;
using System.Collections.Generic;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Events;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Randomization;
using CosmicCollector.Core.Services;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Flow;
using CosmicCollector.MVC.Loop;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет паузу и обратный отсчёт.
/// </summary>
public sealed class PauseCountdownTests
{
  /// <summary>
  /// Проверяет, что второе нажатие паузы запускает отсчёт и снимает паузу.
  /// </summary>
  [Xunit.Fact]
  public void TogglePause_Twice_ResumesAfterCountdown()
  {
    var state = CreateState();
    var service = CreateService();
    var bus = new EventBus();
    var queue = new CommandQueue();
    var toggles = new List<bool>();
    var countdownValues = new List<int>();

    bus.Subscribe<PauseToggled>(evt => toggles.Add(evt.parIsPaused));
    bus.Subscribe<CountdownTick>(evt => countdownValues.Add(evt.parValue));

    var runtime = new TestGameFlowRuntime(state, bus, service, new ResumeCountdownService(), new PlayingState());
    var runner = new ManualGameLoopRunner(state, queue, bus, runtime.Tick, runtime);

    queue.Enqueue(new TogglePauseCommand());
    runner.TickOnce();

    Xunit.Assert.True(state.IsPaused);

    queue.Enqueue(new TogglePauseCommand());

    for (int i = 0; i < 180; i++)
    {
      runner.TickOnce();
    }

    Xunit.Assert.False(state.IsPaused);
    Xunit.Assert.Equal(new[] { 3, 2, 1 }, countdownValues);
    Xunit.Assert.Contains(false, toggles);
  }

  /// <summary>
  /// Проверяет, что таймер не уменьшается во время паузы.
  /// </summary>
  [Xunit.Fact]
  public void Timer_DoesNotDecrease_WhenPaused()
  {
    var state = CreateState();
    var service = CreateService();
    var bus = new EventBus();
    var queue = new CommandQueue();
    var runtime = new TestGameFlowRuntime(state, bus, service, new ResumeCountdownService(), new PlayingState());
    var runner = new ManualGameLoopRunner(state, queue, bus, runtime.Tick, runtime);

    state.LevelTimeRemainingSec = 10;
    queue.Enqueue(new TogglePauseCommand());
    runner.TickOnce();

    for (int i = 0; i < 60; i++)
    {
      runner.TickOnce();
    }

    Xunit.Assert.Equal(10, state.LevelTimeRemainingSec);
  }

  /// <summary>
  /// Проверяет, что таймер не уменьшается во время обратного отсчёта.
  /// </summary>
  [Xunit.Fact]
  public void Timer_DoesNotDecrease_DuringCountdown()
  {
    var state = CreateState();
    var service = CreateService();
    var bus = new EventBus();
    var queue = new CommandQueue();
    var runtime = new TestGameFlowRuntime(state, bus, service, new ResumeCountdownService(), new PlayingState());
    var runner = new ManualGameLoopRunner(state, queue, bus, runtime.Tick, runtime);

    state.LevelTimeRemainingSec = 10;
    queue.Enqueue(new TogglePauseCommand());
    runner.TickOnce();

    queue.Enqueue(new TogglePauseCommand());

    for (int i = 0; i < 120; i++)
    {
      runner.TickOnce();
    }

    Xunit.Assert.Equal(10, state.LevelTimeRemainingSec);
    Xunit.Assert.True(state.IsPaused);
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
