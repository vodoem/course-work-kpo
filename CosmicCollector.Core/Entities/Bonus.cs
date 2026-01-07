using CosmicCollector.Core.Geometry;

namespace CosmicCollector.Core.Entities;

/// <summary>
/// Представляет бонус.
/// </summary>
public sealed class Bonus : GameObject
{
  /// <summary>
  /// Инициализирует бонус.
  /// </summary>
  /// <param name="parId">Идентификатор.</param>
  /// <param name="parPosition">Позиция.</param>
  /// <param name="parVelocity">Скорость.</param>
  /// <param name="parBounds">Габариты.</param>
  /// <param name="parType">Тип бонуса.</param>
  /// <param name="parDurationSec">Длительность бонуса в секундах.</param>
  public Bonus(
    Guid parId,
    Vector2 parPosition,
    Vector2 parVelocity,
    Aabb parBounds,
    BonusType parType,
    double parDurationSec)
    : base(parId, parPosition, parVelocity, parBounds)
  {
    Type = parType;
    DurationSec = parDurationSec;
  }

  /// <summary>
  /// Тип бонуса.
  /// </summary>
  public BonusType Type { get; }

  /// <summary>
  /// Длительность бонуса в секундах.
  /// </summary>
  public double DurationSec { get; }
}
