using CosmicCollector.Core.Model;

namespace CosmicCollector.MVC.Services;

/// <summary>
/// Определяет сервис сохранения игры.
/// </summary>
public interface IGameSaveService
{
  /// <summary>
  /// Сохраняет состояние игры.
  /// </summary>
  /// <param name="parGameState">Состояние игры.</param>
  void Save(GameState parGameState);
}
