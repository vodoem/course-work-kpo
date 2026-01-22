using System;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Services;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет стратегии применения бонусов.
/// </summary>
public sealed class BonusEffectStrategyTests
{
  [Xunit.Fact]
  public void AcceleratorStrategy_ExtendsRemainingTime()
  {
    var state = new GameState();
    state.AcceleratorRemainingSec = 1.0;
    var bonus = CreateBonus(BonusType.Accelerator, 5.0);
    var strategy = new AcceleratorBonusEffectStrategy();

    strategy.Apply(state, bonus);

    Xunit.Assert.Equal(5.0, state.AcceleratorRemainingSec);
  }

  [Xunit.Fact]
  public void TimeStabilizerStrategy_AddsLevelTime_WhenTimerMissing()
  {
    var state = new GameState();
    var bonus = CreateBonus(BonusType.TimeStabilizer, 4.0);
    var strategy = new TimeStabilizerBonusEffectStrategy();

    strategy.Apply(state, bonus);

    Xunit.Assert.Equal(4.0, state.TimeStabilizerRemainingSec);
    Xunit.Assert.Equal(65.0, state.LevelTimeRemainingSec);
  }

  [Xunit.Fact]
  public void MagnetStrategy_ExtendsRemainingTime()
  {
    var state = new GameState();
    state.MagnetRemainingSec = 2.0;
    var bonus = CreateBonus(BonusType.Magnet, 6.0);
    var strategy = new MagnetBonusEffectStrategy();

    strategy.Apply(state, bonus);

    Xunit.Assert.Equal(6.0, state.MagnetRemainingSec);
  }

  private static Bonus CreateBonus(BonusType parType, double parDuration)
  {
    return new Bonus(
      Guid.NewGuid(),
      Vector2.Zero,
      Vector2.Zero,
      new Aabb(10, 10),
      parType,
      parDuration);
  }
}
