using CosmicCollector.Core.Geometry;

namespace CosmicCollector.Core.Snapshots;

/// <summary>
/// Снимок состояния дрона.
/// </summary>
/// <param name="parId">Идентификатор.</param>
/// <param name="parPosition">Позиция.</param>
/// <param name="parVelocity">Скорость.</param>
/// <param name="parBounds">Габариты.</param>
/// <param name="parEnergy">Энергия.</param>
/// <param name="parScore">Очки.</param>
/// <param name="parIsDisoriented">Признак дезориентации.</param>
/// <param name="parDisorientationRemainingSec">Оставшееся время дезориентации.</param>
public sealed record DroneSnapshot(
  Guid parId,
  Vector2 parPosition,
  Vector2 parVelocity,
  Aabb parBounds,
  int parEnergy,
  int parScore,
  bool parIsDisoriented,
  double parDisorientationRemainingSec);
