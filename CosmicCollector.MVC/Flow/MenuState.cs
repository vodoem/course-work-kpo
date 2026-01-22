using CosmicCollector.Core.Events;
namespace CosmicCollector.MVC.Flow;

/// <summary>
/// Состояние главного меню.
/// </summary>
public sealed class MenuState : IGameFlowState
{
  private readonly bool _wasSaved;

  /// <summary>
  /// Инициализирует состояние меню.
  /// </summary>
  /// <param name="parWasSaved">Признак сохранения перед выходом.</param>
  public MenuState(bool parWasSaved)
  {
    _wasSaved = parWasSaved;
  }

  /// <inheritdoc />
  public string Name => "Menu";

  /// <inheritdoc />
  public bool AllowsWorldTick => false;

  /// <inheritdoc />
  public void OnEnter(IGameFlowContext parContext)
  {
    parContext.EventBus.Publish(new MenuNavigationRequested(_wasSaved));
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
