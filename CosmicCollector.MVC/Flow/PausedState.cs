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
    if (parCommand is TogglePauseCommand)
    {
      var isPaused = parContext.GameState.TogglePause();
      parContext.EventBus.Publish(new PauseToggled(isPaused));
      parContext.TransitionTo(new ResumeCountdownState());
      return;
    }

    if (parCommand is RequestBackToMenuCommand)
    {
      parContext.TransitionTo(new ConfirmSaveBeforeMenuState());
    }
  }
}
