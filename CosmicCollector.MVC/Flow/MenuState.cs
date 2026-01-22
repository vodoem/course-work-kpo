using CosmicCollector.Core.Events;

namespace CosmicCollector.MVC.Flow;

/// <summary>
/// Состояние главного меню.
/// </summary>
public sealed class MenuState : IGameFlowState
{
  /// <inheritdoc />
  public string Name => "Menu";

  /// <inheritdoc />
  public bool AllowsWorldTick => false;

  /// <inheritdoc />
  public void OnEnter(IGameFlowContext parContext)
  {
    parContext.EventBus.Publish(new MenuNavigationRequested());
  }

  /// <inheritdoc />
  public void OnExit(IGameFlowContext parContext)
  {
  }

  /// <inheritdoc />
  public void HandleCommand(IGameFlowContext parContext, IGameCommand parCommand)
  {
  }
}
