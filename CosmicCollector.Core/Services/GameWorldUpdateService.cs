using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Events;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Randomization;
using CosmicCollector.Core.Services.Bonuses;
using System;
using System.Collections.Generic;
using System.Linq;

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
  private const double AcceleratorScoreMultiplier = 2.0;
  private const double TimeStabilizerMultiplier = 0.65;
  private const double DroneAcceleratorMultiplier = 1.6;
  private const double DisorientationDurationSec = 3.0;
  private const double MinDistanceEps = 1e-3;
  private const double InnerBoostK = 6.0;
  private const double TangentialDampingK = 12.0;
  private const double RadialDampingK = 1.5;

  private readonly IRandomProvider _randomProvider;
  private readonly SpawnSystem _spawnSystem;
  private readonly LevelService _levelService;
  private readonly IBonusEffectApplier _bonusEffectApplier;

  /// <summary>
  /// Инициализирует сервис обновления мира.
  /// </summary>
  /// <param name="parRandomProvider">Провайдер случайных значений.</param>
  public GameWorldUpdateService(IRandomProvider parRandomProvider)
    : this(parRandomProvider, SpawnConfig.Disabled, new LevelService(new LevelConfigProvider()))
  {
  }

  /// <summary>
  /// Инициализирует сервис обновления мира.
  /// </summary>
  /// <param name="parRandomProvider">Провайдер случайных значений.</param>
  /// <param name="parSpawnConfig">Конфигурация спавна.</param>
  public GameWorldUpdateService(IRandomProvider parRandomProvider, SpawnConfig parSpawnConfig)
    : this(parRandomProvider, parSpawnConfig, new LevelService(new LevelConfigProvider()))
  {
  }

  /// <summary>
  /// Инициализирует сервис обновления мира.
  /// </summary>
  /// <param name="parRandomProvider">Провайдер случайных значений.</param>
  /// <param name="parSpawnConfig">Конфигурация спавна.</param>
  /// <param name="parLevelService">Сервис уровней.</param>
  public GameWorldUpdateService(
    IRandomProvider parRandomProvider,
    SpawnConfig parSpawnConfig,
    LevelService parLevelService)
  {
    _randomProvider = parRandomProvider;
    _spawnSystem = new SpawnSystem(parRandomProvider, parSpawnConfig);
    _levelService = parLevelService;
    _bonusEffectApplier = new BonusEffectApplier(new IBonusEffectStrategy[]
    {
      new AcceleratorBonusEffectStrategy(),
      new TimeStabilizerBonusEffectStrategy(),
      new MagnetBonusEffectStrategy()
    });
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
      if (parGameState.IsGameOver)
      {
        return;
      }

      _levelService.InitLevel(parGameState);

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
      HandleBlackHoleCoreCollisions(parGameState, parEventPublisher);
      var timeStabilizerCollected = false;
      HandleCollisions(parGameState, parEventPublisher, isAcceleratorActive, ref timeStabilizerCollected);
      ClampDroneToBounds(parGameState);
      RemoveOutOfBoundsObjects(parGameState, parEventPublisher);

      if (parGameState.HasLevelTimer &&
          !timeStabilizerCollected &&
          !parGameState.IsGameOver)
      {
        parGameState.LevelTimeRemainingSec = Math.Max(0, parGameState.LevelTimeRemainingSec - parDt);
      }

      CheckEndConditions(parGameState, parEventPublisher);
    }
  }

  /// <summary>
  /// Выполняет UpdateBonusTimers.
  /// </summary>
  private void UpdateBonusTimers(GameState parGameState, double parDt)
  {
    parGameState.AcceleratorRemainingSec = Math.Max(0, parGameState.AcceleratorRemainingSec - parDt);
    parGameState.TimeStabilizerRemainingSec = Math.Max(0, parGameState.TimeStabilizerRemainingSec - parDt);
    parGameState.MagnetRemainingSec = Math.Max(0, parGameState.MagnetRemainingSec - parDt);
  }

  /// <summary>
  /// Выполняет SpawnObjects.
  /// </summary>
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

  /// <summary>
  /// Выполняет UpdateObjectPositions.
  /// </summary>
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
    var direction = NormalizeDirection(parGameState.IsDisoriented, parGameState.DroneMoveDirectionX);
    drone.Velocity = new Vector2(direction * GameRules.DroneBaseSpeed, 0);
    drone.Position = drone.Position.Add(drone.Velocity.Multiply(parDt * droneMultiplier));

    foreach (var crystal in parGameState.CrystalsInternal)
    {
      if (parIsMagnetActive)
      {
        ApplyMagnetEffect(parGameState.DroneInternal, crystal, parDt);
      }

      ApplyBlackHolePull(parGameState.BlackHolesInternal, crystal, parDt);
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

      ApplyBlackHolePull(parGameState.BlackHolesInternal, asteroid, parDt);
      var velocity = asteroid.Velocity.Multiply(multiplier);
      asteroid.Position = asteroid.Position.Add(velocity.Multiply(parDt));
    }

    foreach (var bonus in parGameState.BonusesInternal)
    {
      ApplyBlackHolePull(parGameState.BlackHolesInternal, bonus, parDt);
      var velocity = bonus.Velocity.Multiply(parSpeedMultiplier);
      bonus.Position = bonus.Position.Add(velocity.Multiply(parDt));
    }

    foreach (var blackHole in parGameState.BlackHolesInternal)
    {
      var multiplier = parSpeedMultiplier;

      if (parIsTimeStabilizerActive)
      {
        multiplier *= TimeStabilizerMultiplier;
      }

      var velocity = blackHole.Velocity.Multiply(multiplier);
      blackHole.Position = blackHole.Position.Add(velocity.Multiply(parDt));
    }
  }

  /// <summary>
  /// Выполняет ApplyBlackHolePull<TObject>.
  /// </summary>
  private void ApplyBlackHolePull<TObject>(
    IReadOnlyList<BlackHole> parBlackHoles,
    TObject parObject,
    double parDt)
    where TObject : GameObject
  {
    foreach (var blackHole in parBlackHoles)
    {
      var toCenter = new Vector2(
        blackHole.Position.X - parObject.Position.X,
        blackHole.Position.Y - parObject.Position.Y);

      var distance = toCenter.Length();
      if (distance <= 0 || distance > blackHole.Radius)
        continue;

      var invDist = 1.0 / Math.Max(distance, MinDistanceEps);
      var n = toCenter.Multiply(invDist);

      var t = 1.0 - (distance / blackHole.Radius);
      var boost = 1.0 + InnerBoostK * t * t;

      var accel = n.Multiply(BlackHoleAcceleration * boost);
      parObject.Velocity = parObject.Velocity.Add(accel.Multiply(parDt));

      var relV = new Vector2(
        parObject.Velocity.X - blackHole.Velocity.X,
        parObject.Velocity.Y - blackHole.Velocity.Y);

      var radialSpeed = (relV.X * n.X) + (relV.Y * n.Y);
      var relVRad = n.Multiply(radialSpeed);
      var relVTan = new Vector2(relV.X - relVRad.X, relV.Y - relVRad.Y);

      var tanFactor = Math.Max(0.0, 1.0 - TangentialDampingK * t * parDt);
      var radFactor = Math.Max(0.0, 1.0 - RadialDampingK * t * parDt);

      var newRelV = new Vector2(
        relVRad.X * radFactor + relVTan.X * tanFactor,
        relVRad.Y * radFactor + relVTan.Y * tanFactor);

      parObject.Velocity = new Vector2(
        blackHole.Velocity.X + newRelV.X,
        blackHole.Velocity.Y + newRelV.Y);
    }
  }

  /// <summary>
  /// Выполняет ApplyMagnetEffect.
  /// </summary>
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

  /// <summary>
  /// Выполняет ApplyBlackHoleInfluence.
  /// </summary>
  private void ApplyBlackHoleInfluence(
    GameState parGameState,
    double parDt,
    IEventPublisher parEventPublisher)
  {
    var drone = parGameState.DroneInternal;
    var isInCoreNow = false;

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
      drone.Velocity = new Vector2(
        drone.Velocity.X + (acceleration.X * parDt),
        0);

      if (distance <= blackHole.CoreRadius)
      {
        isInCoreNow = true;
      }
    }

    if (isInCoreNow && !parGameState.IsInBlackHoleCore)
    {
      parGameState.IsInBlackHoleCore = true;
      ActivateDisorientation(parGameState, DisorientationDurationSec);
      drone.Energy -= BlackHoleCoreDamage;
      parEventPublisher.Publish(new DamageTaken("BlackHole", BlackHoleCoreDamage));
    }
    else if (!isInCoreNow && parGameState.IsInBlackHoleCore)
    {
      parGameState.IsInBlackHoleCore = false;
    }
  }

  /// <summary>
  /// Выполняет HandleBlackHoleCoreCollisions.
  /// </summary>
  private void HandleBlackHoleCoreCollisions(
    GameState parGameState,
    IEventPublisher parEventPublisher)
  {
    var crystalsToRemove = new List<Crystal>();
    var asteroidsToRemove = new List<Asteroid>();
    var bonusesToRemove = new List<Bonus>();

    foreach (var blackHole in parGameState.BlackHolesInternal)
    {
      foreach (var crystal in parGameState.CrystalsInternal)
      {
        if (IsWithinCore(crystal, blackHole))
        {
          crystalsToRemove.Add(crystal);
        }
      }

      foreach (var asteroid in parGameState.AsteroidsInternal)
      {
        if (IsWithinCore(asteroid, blackHole))
        {
          asteroidsToRemove.Add(asteroid);
        }
      }

      foreach (var bonus in parGameState.BonusesInternal)
      {
        if (IsWithinCore(bonus, blackHole))
        {
          bonusesToRemove.Add(bonus);
        }
      }
    }

    foreach (var crystal in crystalsToRemove.Distinct())
    {
      parGameState.CrystalsInternal.Remove(crystal);
      parEventPublisher.Publish(new ObjectDespawned(crystal.Id, "BlackHoleCore"));
    }

    foreach (var asteroid in asteroidsToRemove.Distinct())
    {
      parGameState.AsteroidsInternal.Remove(asteroid);
      parEventPublisher.Publish(new ObjectDespawned(asteroid.Id, "BlackHoleCore"));
    }

    foreach (var bonus in bonusesToRemove.Distinct())
    {
      parGameState.BonusesInternal.Remove(bonus);
      parEventPublisher.Publish(new ObjectDespawned(bonus.Id, "BlackHoleCore"));
    }
  }

  /// <summary>
  /// Выполняет IsWithinCore.
  /// </summary>
  private static bool IsWithinCore(GameObject parObject, BlackHole parBlackHole)
  {
    var direction = new Vector2(
      parBlackHole.Position.X - parObject.Position.X,
      parBlackHole.Position.Y - parObject.Position.Y);
    return direction.Length() <= parBlackHole.CoreRadius;
  }

  /// <summary>
  /// Выполняет HandleCollisions.
  /// </summary>
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

  /// <summary>
  /// Выполняет CheckEndConditions.
  /// </summary>
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

    if (IsLevelGoalsMet(parGameState))
    {
      _levelService.AdvanceLevel(parGameState);
      parEventPublisher.Publish(new LevelCompleted("GoalsAndScore"));
    }
  }

  /// <summary>
  /// Выполняет IsLevelGoalsMet.
  /// </summary>
  private static bool IsLevelGoalsMet(GameState parGameState)
  {
    var goals = parGameState.LevelGoals;
    var progress = parGameState.LevelProgress;

    return parGameState.Score >= parGameState.RequiredScore
      && progress.CollectedBlue >= goals.RequiredBlue
      && progress.CollectedGreen >= goals.RequiredGreen
      && progress.CollectedRed >= goals.RequiredRed;
  }

  /// <summary>
  /// Выполняет RemoveOutOfBoundsObjects.
  /// </summary>
  private void RemoveOutOfBoundsObjects(GameState parGameState, IEventPublisher parEventPublisher)
  {
    var bounds = parGameState.WorldBounds;
    var crystalsToRemove = new List<Crystal>();
    var asteroidsToRemove = new List<Asteroid>();
    var bonusesToRemove = new List<Bonus>();
    var blackHolesToRemove = new List<BlackHole>();

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

    foreach (var blackHole in parGameState.BlackHolesInternal)
    {
      if (IsBelowBottom(blackHole, bounds))
      {
        blackHolesToRemove.Add(blackHole);
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

    foreach (var blackHole in blackHolesToRemove)
    {
      parGameState.BlackHolesInternal.Remove(blackHole);
      parEventPublisher.Publish(new ObjectDespawned(blackHole.Id, "OutOfBounds"));
    }
  }

  /// <summary>
  /// Выполняет IsBelowBottom.
  /// </summary>
  private static bool IsBelowBottom(GameObject parObject, WorldBounds parWorldBounds)
  {
    var halfHeight = parObject.Bounds.Height / 2.0;
    var top = parObject.Position.Y - halfHeight;
    return top > parWorldBounds.Bottom;
  }

  /// <summary>
  /// Выполняет GetCrystalPoints.
  /// </summary>
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

  /// <summary>
  /// Выполняет ActivateBonus.
  /// </summary>
  private void ActivateBonus(
    GameState parGameState,
    Bonus parBonus,
    ref bool refTimeStabilizerCollected)
  {
    if (_bonusEffectApplier.Apply(parGameState, parBonus))
    {
      refTimeStabilizerCollected = true;
    }
  }

  /// <summary>
  /// Выполняет ActivateDisorientation.
  /// </summary>
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

  /// <summary>
  /// Выполняет UpdateDisorientation.
  /// </summary>
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

  /// <summary>
  /// Выполняет Intersects.
  /// </summary>
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

  /// <summary>
  /// Выполняет ClampDroneToBounds.
  /// </summary>
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

    if (clampedX < minX)
    {
      clampedX = minX;
    }
    else if (clampedX > maxX)
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

  /// <summary>
  /// Выполняет NormalizeDirection.
  /// </summary>
  private static int NormalizeDirection(bool parIsDisoriented, int parDirection)
  {
    return parIsDisoriented ? -parDirection : parDirection;
  }
}
