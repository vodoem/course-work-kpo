using CosmicCollector.Core.Geometry;

namespace CosmicCollector.Core.Entities;

/// <summary>
/// Представляет кристалл.
/// </summary>
public sealed class Crystal : GameObject
{
  /// <summary>
  /// Инициализирует кристалл.
  /// </summary>
  /// <param name="parId">Идентификатор.</param>
  /// <param name="parPosition">Позиция.</param>
  /// <param name="parVelocity">Скорость.</param>
  /// <param name="parBounds">Габариты.</param>
  /// <param name="parType">Тип кристалла.</param>
  public Crystal(
    Guid parId,
    Vector2 parPosition,
    Vector2 parVelocity,
    Aabb parBounds,
    CrystalType parType)
    : base(parId, parPosition, parVelocity, parBounds)
  {
    Type = parType;
  }

  /// <summary>
  /// Тип кристалла.
  /// </summary>
  public CrystalType Type { get; }
}
