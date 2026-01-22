using System;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Model;

namespace CosmicCollector.Core.Services;

/// <summary>
/// Применяет бонус стабилизатора времени.
/// </summary>
public sealed class TimeStabilizerBonusEffectStrategy : IBonusEffectStrategy
{
  private const double TimeStabilizerBonusSeconds = 5.0;
  private const double TimeStabilizerBaseSeconds = 60.0;

  /// <inheritdoc />
  public BonusType Type => BonusType.TimeStabilizer;

  /// <inheritdoc />
  public bool BlocksTimerDecrease => true;

  /// <inheritdoc />
  public void Apply(GameState parGameState, Bonus parBonus)
  {
    parGameState.TimeStabilizerRemainingSec = Math.Max(
      parGameState.TimeStabilizerRemainingSec,
      parBonus.DurationSec);

    if (!parGameState.HasLevelTimer)
    {
      parGameState.LevelTimeRemainingSec = TimeStabilizerBaseSeconds;
    }

    parGameState.LevelTimeRemainingSec += TimeStabilizerBonusSeconds;
  }
}
