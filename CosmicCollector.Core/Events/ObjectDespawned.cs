namespace CosmicCollector.Core.Events;

/// <summary>
/// Событие удаления объекта.
/// </summary>
/// <param name="parObjectId">Идентификатор объекта.</param>
/// <param name="parReason">Причина удаления.</param>
public sealed record ObjectDespawned(Guid parObjectId, string parReason) : IGameEvent;
