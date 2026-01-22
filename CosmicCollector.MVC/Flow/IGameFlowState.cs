using CosmicCollector.MVC.Commands;

namespace CosmicCollector.MVC.Flow;

/// <summary>
/// Описывает состояние игрового потока.
/// </summary>
public interface IGameFlowState
{
  /// <summary>
  /// Название состояния.
  /// </summary>
  string Name { get; }

  /// <summary>
  /// Признак разрешения обновления игрового мира.
  /// </summary>
  bool AllowsWorldTick { get; }

  /// <summary>
  /// Вызывается при входе в состояние.
  /// </summary>
  /// <param name="parRuntime">Контекст выполнения.</param>
  void OnEnter(IGameFlowRuntime parRuntime);

  /// <summary>
  /// Вызывается при выходе из состояния.
  /// </summary>
  /// <param name="parRuntime">Контекст выполнения.</param>
  void OnExit(IGameFlowRuntime parRuntime);

  /// <summary>
  /// Обрабатывает команду в рамках состояния.
  /// </summary>
  /// <param name="parRuntime">Контекст выполнения.</param>
  /// <param name="parCommand">Команда.</param>
  void HandleCommand(IGameFlowRuntime parRuntime, IGameCommand parCommand);

  /// <summary>
  /// Вызывается на каждом тике игрового цикла.
  /// </summary>
  /// <param name="parRuntime">Контекст выполнения.</param>
  /// <param name="parDt">Длительность шага.</param>
  void OnTick(IGameFlowRuntime parRuntime, double parDt);
}
