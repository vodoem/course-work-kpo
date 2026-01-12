using System;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Events;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Randomization;
using CosmicCollector.Core.Services;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Loop;

namespace CosmicCollector.Avalonia.Infrastructure;

/// <summary>
/// Управляет жизненным циклом игровой сессии для Avalonia UI.
/// </summary>
public sealed class GameRuntime
{
  private readonly object _lockObject = new();
  private GameState? _gameState;
  private EventBus? _eventBus;
  private CommandQueue? _commandQueue;
  private GameLoopRunner? _gameLoopRunner;
  private GameWorldUpdateService? _updateService;
  private bool _isRunning;

  /// <summary>
  /// Возвращает шину событий текущей сессии.
  /// </summary>
  public EventBus EventBus => _eventBus ?? throw new InvalidOperationException("Сессия не запущена.");

  /// <summary>
  /// Возвращает очередь команд текущей сессии.
  /// </summary>
  public CommandQueue CommandQueue => _commandQueue ?? throw new InvalidOperationException("Сессия не запущена.");

  /// <summary>
  /// Возвращает состояние игры текущей сессии.
  /// </summary>
  public GameState GameState => _gameState ?? throw new InvalidOperationException("Сессия не запущена.");

  /// <summary>
  /// Запускает игровую сессию.
  /// </summary>
  public void Start()
  {
    lock (_lockObject)
    {
      if (_isRunning)
      {
        return;
      }

      _isRunning = true;
      _gameState = BuildInitialGameState();
      _eventBus = new EventBus();
      _commandQueue = new CommandQueue();
      _updateService = new GameWorldUpdateService(new DefaultRandomProvider(), SpawnConfig.Default);
      _gameLoopRunner = new GameLoopRunner(_gameState, _commandQueue, _eventBus, UpdateWorld);

      _eventBus.Publish(new GameStarted(_gameState.CurrentLevel));
      _gameLoopRunner.Start();
    }
  }

  /// <summary>
  /// Останавливает игровую сессию.
  /// </summary>
  public void Stop()
  {
    lock (_lockObject)
    {
      if (!_isRunning)
      {
        return;
      }

      _isRunning = false;
      _gameLoopRunner?.Stop();
      _gameLoopRunner = null;
      _updateService = null;
      _gameState = null;
      _eventBus = null;
      _commandQueue = null;
    }
  }

  /// <summary>
  /// Возвращает снимок состояния игры.
  /// </summary>
  /// <returns>Снимок состояния.</returns>
  public Core.Snapshots.GameSnapshot GetSnapshot()
  {
    return GameState.GetSnapshot();
  }

  private void UpdateWorld(double parDt)
  {
    var gameState = _gameState;
    var eventBus = _eventBus;
    var updateService = _updateService;

    if (gameState is null || eventBus is null || updateService is null)
    {
      return;
    }

    updateService.Update(gameState, parDt, gameState.CurrentLevel, eventBus);
  }

  private static GameState BuildInitialGameState()
  {
    var bounds = new WorldBounds(0, 0, 800, 600);
    var droneBounds = new Aabb(32, 32);
    var startPosition = new Vector2(
      (bounds.Left + bounds.Right) / 2.0,
      bounds.Bottom - (droneBounds.Height / 2.0));
    var drone = new Drone(Guid.NewGuid(), startPosition, Vector2.Zero, droneBounds, 100);
    return new GameState(drone, bounds);
  }
}
