using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Randomization;

namespace CosmicCollector.Core.Services;

/// <summary>
/// Отвечает за детерминированный спавн объектов.
/// </summary>
public sealed class SpawnSystem
{
  private readonly IRandomProvider _randomProvider;
  private readonly SpawnConfig _config;
  private readonly IGameObjectFactory _factory;

  /// <summary>
  /// Инициализирует систему спавна.
  /// </summary>
  /// <param name="parRandomProvider">Провайдер случайных значений.</param>
  /// <param name="parConfig">Конфигурация спавна.</param>
  /// <param name="parFactory">Фабрика игровых объектов.</param>
  public SpawnSystem(
    IRandomProvider parRandomProvider,
    SpawnConfig parConfig,
    IGameObjectFactory? parFactory = null)
  {
    _randomProvider = parRandomProvider;
    _config = parConfig;
    _factory = parFactory ?? new DefaultGameObjectFactory();
  }

  /// <summary>
  /// Пытается заспавнить объекты на текущем тике.
  /// </summary>
  /// <param name="parGameState">Состояние игры.</param>
  /// <param name="parLevel">Номер уровня.</param>
  /// <param name="parTickCount">Номер тика.</param>
  /// <returns>Список заспавненных объектов.</returns>
  public IReadOnlyList<SpawnedObject> TrySpawn(GameState parGameState, int parLevel, long parTickCount)
  {
    if (!_config.IsEnabled)
    {
      return Array.Empty<SpawnedObject>();
    }

    var results = new List<SpawnedObject>();

    TrySpawnCrystal(parGameState, parLevel, parTickCount, results);
    TrySpawnAsteroid(parGameState, parLevel, parTickCount, results);
    TrySpawnBonus(parGameState, parLevel, parTickCount, results);
    TrySpawnBlackHole(parGameState, parLevel, parTickCount, results);

    return results;
  }

  /// <summary>
  /// Выполняет TrySpawnCrystal.
  /// </summary>
  private void TrySpawnCrystal(
    GameState parGameState,
    int parLevel,
    long parTickCount,
    ICollection<SpawnedObject> parResults)
  {
    if (!IsTickForSpawn(parTickCount, _config.CrystalIntervalTicks, parLevel))
    {
      return;
    }

    var maxActive = GetScaledMaxActive(_config.MaxActiveCrystals, parLevel);

    if (parGameState.CrystalsInternal.Count >= maxActive)
    {
      return;
    }

    if (!TryFindSpawnPosition(parGameState, _config.CrystalBounds, out var position))
    {
      return;
    }

    var velocity = new Vector2(0, GetScaledSpeed(_config.CrystalBaseSpeed, parLevel));
    var type = PickWeighted(_config.CrystalTypeWeights);
    var crystal = _factory.CreateCrystal(Guid.NewGuid(), position, velocity, _config.CrystalBounds, type);
    parGameState.CrystalsInternal.Add(crystal);
    parResults.Add(new SpawnedObject(nameof(Crystal), crystal.Id));
  }

  /// <summary>
  /// Выполняет TrySpawnAsteroid.
  /// </summary>
  private void TrySpawnAsteroid(
    GameState parGameState,
    int parLevel,
    long parTickCount,
    ICollection<SpawnedObject> parResults)
  {
    if (!IsTickForSpawn(parTickCount, _config.AsteroidIntervalTicks, parLevel))
    {
      return;
    }

    var maxActive = GetScaledMaxActive(_config.MaxActiveAsteroids, parLevel);

    if (parGameState.AsteroidsInternal.Count >= maxActive)
    {
      return;
    }

    if (!TryFindSpawnPosition(parGameState, _config.AsteroidBounds, out var position))
    {
      return;
    }

    var baseSpeed = _config.AsteroidBaseSpeed * _config.AsteroidSpeedMultiplier;
    var velocity = new Vector2(0, GetScaledSpeed(baseSpeed, parLevel));
    var asteroid = _factory.CreateAsteroid(Guid.NewGuid(), position, velocity, _config.AsteroidBounds);
    parGameState.AsteroidsInternal.Add(asteroid);
    parResults.Add(new SpawnedObject(nameof(Asteroid), asteroid.Id));
  }

  /// <summary>
  /// Выполняет TrySpawnBonus.
  /// </summary>
  private void TrySpawnBonus(
    GameState parGameState,
    int parLevel,
    long parTickCount,
    ICollection<SpawnedObject> parResults)
  {
    if (!IsTickForSpawn(parTickCount, _config.BonusIntervalTicks, parLevel))
    {
      return;
    }

    var maxActive = GetScaledMaxActive(_config.MaxActiveBonuses, parLevel);

    if (parGameState.BonusesInternal.Count >= maxActive)
    {
      return;
    }

    if (!TryFindSpawnPosition(parGameState, _config.BonusBounds, out var position))
    {
      return;
    }

    var velocity = new Vector2(0, GetScaledSpeed(_config.BonusBaseSpeed, parLevel));
    var type = PickWeighted(_config.BonusTypeWeights);
    var bonus = _factory.CreateBonus(
      Guid.NewGuid(),
      position,
      velocity,
      _config.BonusBounds,
      type,
      _config.BonusDurationSec);
    parGameState.BonusesInternal.Add(bonus);
    parResults.Add(new SpawnedObject(nameof(Bonus), bonus.Id));
  }

