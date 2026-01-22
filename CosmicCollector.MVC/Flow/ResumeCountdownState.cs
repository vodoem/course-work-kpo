using CosmicCollector.MVC.Commands;

namespace CosmicCollector.MVC.Flow;

/// <summary>
/// Состояние обратного отсчёта перед продолжением игры.
/// </summary>
public sealed class ResumeCountdownState : IGameFlowState
{
  /// <inheritdoc />
  public string Name => "ResumeCountdown";

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
