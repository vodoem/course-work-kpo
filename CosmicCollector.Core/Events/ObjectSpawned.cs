namespace CosmicCollector.Core.Events;

/// <summary>
/// Событие появления объекта.
/// </summary>
/// <param name="parObjectType">Тип объекта.</param>
/// <param name="parObjectId">Идентификатор объекта.</param>
public sealed record ObjectSpawned(string parObjectType, Guid parObjectId) : IGameEvent;
