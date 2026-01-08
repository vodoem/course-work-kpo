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
}
