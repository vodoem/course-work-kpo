using CosmicCollector.MVC.Commands;

namespace CosmicCollector.MVC.Flow;

/// <summary>
/// Описывает состояние игрового потока.
/// </summary>
public interface IGameFlowState
{
  /// <summary>
  /// Имя состояния.
  /// </summary>
  string Name { get; }

  /// <summary>
  /// Можно ли выполнять тик обновления мира.
  /// </summary>
  bool AllowsWorldTick { get; }

  /// <summary>
  /// Вызывается при входе в состояние.
  /// </summary>
  /// <param name="parContext">Контекст игрового потока.</param>
  void OnEnter(IGameFlowContext parContext);

  /// <summary>
  /// Вызывается при выходе из состояния.
  /// </summary>
  /// <param name="parContext">Контекст игрового потока.</param>
  void OnExit(IGameFlowContext parContext);

  /// <summary>
  /// Обрабатывает команду.
  /// </summary>
  /// <param name="parContext">Контекст игрового потока.</param>
  /// <param name="parCommand">Команда.</param>
  void HandleCommand(IGameFlowContext parContext, IGameCommand parCommand);
}
