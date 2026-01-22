using CosmicCollector.Core.Model;

namespace CosmicCollector.MVC.Services;

/// <summary>
/// Заглушка сервиса сохранения.
/// </summary>
public sealed class NullGameSaveService : IGameSaveService
{
  /// <inheritdoc />
  public void Save(GameState parGameState)
  {
  }
}
