using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Events;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Randomization;
using System;
using System.Collections.Generic;

namespace CosmicCollector.Core.Services;

/// <summary>
/// Обновляет состояние игрового мира за один тик.
/// </summary>
public sealed class GameWorldUpdateService
{
  private const double SoftMargin = 8.0;
  private const int AsteroidDamage = 10;
  private const double MagnetRadius = 120.0;
  private const double MagnetAttractionAcceleration = 30.0;
  private const double BlackHoleAcceleration = 30.0;
  private const int BlackHoleCoreDamage = 40;
  private const double AcceleratorScoreMultiplier = 1.25;
  private const double TimeStabilizerMultiplier = 0.65;
  private const double DroneAcceleratorMultiplier = 1.6;
  private const double TimeStabilizerBonusSeconds = 5.0;
  private const double DisorientationDurationSec = 3.0;

  private readonly IRandomProvider _randomProvider;
  private readonly SpawnSystem _spawnSystem;

  /// <summary>
  /// Инициализирует сервис обновления мира.
  /// </summary>
  /// <param name="parRandomProvider">Провайдер случайных значений.</param>
  public GameWorldUpdateService(IRandomProvider parRandomProvider)
    : this(parRandomProvider, SpawnConfig.Disabled)
  {
  }

  /// <summary>
  /// Инициализирует сервис обновления мира.
  /// </summary>
  /// <param name="parRandomProvider">Провайдер случайных значений.</param>
  /// <param name="parSpawnConfig">Конфигурация спавна.</param>
  public GameWorldUpdateService(IRandomProvider parRandomProvider, SpawnConfig parSpawnConfig)
  {
    _randomProvider = parRandomProvider;
    _spawnSystem = new SpawnSystem(parRandomProvider, parSpawnConfig);
  }

  /// <summary>
  /// Обновляет состояние мира за один шаг.
  /// </summary>
  /// <param name="parGameState">Состояние игры.</param>
  /// <param name="parDt">Длительность шага.</param>
  /// <param name="parLevel">Номер уровня.</param>
  /// <param name="parEventPublisher">Публикатор событий.</param>
  public void Update(
    GameState parGameState,
    double parDt,
    int parLevel,
    IEventPublisher parEventPublisher)
  {
    if (parDt <= 0)
    {
      return;
    }

    lock (parGameState.SyncRoot)
    {
      if (parGameState.IsLevelCompleted || parGameState.IsGameOver)
      {
        return;
      }

      if (parGameState.IsPaused)
      {
        ProcessResumeCountdown(parGameState, parDt, parEventPublisher);
        return;
      }

      var tickCount = parGameState.AdvanceTickCount();
      UpdateBonusTimers(parGameState, parDt);
      UpdateDisorientation(parGameState, parDt);

      var speedMultiplier = 1.0 + (0.015 * (parLevel - 1));
      var isAcceleratorActive = parGameState.AcceleratorRemainingSec > 0;
      var isTimeStabilizerActive = parGameState.TimeStabilizerRemainingSec > 0;
      var isMagnetActive = parGameState.MagnetRemainingSec > 0;

      UpdateObjectPositions(
        parGameState,
        parDt,
        speedMultiplier,
        isAcceleratorActive,
        isTimeStabilizerActive,
        isMagnetActive);
      SpawnObjects(parGameState, parLevel, tickCount, parEventPublisher);
      ApplyBlackHoleInfluence(parGameState, parDt, parEventPublisher);
      var timeStabilizerCollected = false;
      HandleCollisions(parGameState, parEventPublisher, isAcceleratorActive, ref timeStabilizerCollected);
      RemoveOutOfBoundsObjects(parGameState, parEventPublisher);

      if (parGameState.HasLevelTimer && !timeStabilizerCollected)
      {
        parGameState.LevelTimeRemainingSec = Math.Max(0, parGameState.LevelTimeRemainingSec - parDt);
      }

      CheckEndConditions(parGameState, parEventPublisher);
    }
  }

  private void UpdateBonusTimers(GameState parGameState, double parDt)
  {
    parGameState.AcceleratorRemainingSec = Math.Max(0, parGameState.AcceleratorRemainingSec - parDt);
    parGameState.TimeStabilizerRemainingSec = Math.Max(0, parGameState.TimeStabilizerRemainingSec - parDt);
    parGameState.MagnetRemainingSec = Math.Max(0, parGameState.MagnetRemainingSec - parDt);
  }

