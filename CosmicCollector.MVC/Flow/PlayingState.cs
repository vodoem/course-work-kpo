using CosmicCollector.Core.Events;
using CosmicCollector.MVC.Commands;

namespace CosmicCollector.MVC.Flow;

/// <summary>
/// Состояние активной игры.
/// </summary>
public sealed class PlayingState : IGameFlowState
{
  /// <inheritdoc />
  public string Name => "Playing";

  /// <inheritdoc />
  public bool AllowsWorldTick => true;

  /// <inheritdoc />
  public void OnEnter(IGameFlowContext parContext)
  {
  }

  /// <inheritdoc />
  public void OnExit(IGameFlowContext parContext)
  {
  }

  /// <inheritdoc />
  public void HandleCommand(IGameFlowContext parContext, IGameCommand parCommand)
  {
    if (parCommand is TogglePauseCommand)
    {
      var isPaused = parContext.GameState.TogglePause();
      parContext.EventBus.Publish(new PauseToggled(isPaused));
      parContext.TransitionTo(new PausedState());
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
    }
  }

  private static void ApplyMoveCommand(IGameFlowContext parContext, int parDirection)
  {
    parContext.GameState.SetDroneMoveDirection(parDirection);
  }
}
