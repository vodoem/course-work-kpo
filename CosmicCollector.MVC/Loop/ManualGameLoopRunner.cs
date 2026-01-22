using CosmicCollector.Core.Events;
using CosmicCollector.Core.Model;
using CosmicCollector.MVC.Eventing;

namespace CosmicCollector.MVC.Loop;

/// <summary>
/// Управляемый игровой цикл без ожиданий, полезный для тестов.
/// </summary>
public sealed class ManualGameLoopRunner
{
  private const double StepSeconds = 1.0 / 60.0;
  private readonly GameState _gameState;
  private readonly CommandQueue _commandQueue;
  private readonly IEventBus _eventBus;
  private readonly Action<double>? _updateCallback;
  private readonly IGameCommandHandler _commandHandler;

  /// <summary>
  /// Инициализирует управляемый игровой цикл.
  /// </summary>
  /// <param name="parGameState">Состояние игры.</param>
  /// <param name="parCommandQueue">Очередь команд.</param>
  /// <param name="parEventBus">Шина событий.</param>
  /// <param name="parUpdateCallback">Колбэк обновления мира.</param>
  /// <param name="parCommandHandler">Обработчик команд.</param>
  public ManualGameLoopRunner(
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

  /// <summary>
  /// Выполняет один тик без ожиданий.
  /// </summary>
  public void TickOnce()
  {
    ProcessCommands();

    _updateCallback?.Invoke(StepSeconds);

    var tickNo = _gameState.AdvanceTick();
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
      _commandHandler.HandleCommand(command);
    }
  }
}