  private void SpawnObjects(
    GameState parGameState,
    int parLevel,
    long parTickCount,
    IEventPublisher parEventPublisher)
  {
    var spawned = _spawnSystem.TrySpawn(parGameState, parLevel, parTickCount);

    foreach (var item in spawned)
    {
      parEventPublisher.Publish(new ObjectSpawned(item.ObjectType, item.ObjectId));
    }
  }

  private void UpdateObjectPositions(
    GameState parGameState,
    double parDt,
    double parSpeedMultiplier,
    bool parIsAcceleratorActive,
    bool parIsTimeStabilizerActive,
    bool parIsMagnetActive)
  {
    var droneMultiplier = parIsAcceleratorActive ? DroneAcceleratorMultiplier : 1.0;
    var drone = parGameState.DroneInternal;
    drone.Position = drone.Position.Add(drone.Velocity.Multiply(parDt * droneMultiplier));
    ClampDroneToBounds(parGameState);

    foreach (var crystal in parGameState.CrystalsInternal)
    {
      if (parIsMagnetActive)
      {
        ApplyMagnetEffect(parGameState.DroneInternal, crystal, parDt);
      }

      var velocity = crystal.Velocity.Multiply(parSpeedMultiplier);
      crystal.Position = crystal.Position.Add(velocity.Multiply(parDt));
    }

    foreach (var asteroid in parGameState.AsteroidsInternal)
    {
      var multiplier = parSpeedMultiplier;

      if (parIsTimeStabilizerActive)
      {
        multiplier *= TimeStabilizerMultiplier;
      }

      var velocity = asteroid.Velocity.Multiply(multiplier);
      asteroid.Position = asteroid.Position.Add(velocity.Multiply(parDt));
    }

    foreach (var bonus in parGameState.BonusesInternal)
    {
      var velocity = bonus.Velocity.Multiply(parSpeedMultiplier);
      bonus.Position = bonus.Position.Add(velocity.Multiply(parDt));
    }
  }

  private void ApplyMagnetEffect(Drone parDrone, Crystal parCrystal, double parDt)
  {
    var direction = new Vector2(
      parDrone.Position.X - parCrystal.Position.X,
      parDrone.Position.Y - parCrystal.Position.Y);
    var distance = direction.Length();

    if (distance <= 0 || distance > MagnetRadius)
    {
      return;
    }

    var normalized = direction.Normalize();
    var acceleratedSpeed = parCrystal.Velocity.Length() + (MagnetAttractionAcceleration * parDt);
    parCrystal.Velocity = normalized.Multiply(acceleratedSpeed);
  }

  private void ApplyBlackHoleInfluence(
    GameState parGameState,
    double parDt,
    IEventPublisher parEventPublisher)
  {
    var drone = parGameState.DroneInternal;

    foreach (var blackHole in parGameState.BlackHolesInternal)
    {
      var direction = new Vector2(
        blackHole.Position.X - drone.Position.X,
        blackHole.Position.Y - drone.Position.Y);
      var distance = direction.Length();

      if (distance <= 0 || distance > blackHole.Radius)
      {
        continue;
      }

      var acceleration = direction.Normalize().Multiply(BlackHoleAcceleration);
      drone.Velocity = drone.Velocity.Add(acceleration.Multiply(parDt));
      ActivateDisorientation(parGameState, DisorientationDurationSec);

      if (distance <= blackHole.CoreRadius)
      {
        drone.Energy -= BlackHoleCoreDamage;
        parEventPublisher.Publish(new DamageTaken("BlackHole", BlackHoleCoreDamage));
      }
    }
  }

