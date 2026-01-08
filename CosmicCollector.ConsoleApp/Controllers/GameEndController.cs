using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;
using CosmicCollector.Core.Snapshots;

namespace CosmicCollector.ConsoleApp.Controllers;

/// <summary>
/// Контроллер экрана завершения игры.
/// </summary>
public sealed class GameEndController
{
  private readonly IGameEndView _view;
  private readonly IConsoleInputReader _inputReader;

  /// <summary>
  /// Создаёт контроллер экрана завершения.
  /// </summary>
  /// <param name="parView">Представление экрана завершения.</param>
  /// <param name="parInputReader">Читатель ввода.</param>
  public GameEndController(IGameEndView parView, IConsoleInputReader parInputReader)
  {
    _view = parView;
    _inputReader = parInputReader;
  }

  /// <summary>
  /// Запускает экран завершения и возвращает выбранное действие.
  /// </summary>
  /// <param name="parReason">Причина завершения.</param>
  /// <param name="parSnapshot">Финальный снимок.</param>
  /// <param name="parLevel">Номер уровня.</param>
  /// <returns>Действие после экрана завершения.</returns>
  public GameEndAction Run(GameEndReason parReason, GameSnapshot parSnapshot, int parLevel)
  {
    _view.Render(parReason, parSnapshot, parLevel);

    while (true)
    {
      ConsoleKeyInfo keyInfo = _inputReader.ReadKey();

      if (keyInfo.Key == ConsoleKey.Enter || keyInfo.Key == ConsoleKey.Escape)
      {
        return GameEndAction.ReturnToMenu;
      }

      if (keyInfo.Key == ConsoleKey.R)
      {
        return GameEndAction.RestartGame;
      }
    }
  }
}
