using CosmicCollector.Core.Model;
using CosmicCollector.MVC.Commands;

namespace CosmicCollector.MVC.Flow;

/// <summary>
/// Контроллер игрового потока и команд.
/// </summary>
public interface IGameFlowController
{
  /// <summary>
  /// Текущее состояние.
  /// </summary>
  IGameFlowState CurrentState { get; }

  /// <summary>
  /// Состояние игры.
  /// </summary>
  GameState GameState { get; }

  /// <summary>
  /// Разрешён ли тик мира в текущем состоянии.
  /// </summary>
  bool AllowsWorldTick { get; }

  /// <summary>
  /// Обрабатывает команду.
  /// </summary>
  /// <param name="parCommand">Команда.</param>
  void HandleCommand(IGameCommand parCommand);
}
