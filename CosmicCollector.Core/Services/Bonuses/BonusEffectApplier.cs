using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Model;
using System;
using System.Collections.Generic;

namespace CosmicCollector.Core.Services.Bonuses;

/// <summary>
/// Применяет эффекты бонусов, используя зарегистрированные стратегии.
/// </summary>
public sealed class BonusEffectApplier : IBonusEffectApplier
{
  private readonly Dictionary<BonusType, IBonusEffectStrategy> _strategies;

  /// <summary>
  /// Инициализирует новый экземпляр <see cref="BonusEffectApplier"/>.
  /// </summary>
  /// <param name="parStrategies">Стратегии бонусов.</param>
  public BonusEffectApplier(IEnumerable<IBonusEffectStrategy> parStrategies)
  {
    _strategies = new Dictionary<BonusType, IBonusEffectStrategy>();

    foreach (var strategy in parStrategies)
    {
      if (strategy is null)
      {
        continue;
      }

      _strategies[strategy.Type] = strategy;
    }
  }

  /// <inheritdoc />
  public bool Apply(GameState parGameState, Bonus parBonus)
  {
    if (parBonus is null)
    {
      throw new ArgumentNullException(nameof(parBonus));
    }

    if (!_strategies.TryGetValue(parBonus.Type, out var strategy))
    {
      return false;
    }

    return strategy.Apply(parGameState, parBonus);
  }
}
