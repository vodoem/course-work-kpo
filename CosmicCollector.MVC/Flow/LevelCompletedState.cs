using CosmicCollector.MVC.Commands;

namespace CosmicCollector.MVC.Flow;

/// <summary>
/// Состояние завершения уровня.
/// </summary>
public sealed class LevelCompletedState : IGameFlowState
{
  /// <inheritdoc />
  public string Name => "LevelCompleted";

  /// <inheritdoc />
  public bool AllowsWorldTick => true;

  /// <inheritdoc />
  public void OnEnter(IGameFlowContext parContext)
  {
    parContext.TransitionTo(new PlayingState());
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
      parContext.TransitionTo(new MenuState());
    }
  }
}
