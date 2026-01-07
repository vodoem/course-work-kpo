namespace CosmicCollector.Core.Events;

/// <summary>
/// Событие завершения уровня.
/// </summary>
/// <param name="parResult">Результат завершения.</param>
public sealed record LevelCompleted(string parResult) : IGameEvent;
