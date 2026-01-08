using CosmicCollector.Core.Geometry;

namespace CosmicCollector.Core.Entities;

/// <summary>
/// Представляет сущность дрона игрока.
/// </summary>
public sealed class Drone : GameObject
{
  /// <summary>
  /// Инициализирует дрон.
  /// </summary>
  /// <param name="parId">Идентификатор.</param>
  /// <param name="parPosition">Позиция.</param>
  /// <param name="parVelocity">Скорость.</param>
  /// <param name="parBounds">Габариты.</param>
  /// <param name="parEnergy">Энергия.</param>
  public Drone(Guid parId, Vector2 parPosition, Vector2 parVelocity, Aabb parBounds, int parEnergy)
    : base(parId, parPosition, parVelocity, parBounds)
  {
    Energy = parEnergy;
  }

  /// <summary>
  /// Текущая энергия дрона.
  /// </summary>
  public int Energy { get; set; }
}
