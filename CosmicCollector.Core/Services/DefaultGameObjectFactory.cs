using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Geometry;

namespace CosmicCollector.Core.Services;

/// <summary>
/// Реализация фабрики игровых объектов по умолчанию.
/// </summary>
public sealed class DefaultGameObjectFactory : IGameObjectFactory
{
  /// <inheritdoc />
  public Crystal CreateCrystal(
    Guid parId,
    Vector2 parPosition,
    Vector2 parVelocity,
    Aabb parBounds,
    CrystalType parType)
  {
    return new Crystal(parId, parPosition, parVelocity, parBounds, parType);
  }

  /// <inheritdoc />
  public Asteroid CreateAsteroid(Guid parId, Vector2 parPosition, Vector2 parVelocity, Aabb parBounds)
  {
    return new Asteroid(parId, parPosition, parVelocity, parBounds);
  }

  /// <inheritdoc />
  public Bonus CreateBonus(
    Guid parId,
    Vector2 parPosition,
    Vector2 parVelocity,
    Aabb parBounds,
    BonusType parType,
    double parDurationSec)
  {
    return new Bonus(parId, parPosition, parVelocity, parBounds, parType, parDurationSec);
  }

  /// <inheritdoc />
  public BlackHole CreateBlackHole(
    Guid parId,
    Vector2 parPosition,
    Vector2 parVelocity,
    Aabb parBounds,
    double parRadius,
    double parCoreRadius)
  {
    return new BlackHole(parId, parPosition, parVelocity, parBounds, parRadius, parCoreRadius);
  }
}
