using CosmicCollector.Core.Snapshots;

namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Контракт представления игрового экрана.
/// </summary>
public interface IGameScreenView
{
  /// <summary>
  /// Отрисовывает игровой экран по снимку.
  /// </summary>
  /// <param name="parSnapshot">Снимок состояния игры.</param>
  /// <param name="parLevel">Номер уровня.</param>
  void Render(GameSnapshot parSnapshot, int parLevel);
}
