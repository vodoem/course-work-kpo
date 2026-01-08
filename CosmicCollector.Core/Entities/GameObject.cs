using CosmicCollector.Core.Geometry;

namespace CosmicCollector.Core.Entities;

/// <summary>
/// Базовый игровой объект.
/// </summary>
public abstract class GameObject
{
  /// <summary>
  /// Инициализирует объект.
  /// </summary>
  /// <param name="parId">Идентификатор.</param>
  /// <param name="parPosition">Позиция.</param>
  /// <param name="parVelocity">Скорость.</param>
  /// <param name="parBounds">Габариты.</param>
  protected GameObject(Guid parId, Vector2 parPosition, Vector2 parVelocity, Aabb parBounds)
  {
    Id = parId;
    Position = parPosition;
    Velocity = parVelocity;
    Bounds = parBounds;
  }

  /// <summary>
  /// Идентификатор объекта.
  /// </summary>
  public Guid Id { get; }

  /// <summary>
  /// Позиция объекта.
  /// </summary>
  public Vector2 Position { get; set; }

  /// <summary>
  /// Скорость объекта.
  /// </summary>
  public Vector2 Velocity { get; set; }

  /// <summary>
  /// Габариты объекта.
  /// </summary>
  public Aabb Bounds { get; }
}
