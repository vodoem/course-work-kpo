using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Geometry;

namespace CosmicCollector.Core.Snapshots;

/// <summary>
/// Снимок состояния бонуса.
/// </summary>
/// <param name="parId">Идентификатор.</param>
/// <param name="parPosition">Позиция.</param>
/// <param name="parVelocity">Скорость.</param>
/// <param name="parBounds">Габариты.</param>
/// <param name="parType">Тип бонуса.</param>
/// <param name="parDurationSec">Длительность бонуса.</param>
public sealed record BonusSnapshot(
  Guid parId,
  Vector2 parPosition,
  Vector2 parVelocity,
  Aabb parBounds,
  BonusType parType,
  double parDurationSec);
