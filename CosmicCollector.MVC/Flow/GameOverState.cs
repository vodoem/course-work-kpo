using CosmicCollector.MVC.Commands;

namespace CosmicCollector.MVC.Flow;

/// <summary>
/// Состояние завершения игры.
/// </summary>
public sealed class GameOverState : IGameFlowState
{
  /// <inheritdoc />
  public string Name => "GameOver";

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

  /// <inheritdoc />
  public void HandleCommand(IGameFlowContext parContext, IGameCommand parCommand)
  {
    if (parCommand is RequestBackToMenuCommand)
    {
      parContext.TransitionTo(new MenuState(false));
    }
  }
}
