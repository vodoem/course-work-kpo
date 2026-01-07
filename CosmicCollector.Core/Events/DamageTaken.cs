namespace CosmicCollector.Core.Events;

/// <summary>
/// Событие получения урона.
/// </summary>
/// <param name="parSourceType">Источник урона.</param>
/// <param name="parAmount">Величина урона.</param>
public sealed record DamageTaken(string parSourceType, int parAmount) : IGameEvent;
