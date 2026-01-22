using System;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Model;

namespace CosmicCollector.Core.Services;

/// <summary>
/// Применяет бонус магнита.
/// </summary>
public sealed class MagnetBonusEffectStrategy : IBonusEffectStrategy
{
  /// <inheritdoc />
  public BonusType Type => BonusType.Magnet;

  /// <inheritdoc />
  public bool BlocksTimerDecrease => false;

  /// <inheritdoc />
  public void Apply(GameState parGameState, Bonus parBonus)
  {
    parGameState.MagnetRemainingSec = Math.Max(
      parGameState.MagnetRemainingSec,
      parBonus.DurationSec);
  }
}
