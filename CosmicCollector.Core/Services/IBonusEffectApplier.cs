using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Model;

namespace CosmicCollector.Core.Services;

/// <summary>
/// Определяет сервис применения бонусов.
/// </summary>
public interface IBonusEffectApplier
{
  /// <summary>
  /// Применяет бонус и возвращает признак блокировки таймера.
  /// </summary>
  /// <param name="parGameState">Состояние игры.</param>
  /// <param name="parBonus">Бонус.</param>
  /// <returns>True, если нужно заблокировать уменьшение таймера.</returns>
  bool Apply(GameState parGameState, Bonus parBonus);
}
