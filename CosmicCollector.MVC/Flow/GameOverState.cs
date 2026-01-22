using CosmicCollector.MVC.Commands;

namespace CosmicCollector.MVC.Flow;

/// <summary>
/// Состояние окончания игры.
/// </summary>
public sealed class GameOverState : IGameFlowState
{
  /// <inheritdoc />
  public string Name => "GameOver";

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
  }

  /// <inheritdoc />
  public void OnTick(IGameFlowRuntime parRuntime, double parDt)
  {
  }
}
