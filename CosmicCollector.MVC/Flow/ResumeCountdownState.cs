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
  }
}
