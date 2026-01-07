using CosmicCollector.MVC.Commands;

namespace CosmicCollector.MVC.Controllers;

/// <summary>
/// Определяет операции контроллера игрового движка.
/// </summary>
public interface IGameController
{
  /// <summary>
  /// Запускает игровой цикл.
  /// </summary>
  void Start();

  /// <summary>
  /// Останавливает игровой цикл.
  /// </summary>
  void Stop();

  /// <summary>
  /// Помещает команду из слоя ввода в очередь.
  /// </summary>
  /// <param name="parCommand">Команда для постановки в очередь.</param>
  void EnqueueCommand(IGameCommand parCommand);

}
