using System;
using System.Collections.Generic;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Model;

namespace CosmicCollector.Core.Services;

/// <summary>
/// Применяет бонусы через зарегистрированные стратегии.
/// </summary>
public sealed class BonusEffectApplier : IBonusEffectApplier
{
  private readonly IReadOnlyDictionary<BonusType, IBonusEffectStrategy> _strategies;

  /// <summary>
  /// Инициализирует аплайер бонусов с дефолтными стратегиями.
  /// </summary>
  public BonusEffectApplier()
    : this(new IBonusEffectStrategy[]
    {
      new AcceleratorBonusEffectStrategy(),
      new TimeStabilizerBonusEffectStrategy(),
      new MagnetBonusEffectStrategy()
    })
  {
  }

  /// <summary>
  /// Инициализирует аплайер бонусов с указанными стратегиями.
  /// </summary>
  /// <param name="parStrategies">Стратегии бонусов.</param>
  public BonusEffectApplier(IEnumerable<IBonusEffectStrategy> parStrategies)
  {
    if (parStrategies is null)
    {
      throw new ArgumentNullException(nameof(parStrategies));
    }

    var map = new Dictionary<BonusType, IBonusEffectStrategy>();
    foreach (var strategy in parStrategies)
    {
      map[strategy.Type] = strategy;
    }

    _strategies = map;
  }

  /// <inheritdoc />
  public bool Apply(GameState parGameState, Bonus parBonus)
  {
    if (!_strategies.TryGetValue(parBonus.Type, out var strategy))
    {
      return false;
    }

    strategy.Apply(parGameState, parBonus);
    return strategy.BlocksTimerDecrease;
  }
}
