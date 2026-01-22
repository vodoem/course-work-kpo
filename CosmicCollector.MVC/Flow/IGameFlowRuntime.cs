using CosmicCollector.Core.Events;
using CosmicCollector.Core.Model;

namespace CosmicCollector.MVC.Flow;

/// <summary>
/// Контекст выполнения игрового потока для состояний.
/// </summary>
public interface IGameFlowRuntime
{
  /// <summary>
  /// Состояние игры.
  /// </summary>
  GameState GameState { get; }

  /// <summary>
  /// Публикатор событий.
  /// </summary>
  IEventPublisher EventPublisher { get; }

  /// <summary>
  /// Выполняет обработку обратного отсчёта для снятия паузы.
  /// </summary>
  /// <param name="parDt">Длительность шага.</param>
  /// <returns>Признак завершения отсчёта.</returns>
  bool ProcessResumeCountdown(double parDt);

  /// <summary>
  /// Переключает состояние игрового потока.
  /// </summary>
  /// <param name="parNextState">Следующее состояние.</param>
  void TransitionTo(IGameFlowState parNextState);
}
