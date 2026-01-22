using CosmicCollector.Core.Events;
using CosmicCollector.MVC.Commands;

namespace CosmicCollector.MVC.Flow;

/// <summary>
/// Состояние паузы.
/// </summary>
public sealed class PausedState : IGameFlowState
{
  /// <inheritdoc />
  public string Name => "Paused";

  /// <inheritdoc />
  public bool AllowsWorldTick => false;

  /// <inheritdoc />
  public void OnEnter(IGameFlowContext parContext)
  {
  }

  /// <inheritdoc />
  public void OnExit(IGameFlowContext parContext)
  {
  }

  public void HandleCommand(IGameFlowContext parContext, IGameCommand parCommand)
  {
    if (parCommand is TogglePauseCommand)
    {
      var isPaused = parContext.GameState.TogglePause();
      parContext.EventBus.Publish(new PauseToggled(isPaused));
      parContext.TransitionTo(new ResumeCountdownState());
      return;
    }

    if (parCommand is SetMoveDirectionCommand setMoveDirectionCommand)
    {
      ApplyMoveCommand(parContext, setMoveDirectionCommand.DirectionX);
      return;
    }

    if (parCommand is MoveLeftCommand)
    {
      ApplyMoveCommand(parContext, -1);
      return;
    }

    if (parCommand is MoveRightCommand)
    {
      ApplyMoveCommand(parContext, 1);
      return;
    }

    if (parCommand is RequestBackToMenuCommand)
    {
      parContext.TransitionTo(new MenuState());
    }
  }

  private static void ApplyMoveCommand(IGameFlowContext parContext, int parDirection)
  {
    parContext.GameState.SetDroneMoveDirection(parDirection);
  }
}
