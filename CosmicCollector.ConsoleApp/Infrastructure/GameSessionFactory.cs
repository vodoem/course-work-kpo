using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Randomization;
using CosmicCollector.Core.Services;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Loop;

namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Создаёт игровые сессии для консольного UI.
/// </summary>
public sealed class GameSessionFactory : IGameSessionFactory
{
  private const int DefaultLevel = 1;

  /// <inheritdoc />
  public GameSession Create()
  {
    var gameState = new GameState();
    var eventBus = new EventBus();
    var commandQueue = new CommandQueue();
    var randomProvider = new DefaultRandomProvider();
    var spawnConfig = BuildSpawnConfig();
    var updateService = new GameWorldUpdateService(randomProvider, spawnConfig);
    var snapshotProvider = new GameSnapshotProvider(gameState);
    InitializeDrone(gameState);
    var gameLoopRunner = new GameLoopRunner(
      gameState,
      commandQueue,
      eventBus,
      parDt => updateService.Update(gameState, parDt, DefaultLevel, eventBus));

    return new GameSession(
      eventBus,
      gameLoopRunner,
      snapshotProvider,
      gameState.WorldBounds,
      DefaultLevel);
  }

  private static SpawnConfig BuildSpawnConfig()
  {
    var defaultConfig = SpawnConfig.Default;
    var speedMultiplier = 3.0;
    var intervalDivider = 2;

    return new SpawnConfig(
      defaultConfig.IsEnabled,
      defaultConfig.MaxActiveCrystals,
      defaultConfig.MaxActiveAsteroids,
      defaultConfig.MaxActiveBonuses,
      defaultConfig.MaxActiveBlackHoles,
      Math.Max(1, defaultConfig.CrystalIntervalTicks / intervalDivider),
      Math.Max(1, defaultConfig.AsteroidIntervalTicks / intervalDivider),
      Math.Max(1, defaultConfig.BonusIntervalTicks / intervalDivider),
      Math.Max(1, defaultConfig.BlackHoleIntervalTicks / intervalDivider),
      defaultConfig.IntervalDecreasePerLevel,
      defaultConfig.MaxActiveIncreasePerLevel,
      defaultConfig.SpawnMargin,
      defaultConfig.SpawnGap,
      defaultConfig.MaxSpawnAttempts,
      defaultConfig.CrystalBounds,
      defaultConfig.AsteroidBounds,
      defaultConfig.BonusBounds,
      defaultConfig.BlackHoleBounds,
      defaultConfig.CrystalBaseSpeed * speedMultiplier,
      defaultConfig.AsteroidBaseSpeed * speedMultiplier,
      defaultConfig.BonusBaseSpeed * speedMultiplier,
      defaultConfig.BlackHoleBaseSpeed * speedMultiplier,
      defaultConfig.BlackHoleRadius,
      defaultConfig.BlackHoleCoreRadius,
      defaultConfig.BonusDurationSec,
      defaultConfig.AsteroidSpeedMultiplier,
      defaultConfig.CrystalTypeWeights,
      defaultConfig.BonusTypeWeights);
  }

  private static void InitializeDrone(GameState parGameState)
  {
    var drone = parGameState.Drone;
    var bounds = parGameState.WorldBounds;
    var halfWidth = drone.Bounds.Width / 2.0;
    var halfHeight = drone.Bounds.Height / 2.0;
    var centerX = (bounds.Left + bounds.Right) / 2.0;
    var bottomY = bounds.Bottom - halfHeight - 1;
    drone.Position = new Vector2(centerX, bottomY);
  }
}
