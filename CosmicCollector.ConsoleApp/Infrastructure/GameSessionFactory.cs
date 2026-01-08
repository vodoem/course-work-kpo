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
    var updateService = new GameWorldUpdateService(randomProvider, SpawnConfig.Default);
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
