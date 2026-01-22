using CosmicCollector.Core.Model;
using CosmicCollector.MVC.Eventing;

namespace CosmicCollector.MVC.Flow;

/// <summary>
/// Контекст игрового потока для состояний.
/// </summary>
public interface IGameFlowContext
{
  /// <summary>
  /// Состояние игры.
  /// </summary>
  GameState GameState { get; }

  /// <summary>
  /// Шина событий.
  /// </summary>
  IEventBus EventBus { get; }

  /// <summary>
  /// Выполняет переход в новое состояние.
  /// </summary>
  /// <param name="parNextState">Следующее состояние.</param>
  void TransitionTo(IGameFlowState parNextState);
}
