using CosmicCollector.Core.Geometry;

namespace CosmicCollector.Core.Snapshots;

/// <summary>
/// Снимок состояния чёрной дыры.
/// </summary>
/// <param name="parId">Идентификатор.</param>
/// <param name="parPosition">Позиция.</param>
/// <param name="parVelocity">Скорость.</param>
/// <param name="parBounds">Габариты.</param>
/// <param name="parRadius">Радиус притяжения.</param>
/// <param name="parCoreRadius">Радиус ядра.</param>
public sealed record BlackHoleSnapshot(
  Guid parId,
  Vector2 parPosition,
  Vector2 parVelocity,
  Aabb parBounds,
  double parRadius,
  double parCoreRadius);