  /// <summary>
  /// Выполняет TrySpawnBlackHole.
  /// </summary>
  private void TrySpawnBlackHole(
    GameState parGameState,
    int parLevel,
    long parTickCount,
    ICollection<SpawnedObject> parResults)
  {
    if (!IsTickForSpawn(parTickCount, _config.BlackHoleIntervalTicks, parLevel))
    {
      return;
    }

    var maxActive = GetScaledMaxActive(_config.MaxActiveBlackHoles, parLevel);

    if (parGameState.BlackHolesInternal.Count >= maxActive)
    {
      return;
    }

    if (!TryFindSpawnPosition(parGameState, _config.BlackHoleBounds, out var position))
    {
      return;
    }

    var velocity = new Vector2(0, GetScaledSpeed(_config.BlackHoleBaseSpeed, parLevel));
    var blackHole = _factory.CreateBlackHole(
      Guid.NewGuid(),
      position,
      velocity,
      _config.BlackHoleBounds,
      _config.BlackHoleRadius,
      _config.BlackHoleCoreRadius);
    parGameState.BlackHolesInternal.Add(blackHole);
    parResults.Add(new SpawnedObject(nameof(BlackHole), blackHole.Id));
  }

  /// <summary>
  /// Выполняет IsTickForSpawn.
  /// </summary>
  private bool IsTickForSpawn(long parTickCount, int parBaseInterval, int parLevel)
  {
    if (parBaseInterval <= 0)
    {
      return false;
    }

    var interval = Math.Max(1, parBaseInterval - ((parLevel - 1) * _config.IntervalDecreasePerLevel));
    return parTickCount % interval == 0;
  }

  /// <summary>
  /// Выполняет GetScaledMaxActive.
  /// </summary>
  private int GetScaledMaxActive(int parBaseMax, int parLevel)
  {
    return parBaseMax + ((parLevel - 1) * _config.MaxActiveIncreasePerLevel);
  }

  /// <summary>
  /// Выполняет GetScaledSpeed.
  /// </summary>
  private double GetScaledSpeed(double parBaseSpeed, int parLevel)
  {
    var multiplier = 1.0 + (0.015 * (parLevel - 1));
    return parBaseSpeed * multiplier;
  }

  /// <summary>
  /// Выполняет TryFindSpawnPosition.
  /// </summary>
  private bool TryFindSpawnPosition(GameState parGameState, Aabb parBounds, out Vector2 parPosition)
  {
    var halfWidth = parBounds.Width / 2.0;
    var halfHeight = parBounds.Height / 2.0;
    var world = parGameState.WorldBounds;
    var minX = world.Left + halfWidth + _config.SpawnMargin;
    var maxX = world.Right - halfWidth - _config.SpawnMargin;

    if (minX > maxX)
    {
      parPosition = Vector2.Zero;
      return false;
    }

    var minInt = (int)Math.Ceiling(minX);
    var maxInt = (int)Math.Floor(maxX);

    if (minInt > maxInt)
    {
      parPosition = Vector2.Zero;
      return false;
    }

    var centerY = world.Top - (halfHeight + _config.SpawnGap);

    for (var attempt = 0; attempt < _config.MaxSpawnAttempts; attempt++)
    {
      var x = _randomProvider.NextInt(minInt, maxInt);
      var candidate = new Vector2(x, centerY);

      if (!HasCollision(parGameState, candidate, parBounds))
      {
        parPosition = candidate;
        return true;
      }
    }

    parPosition = Vector2.Zero;
    return false;
  }

  /// <summary>
  /// Выполняет HasCollision.
  /// </summary>
  private bool HasCollision(GameState parGameState, Vector2 parPosition, Aabb parBounds)
  {
    if (Intersects(parPosition, parBounds, parGameState.DroneInternal.Position, parGameState.DroneInternal.Bounds))
    {
      return true;
    }

    foreach (var crystal in parGameState.CrystalsInternal)
    {
      if (Intersects(parPosition, parBounds, crystal.Position, crystal.Bounds))
      {
        return true;
      }
    }

    foreach (var asteroid in parGameState.AsteroidsInternal)
    {
      if (Intersects(parPosition, parBounds, asteroid.Position, asteroid.Bounds))
      {
        return true;
      }
    }

    foreach (var bonus in parGameState.BonusesInternal)
    {
      if (Intersects(parPosition, parBounds, bonus.Position, bonus.Bounds))
      {
        return true;
      }
    }

    foreach (var blackHole in parGameState.BlackHolesInternal)
    {
      if (Intersects(parPosition, parBounds, blackHole.Position, blackHole.Bounds))
      {
        return true;
      }
    }

    return false;
  }

  /// <summary>
  /// Выполняет Intersects.
  /// </summary>
  private static bool Intersects(Vector2 parLeftPosition, Aabb parLeftBounds, Vector2 parRightPosition, Aabb parRightBounds)
  {
    var halfWidthLeft = parLeftBounds.Width / 2.0;
    var halfHeightLeft = parLeftBounds.Height / 2.0;
    var halfWidthRight = parRightBounds.Width / 2.0;
    var halfHeightRight = parRightBounds.Height / 2.0;

    var deltaX = Math.Abs(parLeftPosition.X - parRightPosition.X);
    var deltaY = Math.Abs(parLeftPosition.Y - parRightPosition.Y);

    return deltaX <= (halfWidthLeft + halfWidthRight)
      && deltaY <= (halfHeightLeft + halfHeightRight);
  }

  /// <summary>
  /// Выполняет PickWeighted<T>.
  /// </summary>
  private T PickWeighted<T>(IReadOnlyList<WeightedOption<T>> parWeights)
  {
    if (parWeights.Count == 0)
    {
      return default!;
    }

    var total = 0;

    foreach (var entry in parWeights)
    {
      total += entry.Weight;
    }

    if (total <= 0)
    {
      return parWeights[0].Value;
    }

    var roll = _randomProvider.NextInt(1, total);
    var cumulative = 0;

    foreach (var entry in parWeights)
    {
      cumulative += entry.Weight;

      if (roll <= cumulative)
      {
        return entry.Value;
      }
    }

    return parWeights[0].Value;
  }
}
