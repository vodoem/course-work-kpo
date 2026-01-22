using System;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Model;

namespace CosmicCollector.Core.Services;

/// <summary>
/// Применяет бонус ускорителя.
/// </summary>
public sealed class AcceleratorBonusEffectStrategy : IBonusEffectStrategy
{
  /// <inheritdoc />
  public BonusType Type => BonusType.Accelerator;

  /// <inheritdoc />
  public bool BlocksTimerDecrease => false;

  /// <inheritdoc />
  public void Apply(GameState parGameState, Bonus parBonus)
  {
    parGameState.AcceleratorRemainingSec = Math.Max(
      parGameState.AcceleratorRemainingSec,
      parBonus.DurationSec);
  }
}
