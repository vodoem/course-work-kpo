using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Model;

namespace CosmicCollector.Core.Services.Bonuses;

/// <summary>
/// Определяет стратегию применения бонуса.
/// </summary>
public interface IBonusEffectStrategy
{
  /// <summary>
  /// Тип бонуса.
  /// </summary>
  BonusType Type { get; }

  /// <summary>
  /// Применяет эффект бонуса.
  /// </summary>
  /// <param name="parGameState">Состояние игры.</param>
  /// <param name="parBonus">Бонус.</param>
  /// <returns>Признак влияния на таймер уровня.</returns>
  bool Apply(GameState parGameState, Bonus parBonus);
}
