using CosmicCollector.Core.Events;
using CosmicCollector.MVC.Commands;

namespace CosmicCollector.MVC.Flow;

/// <summary>
/// Состояние подтверждения сохранения перед выходом в меню.
/// </summary>
public sealed class ConfirmSaveBeforeMenuState : IGameFlowState
{
  /// <inheritdoc />
  public string Name => "ConfirmSaveBeforeMenu";

  /// <inheritdoc />
  public bool AllowsWorldTick => false;

  /// <inheritdoc />
  public void OnEnter(IGameFlowContext parContext)
  {
    parContext.EventBus.Publish(new ConfirmSaveBeforeMenuRequested());
  }

  /// <inheritdoc />
  public void OnExit(IGameFlowContext parContext)
  {
    parContext.EventBus.Publish(new ConfirmSaveBeforeMenuClosed());
  }

  /// <inheritdoc />
  public void HandleCommand(IGameFlowContext parContext, IGameCommand parCommand)
  {
    if (parCommand is ConfirmSaveAndExitCommand)
    {
      parContext.SaveService.Save(parContext.GameState);
      parContext.TransitionTo(new MenuState(true));
      return;
    }

    if (parCommand is ConfirmExitWithoutSaveCommand)
    {
      parContext.TransitionTo(new MenuState(false));
      return;
    }

    if (parCommand is CancelExitCommand)
    {
      parContext.TransitionTo(new PausedState());
    }
  }
}
