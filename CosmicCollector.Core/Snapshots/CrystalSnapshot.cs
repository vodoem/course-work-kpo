using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Geometry;

namespace CosmicCollector.Core.Snapshots;

/// <summary>
/// Снимок состояния кристалла.
/// </summary>
/// <param name="parId">Идентификатор.</param>
/// <param name="parPosition">Позиция.</param>
/// <param name="parVelocity">Скорость.</param>
/// <param name="parBounds">Габариты.</param>
/// <param name="parType">Тип кристалла.</param>
public sealed record CrystalSnapshot(
  Guid parId,
  Vector2 parPosition,
  Vector2 parVelocity,
  Aabb parBounds,
  CrystalType parType);
