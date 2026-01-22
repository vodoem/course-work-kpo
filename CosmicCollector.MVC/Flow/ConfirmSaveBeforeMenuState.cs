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
  public void OnEnter(IGameFlowRuntime parRuntime)
  {
    parRuntime.EventPublisher.Publish(new ConfirmSaveDialogOpened());
  }

  /// <inheritdoc />
  public void OnExit(IGameFlowRuntime parRuntime)
  {
    parRuntime.EventPublisher.Publish(new ConfirmSaveDialogClosed());
  }

  /// <inheritdoc />
  public void HandleCommand(IGameFlowRuntime parRuntime, IGameCommand parCommand)
  {
    switch (parCommand)
    {
      case ConfirmSaveYesCommand:
        parRuntime.EventPublisher.Publish(new MenuExitRequested(true));
        parRuntime.TransitionTo(new MenuState());
        return;
      case ConfirmSaveNoCommand:
        parRuntime.EventPublisher.Publish(new MenuExitRequested(false));
        parRuntime.TransitionTo(new MenuState());
        return;
      case ConfirmSaveCancelCommand:
        parRuntime.TransitionTo(new PausedState());
        return;
    }
  }

  /// <inheritdoc />
  public void OnTick(IGameFlowRuntime parRuntime, double parDt)
  {
  }
}
