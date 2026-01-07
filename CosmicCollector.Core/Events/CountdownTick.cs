namespace CosmicCollector.Core.Events;

/// <summary>
/// Событие шага обратного отсчёта.
/// </summary>
/// <param name="parValue">Текущее значение отсчёта.</param>
public sealed record CountdownTick(int parValue) : IGameEvent;
