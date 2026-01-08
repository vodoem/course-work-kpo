using CosmicCollector.Core.Geometry;

namespace CosmicCollector.Core.Entities;

/// <summary>
/// Представляет чёрную дыру.
/// </summary>
public sealed class BlackHole : GameObject
{
  /// <summary>
  /// Инициализирует чёрную дыру.
  /// </summary>
  /// <param name="parId">Идентификатор.</param>
  /// <param name="parPosition">Позиция.</param>
  /// <param name="parVelocity">Скорость.</param>
  /// <param name="parBounds">Габариты.</param>
  /// <param name="parRadius">Радиус притяжения.</param>
  /// <param name="parCoreRadius">Радиус ядра.</param>
  public BlackHole(
    Guid parId,
    Vector2 parPosition,
    Vector2 parVelocity,
    Aabb parBounds,
    double parRadius,
    double parCoreRadius)
    : base(parId, parPosition, parVelocity, parBounds)
  {
    Radius = parRadius;
    CoreRadius = parCoreRadius;
  }

  /// <summary>
  /// Радиус притяжения.
  /// </summary>
  public double Radius { get; }

  /// <summary>
  /// Радиус ядра.
  /// </summary>
  public double CoreRadius { get; }
}
