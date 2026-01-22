using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Model;

namespace CosmicCollector.Core.Services;

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
  /// Признак блокировки уменьшения таймера уровня.
  /// </summary>
  bool BlocksTimerDecrease { get; }

  /// <summary>
  /// Применяет эффект бонуса.
  /// </summary>
  /// <param name="parGameState">Состояние игры.</param>
  /// <param name="parBonus">Бонус.</param>
  void Apply(GameState parGameState, Bonus parBonus);
}