  private void HandleCollisions(
    GameState parGameState,
    IEventPublisher parEventPublisher,
    bool parIsAcceleratorActive,
    ref bool refTimeStabilizerCollected)
  {
    var drone = parGameState.DroneInternal;
    var crystalsToRemove = new List<Crystal>();
    var asteroidsToRemove = new List<Asteroid>();
    var bonusesToRemove = new List<Bonus>();

    foreach (var crystal in parGameState.CrystalsInternal)
    {
      if (!Intersects(drone, crystal, SoftMargin))
      {
        continue;
      }

      var points = GetCrystalPoints(crystal.Type);

      if (parIsAcceleratorActive)
      {
        points = (int)Math.Round(points * AcceleratorScoreMultiplier, MidpointRounding.AwayFromZero);
      }

      parGameState.AddScore(points);
      parGameState.MarkCrystalCollected(crystal.Type);
      parEventPublisher.Publish(new CrystalCollected(crystal.Type.ToString(), points, 0));
      parEventPublisher.Publish(new ObjectDespawned(crystal.Id, "Collected"));
      crystalsToRemove.Add(crystal);
    }

    foreach (var asteroid in parGameState.AsteroidsInternal)
    {
      if (!Intersects(drone, asteroid, SoftMargin))
      {
        continue;
      }

      drone.Energy -= AsteroidDamage;
      parEventPublisher.Publish(new DamageTaken("Asteroid", AsteroidDamage));
      parEventPublisher.Publish(new ObjectDespawned(asteroid.Id, "Collision"));
      asteroidsToRemove.Add(asteroid);
    }

    foreach (var bonus in parGameState.BonusesInternal)
    {
      if (!Intersects(drone, bonus, SoftMargin))
      {
        continue;
      }

      ActivateBonus(parGameState, bonus, ref refTimeStabilizerCollected);
      parEventPublisher.Publish(new BonusActivated(bonus.Type.ToString(), bonus.DurationSec));
      parEventPublisher.Publish(new ObjectDespawned(bonus.Id, "Collected"));
      bonusesToRemove.Add(bonus);
    }

    foreach (var crystal in crystalsToRemove)
    {
      parGameState.CrystalsInternal.Remove(crystal);
    }

    foreach (var asteroid in asteroidsToRemove)
    {
      parGameState.AsteroidsInternal.Remove(asteroid);
    }

    foreach (var bonus in bonusesToRemove)
    {
      parGameState.BonusesInternal.Remove(bonus);
    }
  }

  private void CheckEndConditions(GameState parGameState, IEventPublisher parEventPublisher)
  {
    if (parGameState.HasLevelTimer && parGameState.LevelTimeRemainingSec <= 0)
    {
      parGameState.MarkGameOver();
      parEventPublisher.Publish(new GameOver("TimeExpired"));
      return;
    }

    if (parGameState.DroneInternal.Energy <= 0)
    {
      parGameState.MarkGameOver();
      parEventPublisher.Publish(new GameOver("EnergyDepleted"));
      return;
    }

    if (parGameState.Score >= parGameState.RequiredScore && parGameState.HasAllCrystalTypes())
    {
      parGameState.MarkLevelCompleted();
      parEventPublisher.Publish(new LevelCompleted("ScoreAndTypes"));
    }
  }

  private void ProcessResumeCountdown(
    GameState parGameState,
    double parDt,
    IEventPublisher parEventPublisher)
  {
    if (!parGameState.IsResumeCountdownActive)
    {
      return;
    }

    parGameState.ResumeCountdownAccumulatedSec += parDt;

    while (parGameState.ResumeCountdownAccumulatedSec >= 1.0 && parGameState.IsResumeCountdownActive)
    {
      parGameState.ResumeCountdownAccumulatedSec -= 1.0;
      var value = parGameState.ResumeCountdownValue;

      if (value <= 0)
      {
        parGameState.StopResumeCountdown();
        break;
      }

      parEventPublisher.Publish(new CountdownTick(value));
      value--;
      parGameState.ResumeCountdownValue = value;

      if (value == 0)
      {
        parGameState.StopResumeCountdown();
        parGameState.SetPaused(false);
        parEventPublisher.Publish(new PauseToggled(false));
        break;
      }
    }
  }

  private void RemoveOutOfBoundsObjects(GameState parGameState, IEventPublisher parEventPublisher)
  {
    var bounds = parGameState.WorldBounds;
    var crystalsToRemove = new List<Crystal>();
    var asteroidsToRemove = new List<Asteroid>();
    var bonusesToRemove = new List<Bonus>();

    foreach (var crystal in parGameState.CrystalsInternal)
    {
      if (IsBelowBottom(crystal, bounds))
      {
        crystalsToRemove.Add(crystal);
      }
    }

    foreach (var asteroid in parGameState.AsteroidsInternal)
    {
      if (IsBelowBottom(asteroid, bounds))
      {
        asteroidsToRemove.Add(asteroid);
      }
    }

    foreach (var bonus in parGameState.BonusesInternal)
    {
      if (IsBelowBottom(bonus, bounds))
      {
        bonusesToRemove.Add(bonus);
      }
    }

    foreach (var crystal in crystalsToRemove)
    {
      parGameState.CrystalsInternal.Remove(crystal);
      parEventPublisher.Publish(new ObjectDespawned(crystal.Id, "OutOfBounds"));
    }

    foreach (var asteroid in asteroidsToRemove)
    {
      parGameState.AsteroidsInternal.Remove(asteroid);
      parEventPublisher.Publish(new ObjectDespawned(asteroid.Id, "OutOfBounds"));
    }

    foreach (var bonus in bonusesToRemove)
    {
      parGameState.BonusesInternal.Remove(bonus);
      parEventPublisher.Publish(new ObjectDespawned(bonus.Id, "OutOfBounds"));
    }
  }

