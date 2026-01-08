using CosmicCollector.Core.Model;
using CosmicCollector.Core.Snapshots;

namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Поставщик снимков из игрового состояния.
/// </summary>
public sealed class GameSnapshotProvider : IGameSnapshotProvider
{
  private readonly GameState _gameState;

  /// <summary>
  /// Создаёт поставщик снимков.
  /// </summary>
  /// <param name="parGameState">Состояние игры.</param>
  public GameSnapshotProvider(GameState parGameState)
  {
    _gameState = parGameState;
  }

  /// <inheritdoc />
  public GameSnapshot GetSnapshot()
  {
    return _gameState.GetSnapshot();
  }
}
