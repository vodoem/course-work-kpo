using CosmicCollector.Core.Geometry;

namespace CosmicCollector.Core.Entities;

/// <summary>
/// Представляет астероид.
/// </summary>
public sealed class Asteroid : GameObject
{
  /// <summary>
  /// Инициализирует астероид.
  /// </summary>
  /// <param name="parId">Идентификатор.</param>
  /// <param name="parPosition">Позиция.</param>
  /// <param name="parVelocity">Скорость.</param>
  /// <param name="parBounds">Габариты.</param>
  public Asteroid(Guid parId, Vector2 parPosition, Vector2 parVelocity, Aabb parBounds)
    : base(parId, parPosition, parVelocity, parBounds)
  {
  }
}
