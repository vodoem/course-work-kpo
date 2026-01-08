using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.Core.Snapshots;

namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Контракт представления экрана завершения игры.
/// </summary>
public interface IGameEndView
{
  /// <summary>
  /// Отрисовывает экран завершения игры.
  /// </summary>
  /// <param name="parReason">Причина завершения.</param>
  /// <param name="parSnapshot">Финальный снимок.</param>
  /// <param name="parLevel">Номер уровня.</param>
  void Render(GameEndReason parReason, GameSnapshot parSnapshot, int parLevel);
}
