using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Geometry;

namespace CosmicCollector.Core.Services;

/// <summary>
/// Определяет фабрику игровых объектов.
/// </summary>
public interface IGameObjectFactory
{
  /// <summary>
  /// Создаёт кристалл.
  /// </summary>
  Crystal CreateCrystal(Guid parId, Vector2 parPosition, Vector2 parVelocity, Aabb parBounds, CrystalType parType);

  /// <summary>
  /// Создаёт астероид.
  /// </summary>
  Asteroid CreateAsteroid(Guid parId, Vector2 parPosition, Vector2 parVelocity, Aabb parBounds);

  /// <summary>
  /// Создаёт бонус.
  /// </summary>
  Bonus CreateBonus(
    Guid parId,
    Vector2 parPosition,
    Vector2 parVelocity,
    Aabb parBounds,
    BonusType parType,
    double parDurationSec);

  /// <summary>
  /// Создаёт чёрную дыру.
  /// </summary>
  BlackHole CreateBlackHole(
    Guid parId,
    Vector2 parPosition,
    Vector2 parVelocity,
    Aabb parBounds,
    double parRadius,
    double parCoreRadius);
}
