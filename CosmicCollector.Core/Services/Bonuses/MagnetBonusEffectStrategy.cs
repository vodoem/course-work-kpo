using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Model;
using System;

namespace CosmicCollector.Core.Services.Bonuses;

/// <summary>
/// Применяет эффект магнита.
/// </summary>
public sealed class MagnetBonusEffectStrategy : IBonusEffectStrategy
{
  /// <inheritdoc />
  public BonusType Type => BonusType.Magnet;

  /// <inheritdoc />
  public bool Apply(GameState parGameState, Bonus parBonus)
  {
    parGameState.MagnetRemainingSec = Math.Max(
      parGameState.MagnetRemainingSec,
      parBonus.DurationSec);
    return false;
  }
}
