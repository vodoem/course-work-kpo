using CosmicCollector.MVC.Commands;

namespace CosmicCollector.MVC.Flow;

/// <summary>
/// Состояние обратного отсчёта после паузы.
/// </summary>
public sealed class ResumeCountdownState : IGameFlowState
{
  /// <inheritdoc />
  public string Name => "ResumeCountdown";

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
    if (parRuntime.ProcessResumeCountdown(parDt))
    {
      parRuntime.TransitionTo(new PlayingState());
    }
  }
}
