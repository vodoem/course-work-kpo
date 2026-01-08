using System.Diagnostics;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Events;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Services;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;

namespace CosmicCollector.MVC.Loop;

/// <summary>
/// Выполняет игровой цикл в отдельном потоке с фиксированным шагом.
/// </summary>
public sealed class GameLoopRunner : IGameLoopRunner
{
  private const double StepSeconds = 1.0 / 60.0;
  private readonly GameState _gameState;
  private readonly CommandQueue _commandQueue;
  private readonly IEventBus _eventBus;
  private readonly Action<double>? _updateCallback;
  private readonly object _stateLock = new();
  private Thread? _thread;
  private bool _isRunning;

  /// <summary>
  /// Инициализирует игровой цикл.
  /// </summary>
  /// <param name="parGameState">Состояние игры.</param>
  /// <param name="parCommandQueue">Очередь команд.</param>
  /// <param name="parEventBus">Шина событий.</param>
  /// <param name="parUpdateCallback">Колбэк обновления мира.</param>
  public GameLoopRunner(
    GameState parGameState,
    CommandQueue parCommandQueue,
    IEventBus parEventBus,
    Action<double>? parUpdateCallback)
  {
    _gameState = parGameState;
    _commandQueue = parCommandQueue;
    _eventBus = parEventBus;
    _updateCallback = parUpdateCallback;
  }

  /// <inheritdoc />
  public void Start()
  {
    lock (_stateLock)
    {
      if (_isRunning)
      {
        return;
      }

      _isRunning = true;
      _thread = new Thread(RunLoop)
      {
        IsBackground = true,
        Name = "GameLoop"
      };
      _thread.Start();
    }
  }

  /// <inheritdoc />
  public void Stop()
  {
    Thread? threadToJoin;

    lock (_stateLock)
    {
      _isRunning = false;
      threadToJoin = _thread;
      _thread = null;
    }

    threadToJoin?.Join();
  }

  /// <inheritdoc />
  public void Dispose()
  {
    Stop();
  }

  private void RunLoop()
  {
    var stopwatch = Stopwatch.StartNew();

    while (IsRunning())
    {
      var tickStart = stopwatch.Elapsed;

      ProcessCommands();

      if (!_gameState.IsPaused)
      {
        _updateCallback?.Invoke(StepSeconds);
      }

      var tickNo = _gameState.AdvanceTick();
      _eventBus.Publish(new GameTick(StepSeconds, tickNo));

      var target = tickStart + TimeSpan.FromSeconds(StepSeconds);
      var remaining = target - stopwatch.Elapsed;

      if (remaining > TimeSpan.Zero)
      {
        Thread.Sleep(remaining);
      }
    }
  }

  private void ProcessCommands()
  {
    var commands = _commandQueue.DrainAll();

    foreach (var command in commands)
    {
      if (command is TogglePauseCommand)
      {
        var isPaused = _gameState.TogglePause();
        _eventBus.Publish(new PauseToggled(isPaused));
        continue;
      }

      if (command is MoveLeftCommand)
      {
        ApplyMoveCommand(-1);
        continue;
      }

      if (command is MoveRightCommand)
      {
        ApplyMoveCommand(1);
      }
    }
  }

  private void ApplyMoveCommand(int parDirection)
  {
    var snapshot = _gameState.GetSnapshot();
    var direction = NormalizeDirection(snapshot.parDrone.parIsDisoriented, parDirection);
    _gameState.SetDroneVelocity(new Vector2(direction * GameRules.DroneBaseSpeed, snapshot.parDrone.parVelocity.Y));
  }

  private static int NormalizeDirection(bool parIsDisoriented, int parDirection)
  {
    return parIsDisoriented ? -parDirection : parDirection;
  }

  private bool IsRunning()
  {
    lock (_stateLock)
    {
      return _isRunning;
    }
  }
}
