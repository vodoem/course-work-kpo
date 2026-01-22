using System;
using System.Reflection;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Services.Bonuses;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет стратегии бонусов.
/// </summary>
public sealed class BonusEffectStrategyTests
{
  [Xunit.Fact]
  public void AcceleratorStrategy_UsesMaxDuration()
  {
    var state = CreateState();
    var strategy = new AcceleratorBonusEffectStrategy();
    SetInternalProperty(state, "AcceleratorRemainingSec", 2.0);

    var bonus = CreateBonus(BonusType.Accelerator, 5.0);

    strategy.Apply(state, bonus);

    var remaining = GetInternalProperty<double>(state, "AcceleratorRemainingSec");
    Xunit.Assert.Equal(5.0, remaining);
  }

  [Xunit.Fact]
  public void TimeStabilizerStrategy_AddsTimerAndDuration()
  {
    var state = CreateState();
    var strategy = new TimeStabilizerBonusEffectStrategy();

    var bonus = CreateBonus(BonusType.TimeStabilizer, 4.0);

    var affectsTimer = strategy.Apply(state, bonus);

    Xunit.Assert.True(affectsTimer);
    Xunit.Assert.Equal(65.0, state.LevelTimeRemainingSec);
    var remaining = GetInternalProperty<double>(state, "TimeStabilizerRemainingSec");
    Xunit.Assert.Equal(4.0, remaining);
  }

  [Xunit.Fact]
  public void MagnetStrategy_UsesMaxDuration()
  {
    var state = CreateState();
    var strategy = new MagnetBonusEffectStrategy();
    SetInternalProperty(state, "MagnetRemainingSec", 1.5);

    var bonus = CreateBonus(BonusType.Magnet, 3.0);

    strategy.Apply(state, bonus);

    var remaining = GetInternalProperty<double>(state, "MagnetRemainingSec");
    Xunit.Assert.Equal(3.0, remaining);
  }

  private static GameState CreateState()
  {
    return new GameState(
      new Drone(Guid.NewGuid(), Vector2.Zero, Vector2.Zero, new Aabb(10, 10), 100),
      new WorldBounds(0, 0, 800, 600));
  }

  private static Bonus CreateBonus(BonusType parType, double parDurationSec)
  {
    return new Bonus(Guid.NewGuid(), Vector2.Zero, Vector2.Zero, new Aabb(10, 10), parType, parDurationSec);
  }

  private static void SetInternalProperty<TValue>(GameState parState, string parName, TValue parValue)
  {
    var property = typeof(GameState).GetProperty(parName, BindingFlags.Instance | BindingFlags.NonPublic);
    if (property is null)
    {
      throw new InvalidOperationException($"Property '{parName}' not found.");
    }

    property.SetValue(parState, parValue);
  }

  private static TValue GetInternalProperty<TValue>(GameState parState, string parName)
  {
    var property = typeof(GameState).GetProperty(parName, BindingFlags.Instance | BindingFlags.NonPublic);
    if (property is null)
    {
      throw new InvalidOperationException($"Property '{parName}' not found.");
    }

    return (TValue)property.GetValue(parState)!;
  }
}
