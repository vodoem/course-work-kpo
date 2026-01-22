using System;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Events;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Randomization;
using CosmicCollector.Core.Services;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Flow;
using CosmicCollector.MVC.Loop;

namespace CosmicCollector.Avalonia.Infrastructure;

/// <summary>
/// Управляет жизненным циклом игровой сессии для Avalonia UI.
/// </summary>
public sealed class GameRuntime
{
  private static readonly object InstanceLock = new();
  private static GameRuntime? _instance;
  private readonly object _lockObject = new();
  private GameState? _gameState;
  private EventBus? _eventBus;
  private CommandQueue? _commandQueue;
  private GameLoopRunner? _gameLoopRunner;
  private GameWorldUpdateService? _updateService;
  private GameFlowController? _flowController;
  private bool _isRunning;

  private GameRuntime()
  {
  }

  /// <summary>
  /// Возвращает активный экземпляр runtime.
  /// </summary>
  public static GameRuntime Instance
  {
    get
    {
      lock (InstanceLock)
      {
        _instance ??= new GameRuntime();
        return _instance;
      }
    }
  }

  /// <summary>
  /// Создаёт новый runtime и останавливает предыдущий.
  /// </summary>
  /// <returns>Новый runtime.</returns>
  public static GameRuntime CreateNew()
  {
    lock (InstanceLock)
    {
      _instance?.Stop();
      _instance = new GameRuntime();
      return _instance;
    }
  }

  /// <summary>
  /// Сбрасывает runtime для тестов.
  /// </summary>
  internal static void ResetForTests()
  {
    lock (InstanceLock)
    {
      _instance?.Stop();
      _instance = null;
    }
  }

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
  /// Возвращает текущее состояние игрового процесса.
  /// </summary>
  public IGameFlowState CurrentState => _flowController?.CurrentState
                                        ?? throw new InvalidOperationException("Сессия не запущена.");

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
      _flowController = new GameFlowController(_gameState, _eventBus, new PlayingState());
      _gameLoopRunner = new GameLoopRunner(_flowController, _commandQueue, _eventBus, UpdateWorld);

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
      _flowController?.Dispose();
      _flowController = null;
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

  /// <summary>
  /// Выполняет переход между состояниями игрового процесса.
  /// </summary>
  /// <param name="parNextState">Следующее состояние.</param>
  public void TransitionTo(IGameFlowState parNextState)
  {
    if (_flowController is null)
    {
      throw new InvalidOperationException("Сессия не запущена.");
    }

    _flowController.TransitionTo(parNextState);
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
    var bounds = new WorldBounds(
      0,
      0,
      GameLayoutConstants.WorldWidth,
      GameLayoutConstants.WorldHeight);
    var droneBounds = new Aabb(80, 80);
    var drone = new Drone(Guid.NewGuid(), Vector2.Zero, Vector2.Zero, droneBounds, 100);
    InitializeDrone(drone, bounds);
    return new GameState(drone, bounds);
  }

  private static void InitializeDrone(Drone parDrone, WorldBounds parBounds)
  {
    var halfHeight = parDrone.Bounds.Height / 2.0;
    var centerX = (parBounds.Left + parBounds.Right) / 2.0;
    var bottomY = parBounds.Bottom - halfHeight - 1;
    parDrone.Position = new Vector2(centerX, bottomY);
  }
}
