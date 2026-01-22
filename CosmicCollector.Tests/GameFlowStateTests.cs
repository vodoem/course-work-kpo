using CosmicCollector.Core.Events;
using CosmicCollector.Core.Model;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Flow;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет переходы игровых состояний.
/// </summary>
public sealed class GameFlowStateTests
{
  [Xunit.Fact]
  public void Pause_To_ResumeCountdown_To_Playing()
  {
    var state = new GameState();
    var bus = new EventBus();
    var controller = new GameFlowController(state, bus, new PlayingState());

    controller.HandleCommand(new TogglePauseCommand());

    Xunit.Assert.True(state.IsPaused);
    Xunit.Assert.IsType<PausedState>(controller.CurrentState);

    controller.HandleCommand(new TogglePauseCommand());

    Xunit.Assert.IsType<ResumeCountdownState>(controller.CurrentState);

    bus.Publish(new PauseToggled(false));

    Xunit.Assert.IsType<PlayingState>(controller.CurrentState);
  }

  [Xunit.Fact]
  public void Pause_To_Menu()
  {
    var state = new GameState();
    var bus = new EventBus();
    var controller = new GameFlowController(state, bus, new PlayingState());

    controller.HandleCommand(new TogglePauseCommand());
    controller.HandleCommand(new RequestBackToMenuCommand());

    Xunit.Assert.IsType<MenuState>(controller.CurrentState);
  }

  [Xunit.Fact]
  public void LevelCompleted_TransitionsToPlaying()
  {
    var state = new GameState();
    var bus = new EventBus();
    var controller = new GameFlowController(state, bus, new PlayingState());

    bus.Publish(new LevelCompleted("GoalsAndScore"));

    Xunit.Assert.IsType<PlayingState>(controller.CurrentState);
  }
}
