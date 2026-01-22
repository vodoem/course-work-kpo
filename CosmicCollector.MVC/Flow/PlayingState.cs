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
  public void OnEnter(IGameFlowRuntime parRuntime)
  {
  }

  /// <inheritdoc />
  public void OnExit(IGameFlowRuntime parRuntime)
  {
  }

  /// <inheritdoc />
  public void HandleCommand(IGameFlowRuntime parRuntime, IGameCommand parCommand)
  {
    switch (parCommand)
    {
      case TogglePauseCommand:
        var isPaused = parRuntime.GameState.TogglePause();
        parRuntime.EventPublisher.Publish(new PauseToggled(isPaused));
        parRuntime.TransitionTo(new PausedState());
        return;
      case SetMoveDirectionCommand setMoveDirectionCommand:
        parRuntime.GameState.SetDroneMoveDirection(setMoveDirectionCommand.DirectionX);
        return;
      case MoveLeftCommand:
        parRuntime.GameState.SetDroneMoveDirection(-1);
        return;
      case MoveRightCommand:
        parRuntime.GameState.SetDroneMoveDirection(1);
        return;
    }
  }

  /// <inheritdoc />
  public void OnTick(IGameFlowRuntime parRuntime, double parDt)
  {
  }
}
