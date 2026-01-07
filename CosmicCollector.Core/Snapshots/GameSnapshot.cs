namespace CosmicCollector.Core.Snapshots;

/// <summary>
/// Неизменяемый снимок состояния игры для представлений.
/// </summary>
/// <param name="parIsPaused">Признак паузы.</param>
/// <param name="parTickNo">Номер тика.</param>
public sealed record GameSnapshot(bool parIsPaused, long parTickNo);
