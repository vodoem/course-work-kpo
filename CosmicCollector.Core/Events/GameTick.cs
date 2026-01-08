namespace CosmicCollector.Core.Events;

/// <summary>
/// Событие тика игрового цикла.
/// </summary>
/// <param name="parDt">Длительность шага в секундах.</param>
/// <param name="parTickNo">Номер тика.</param>
public sealed record GameTick(double parDt, long parTickNo) : IGameEvent;
