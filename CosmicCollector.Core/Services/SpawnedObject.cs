namespace CosmicCollector.Core.Services;

/// <summary>
/// Описывает результат спавна объекта.
/// </summary>
public sealed record SpawnedObject(string ObjectType, Guid ObjectId);
