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
  public void OnEnter(IGameFlowRuntime parRuntime)
  {
    parRuntime.GameState.SetDroneMoveDirection(0);
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
        parRuntime.TransitionTo(new ResumeCountdownState());
        return;
      case BackToMenuCommand:
        parRuntime.TransitionTo(new ConfirmSaveBeforeMenuState());
        return;
      case SetMoveDirectionCommand:
      case MoveLeftCommand:
      case MoveRightCommand:
        parRuntime.GameState.SetDroneMoveDirection(0);
        return;
    }
  }

  /// <inheritdoc />
  public void OnTick(IGameFlowRuntime parRuntime, double parDt)
  {
  }
}
