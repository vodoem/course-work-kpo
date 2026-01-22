using CosmicCollector.Core.Model;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Services;

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
  /// Сервис сохранения.
  /// </summary>
  IGameSaveService SaveService { get; }

  /// <summary>
  /// Выполняет переход в новое состояние.
  /// </summary>
  /// <param name="parNextState">Следующее состояние.</param>
  void TransitionTo(IGameFlowState parNextState);
}
