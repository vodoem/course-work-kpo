using CosmicCollector.Core.Events;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Flow;

namespace CosmicCollector.MVC.Loop;

/// <summary>
/// Управляемый игровой цикл без ожиданий, полезный для тестов.
/// </summary>
public sealed class ManualGameLoopRunner
{
  private const double StepSeconds = 1.0 / 60.0;
  private readonly IGameFlowController _flowController;
  private readonly CommandQueue _commandQueue;
  private readonly IEventBus _eventBus;
  private readonly Action<double>? _updateCallback;

  /// <summary>
  /// Инициализирует управляемый игровой цикл.
  /// </summary>
  /// <param name="parFlowController">Контроллер игрового процесса.</param>
  /// <param name="parCommandQueue">Очередь команд.</param>
  /// <param name="parEventBus">Шина событий.</param>
  /// <param name="parUpdateCallback">Колбэк обновления мира.</param>
  public ManualGameLoopRunner(
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

  /// <summary>
  /// Выполняет один тик без ожиданий.
  /// </summary>
  public void TickOnce()
  {
    ProcessCommands();

    if (_flowController.AllowsWorldTick)
    {
      _updateCallback?.Invoke(StepSeconds);
    }

    var tickNo = _flowController.GameState.AdvanceTick();
    _eventBus.Publish(new GameTick(StepSeconds, tickNo));
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
}
