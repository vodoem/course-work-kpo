using System;
using System.Collections.Generic;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Events;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Randomization;
using CosmicCollector.Core.Services;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Flow;
using CosmicCollector.MVC.Loop;

namespace CosmicCollector.MVC.Runtime;

/// <summary>
/// Управляет жизненным циклом игровой сессии и предоставляет доступ к активному runtime.
/// </summary>
public sealed class GameRuntime : IGameCommandHandler, IGameFlowRuntime
{
  private static readonly object InstanceLock = new();
  private static GameRuntime? _instance;

  private readonly object _lockObject = new();
  private readonly List<IDisposable> _subscriptions = new();
  private GameState? _gameState;
  private EventBus? _eventBus;
  private CommandQueue? _commandQueue;
  private IGameLoopRunner? _gameLoopRunner;
  private GameWorldUpdateService? _updateService;
  private ResumeCountdownService? _resumeCountdownService;
  private IGameFlowState? _currentState;
  private bool _isRunning;

  private GameRuntime()
  {
  }

  /// <summary>
  /// Возвращает текущий активный runtime.
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
  /// Создаёт новый runtime, останавливая предыдущую сессию.
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
  /// Название текущего состояния игрового потока.
  /// </summary>
  public string CurrentStateName => _currentState?.Name ?? "Unknown";

  /// <inheritdoc />
  IEventPublisher IGameFlowRuntime.EventPublisher => EventBus;

  /// <inheritdoc />
  GameState IGameFlowRuntime.GameState => GameState;

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
      _resumeCountdownService = new ResumeCountdownService();
      _currentState = new PlayingState();
      _currentState.OnEnter(this);
      _gameLoopRunner = new GameLoopRunner(_gameState, _commandQueue, _eventBus, ProcessTick, this);

      RegisterStateTransitions();

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
      _resumeCountdownService = null;
      _currentState = null;
      DisposeSubscriptions();
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

  /// <inheritdoc />
  public void HandleCommand(IGameCommand parCommand)
  {
    _currentState?.HandleCommand(this, parCommand);
  }

  /// <inheritdoc />
  public bool ProcessResumeCountdown(double parDt)
  {
    if (_gameState is null || _eventBus is null || _resumeCountdownService is null)
    {
      return false;
    }

    return _resumeCountdownService.Process(_gameState, parDt, _eventBus);
  }

  /// <inheritdoc />
  public void TransitionTo(IGameFlowState parNextState)
  {
    if (parNextState is null)
    {
      throw new ArgumentNullException(nameof(parNextState));
    }

    lock (_lockObject)
    {
      _currentState?.OnExit(this);
      _currentState = parNextState;
      _currentState.OnEnter(this);
    }
  }

  private void RegisterStateTransitions()
  {
    if (_eventBus is null)
    {
      return;
    }

    _subscriptions.Add(_eventBus.Subscribe<GameOver>(_ => TransitionTo(new GameOverState())));
    _subscriptions.Add(_eventBus.Subscribe<LevelCompleted>(_ => TransitionTo(new LevelCompletedState())));
  }

  private void DisposeSubscriptions()
  {
    foreach (var subscription in _subscriptions)
    {
      subscription.Dispose();
    }

    _subscriptions.Clear();
  }

  private void ProcessTick(double parDt)
  {
    var state = _currentState;
    if (state is null)
    {
      return;
    }

    state.OnTick(this, parDt);

    if (state.AllowsWorldTick)
    {
      UpdateWorld(parDt);
    }
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
      GameWorldLayoutConstants.WorldWidth,
      GameWorldLayoutConstants.WorldHeight);
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
