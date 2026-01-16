using CosmicCollector.Core.Snapshots;

namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Предоставляет снимок состояния игры.
/// </summary>
public interface IGameSnapshotProvider
{
  /// <summary>
  /// Возвращает актуальный снимок состояния.
  /// </summary>
  /// <returns>Снимок состояния игры.</returns>
  GameSnapshot GetSnapshot();
}
