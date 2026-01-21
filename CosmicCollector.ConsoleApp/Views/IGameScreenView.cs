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
  /// <param name="parIsPaused">Признак паузы.</param>
  /// <param name="parCountdownValue">Текущее значение отсчёта.</param>
  void Render(GameSnapshot parSnapshot, int parLevel, bool parIsPaused, int parCountdownValue);
}
