using System.Diagnostics;
using CosmicCollector.Core.Events;
using CosmicCollector.Core.Model;
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
  private readonly IGameCommandHandler _commandHandler;
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
  /// <param name="parCommandHandler">Обработчик команд.</param>
  public GameLoopRunner(
    GameState parGameState,
    CommandQueue parCommandQueue,
    IEventBus parEventBus,
    Action<double>? parUpdateCallback,
    IGameCommandHandler parCommandHandler)
  {
    _gameState = parGameState;
    _commandQueue = parCommandQueue;
    _eventBus = parEventBus;
    _updateCallback = parUpdateCallback;
    _commandHandler = parCommandHandler;
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

  /// <summary>
  /// Выполняет RunLoop.
  /// </summary>
  private void RunLoop()
  {
    var stopwatch = Stopwatch.StartNew();
    var stepDuration = TimeSpan.FromSeconds(StepSeconds);
    var lastTimestamp = stopwatch.Elapsed;
    var accumulated = TimeSpan.Zero;

    while (IsRunning())
    {
      var currentTimestamp = stopwatch.Elapsed;
      accumulated += currentTimestamp - lastTimestamp;
      lastTimestamp = currentTimestamp;

      while (accumulated >= stepDuration)
      {
        ProcessCommands();

        _updateCallback?.Invoke(StepSeconds);

        var tickNo = _gameState.AdvanceTick();
        _eventBus.Publish(new GameTick(StepSeconds, tickNo));

        accumulated -= stepDuration;
      }

      var remaining = stepDuration - accumulated;
      if (remaining > TimeSpan.Zero)
      {
        Thread.Sleep(remaining);
      }
    }
  }

  /// <summary>
  /// Выполняет ProcessCommands.
  /// </summary>
  private void ProcessCommands()
  {
    var commands = _commandQueue.DrainAll();

    foreach (var command in commands)
    {
      _commandHandler.HandleCommand(command);
    }
  }

  /// <summary>
  /// Выполняет IsRunning.
  /// </summary>
  private bool IsRunning()
  {
    lock (_stateLock)
    {
      return _isRunning;
    }
  }
}
