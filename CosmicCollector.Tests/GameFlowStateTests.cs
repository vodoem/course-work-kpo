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

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет переходы состояний игрового потока.
/// </summary>
public sealed class GameFlowStateTests
{
  [Xunit.Fact]
  public void PauseToResumeCountdownToPlaying_TransitionsAndPublishesCountdown()
  {
    var state = new GameState(
      new Drone(Guid.NewGuid(), Vector2.Zero, Vector2.Zero, new Aabb(10, 10), 100),
      new WorldBounds(0, 0, 800, 600));
    var bus = new EventBus();
    var service = new GameWorldUpdateService(new FakeRandomProvider(1));
    var runtime = new TestGameFlowRuntime(state, bus, service, new ResumeCountdownService(), new PlayingState());
    var countdownValues = new List<int>();

    bus.Subscribe<CountdownTick>(evt => countdownValues.Add(evt.parValue));

    runtime.HandleCommand(new TogglePauseCommand());
    Xunit.Assert.IsType<PausedState>(runtime.CurrentState);

    runtime.HandleCommand(new TogglePauseCommand());
    Xunit.Assert.IsType<ResumeCountdownState>(runtime.CurrentState);

    for (var i = 0; i < 180; i++)
    {
      runtime.Tick(1.0 / 60.0);
    }

    Xunit.Assert.IsType<PlayingState>(runtime.CurrentState);
    Xunit.Assert.False(state.IsPaused);
    Xunit.Assert.Contains(3, countdownValues);
    Xunit.Assert.Contains(2, countdownValues);
    Xunit.Assert.Contains(1, countdownValues);
  }

  [Xunit.Fact]
  public void PauseToConfirmSaveToMenu_PublishesMenuExitRequest()
  {
    var state = new GameState(
      new Drone(Guid.NewGuid(), Vector2.Zero, Vector2.Zero, new Aabb(10, 10), 100),
      new WorldBounds(0, 0, 800, 600));
    var bus = new EventBus();
    var service = new GameWorldUpdateService(new FakeRandomProvider(2));
    var runtime = new TestGameFlowRuntime(state, bus, service, new ResumeCountdownService(), new PlayingState());
    var exitRequests = new List<MenuExitRequested>();
    var dialogsOpened = new List<ConfirmSaveDialogOpened>();
    var dialogsClosed = new List<ConfirmSaveDialogClosed>();

    bus.Subscribe<MenuExitRequested>(evt => exitRequests.Add(evt));
    bus.Subscribe<ConfirmSaveDialogOpened>(evt => dialogsOpened.Add(evt));
    bus.Subscribe<ConfirmSaveDialogClosed>(evt => dialogsClosed.Add(evt));

    runtime.HandleCommand(new TogglePauseCommand());
    Xunit.Assert.IsType<PausedState>(runtime.CurrentState);

    runtime.HandleCommand(new BackToMenuCommand());
    Xunit.Assert.IsType<ConfirmSaveBeforeMenuState>(runtime.CurrentState);
    Xunit.Assert.Single(dialogsOpened);

    runtime.HandleCommand(new ConfirmSaveYesCommand());

    Xunit.Assert.IsType<MenuState>(runtime.CurrentState);
    Xunit.Assert.Single(exitRequests);
    Xunit.Assert.True(exitRequests[0].parShouldSave);
    Xunit.Assert.Single(dialogsClosed);
  }
}
