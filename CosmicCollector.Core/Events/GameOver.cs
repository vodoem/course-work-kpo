namespace CosmicCollector.Core.Events;

/// <summary>
/// Событие окончания игры.
/// </summary>
/// <param name="parResult">Результат окончания.</param>
public sealed record GameOver(string parResult) : IGameEvent;
