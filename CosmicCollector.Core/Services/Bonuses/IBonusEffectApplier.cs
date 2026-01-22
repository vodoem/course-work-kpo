using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Model;

namespace CosmicCollector.Core.Services.Bonuses;

/// <summary>
/// Применяет эффекты бонусов через стратегии.
/// </summary>
public interface IBonusEffectApplier
{
  /// <summary>
  /// Применяет эффект бонуса.
  /// </summary>
  /// <param name="parGameState">Состояние игры.</param>
  /// <param name="parBonus">Бонус.</param>
  /// <returns>Признак влияния на таймер уровня.</returns>
  bool Apply(GameState parGameState, Bonus parBonus);
}
