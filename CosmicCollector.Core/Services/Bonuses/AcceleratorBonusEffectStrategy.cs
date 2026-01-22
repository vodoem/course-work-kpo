using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Model;
using System;

namespace CosmicCollector.Core.Services.Bonuses;

/// <summary>
/// Применяет эффект ускорителя.
/// </summary>
public sealed class AcceleratorBonusEffectStrategy : IBonusEffectStrategy
{
  /// <inheritdoc />
  public BonusType Type => BonusType.Accelerator;

  /// <inheritdoc />
  public bool Apply(GameState parGameState, Bonus parBonus)
  {
    parGameState.AcceleratorRemainingSec = Math.Max(
      parGameState.AcceleratorRemainingSec,
      parBonus.DurationSec);
    return false;
  }
}
