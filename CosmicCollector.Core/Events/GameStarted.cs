namespace CosmicCollector.Core.Events;

/// <summary>
/// Событие запуска игры на указанном уровне.
/// </summary>
/// <param name="parLevel">Номер уровня.</param>
public sealed record GameStarted(int parLevel) : IGameEvent;