  private static bool IsBelowBottom(GameObject parObject, WorldBounds parWorldBounds)
  {
    var halfHeight = parObject.Bounds.Height / 2.0;
    var top = parObject.Position.Y - halfHeight;
    return top > parWorldBounds.Bottom;
  }

  private int GetCrystalPoints(CrystalType parType)
  {
    return parType switch
    {
      CrystalType.Blue => _randomProvider.NextInt(8, 10),
      CrystalType.Green => _randomProvider.NextInt(5, 7),
      CrystalType.Red => _randomProvider.NextInt(1, 4),
      _ => 0
    };
  }

  private void ActivateBonus(
    GameState parGameState,
    Bonus parBonus,
    ref bool refTimeStabilizerCollected)
  {
    switch (parBonus.Type)
    {
      case BonusType.Accelerator:
        parGameState.AcceleratorRemainingSec = Math.Max(
          parGameState.AcceleratorRemainingSec,
          parBonus.DurationSec);
        break;
      case BonusType.TimeStabilizer:
        parGameState.TimeStabilizerRemainingSec = Math.Max(
          parGameState.TimeStabilizerRemainingSec,
          parBonus.DurationSec);
        parGameState.LevelTimeRemainingSec += TimeStabilizerBonusSeconds;
        refTimeStabilizerCollected = true;
        break;
      case BonusType.Magnet:
        parGameState.MagnetRemainingSec = Math.Max(
          parGameState.MagnetRemainingSec,
          parBonus.DurationSec);
        break;
    }
  }

  private void ActivateDisorientation(GameState parGameState, double parDurationSec)
  {
    if (parDurationSec <= 0)
    {
      return;
    }

    parGameState.IsDisoriented = true;
    parGameState.DisorientationRemainingSec = Math.Max(
      parGameState.DisorientationRemainingSec,
      parDurationSec);
  }

  private void UpdateDisorientation(GameState parGameState, double parDt)
  {
    if (!parGameState.IsDisoriented)
    {
      return;
    }

    var remaining = parGameState.DisorientationRemainingSec - parDt;

    if (remaining <= 1e-9)
    {
      parGameState.DisorientationRemainingSec = 0;
      parGameState.IsDisoriented = false;
      return;
    }

    parGameState.DisorientationRemainingSec = remaining;
  }

  private static bool Intersects(GameObject parLeft, GameObject parRight, double parSoftMargin)
  {
    var halfWidthLeft = parLeft.Bounds.Width / 2.0;
    var halfHeightLeft = parLeft.Bounds.Height / 2.0;
    var halfWidthRight = parRight.Bounds.Width / 2.0;
    var halfHeightRight = parRight.Bounds.Height / 2.0;

    var deltaX = Math.Abs(parLeft.Position.X - parRight.Position.X);
    var deltaY = Math.Abs(parLeft.Position.Y - parRight.Position.Y);

    return deltaX <= (halfWidthLeft + halfWidthRight + parSoftMargin)
      && deltaY <= (halfHeightLeft + halfHeightRight + parSoftMargin);
  }

  private static void ClampDroneToBounds(GameState parGameState)
  {
    var drone = parGameState.DroneInternal;
    var bounds = parGameState.WorldBounds;
    var halfWidth = drone.Bounds.Width / 2.0;
    var halfHeight = drone.Bounds.Height / 2.0;
    var minX = bounds.Left + halfWidth;
    var maxX = bounds.Right - halfWidth;
    var minY = bounds.Top + halfHeight;
    var maxY = bounds.Bottom - halfHeight;
    var clampedX = drone.Position.X;
    var clampedY = drone.Position.Y;

    if (drone.Position.X < bounds.Left - halfWidth)
    {
      clampedX = minX;
    }
    else if (drone.Position.X > bounds.Right + halfWidth)
    {
      clampedX = maxX;
    }

    if (drone.Position.Y < bounds.Top - halfHeight)
    {
      clampedY = minY;
    }
    else if (drone.Position.Y > bounds.Bottom + halfHeight)
    {
      clampedY = maxY;
    }

    drone.Position = new Vector2(clampedX, clampedY);
  }
}
