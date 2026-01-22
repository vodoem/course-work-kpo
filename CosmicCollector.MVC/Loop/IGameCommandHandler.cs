using CosmicCollector.MVC.Commands;

namespace CosmicCollector.MVC.Loop;

/// <summary>
/// Обрабатывает игровые команды.
/// </summary>
public interface IGameCommandHandler
{
  /// <summary>
  /// Обрабатывает команду.
  /// </summary>
  /// <param name="parCommand">Команда игры.</param>
  void HandleCommand(IGameCommand parCommand);
}
