using System.Diagnostics;
using CosmicCollector.Core.Events;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Flow;

namespace CosmicCollector.MVC.Loop;

/// <summary>
/// Выполняет игровой цикл в отдельном потоке с фиксированным шагом.
/// </summary>
public sealed class GameLoopRunner : IGameLoopRunner
{
  private const double StepSeconds = 1.0 / 60.0;
  private readonly IGameFlowController _flowController;
  private readonly CommandQueue _commandQueue;
  private readonly IEventBus _eventBus;
  private readonly Action<double>? _updateCallback;
  private readonly object _stateLock = new();
  private Thread? _thread;
  private bool _isRunning;

  /// <summary>
  /// Инициализирует игровой цикл.
  /// </summary>
  /// <param name="parFlowController">Контроллер игрового процесса.</param>
  /// <param name="parCommandQueue">Очередь команд.</param>
  /// <param name="parEventBus">Шина событий.</param>
  /// <param name="parUpdateCallback">Колбэк обновления мира.</param>
  public GameLoopRunner(
    IGameFlowController parFlowController,
    CommandQueue parCommandQueue,
    IEventBus parEventBus,
    Action<double>? parUpdateCallback)
  {
    _flowController = parFlowController;
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

        if (_flowController.AllowsWorldTick)
        {
          _updateCallback?.Invoke(StepSeconds);
        }

        var tickNo = _flowController.GameState.AdvanceTick();
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
      _flowController.HandleCommand(command);
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
