using CosmicCollector.Core.Snapshots;

namespace CosmicCollector.MVC.Views;

/// <summary>
/// Определяет обновление представления для отрисовки состояния игры.
/// </summary>
public interface IGameView
{
  /// <summary>
  /// Применяет новый неизменяемый снимок для отрисовки.
  /// </summary>
  /// <param name="parSnapshot">Снимок для отрисовки.</param>
  void Update(GameSnapshot parSnapshot);
}
