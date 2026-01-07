using CosmicCollector.Core.Events;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;

namespace CosmicCollector.MVC.Loop;

/// <summary>
/// Управляемый игровой цикл без ожиданий, полезный для тестов.
/// </summary>
public sealed class ManualGameLoopRunner
{
  private const double StepSeconds = 1.0 / 60.0;
  private const double DroneSpeed = 10.0;
  private readonly GameState _gameState;
  private readonly CommandQueue _commandQueue;
  private readonly IEventBus _eventBus;
  private readonly Action<double>? _updateCallback;

  /// <summary>
  /// Инициализирует управляемый игровой цикл.
  /// </summary>
  /// <param name="parGameState">Состояние игры.</param>
  /// <param name="parCommandQueue">Очередь команд.</param>
  /// <param name="parEventBus">Шина событий.</param>
  /// <param name="parUpdateCallback">Колбэк обновления мира.</param>
  public ManualGameLoopRunner(
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

  /// <summary>
  /// Выполняет один тик без ожиданий.
  /// </summary>
  public void TickOnce()
  {
    ProcessCommands();

    if (!_gameState.IsPaused)
    {
      _updateCallback?.Invoke(StepSeconds);
    }

    var tickNo = _gameState.AdvanceTick();
    _eventBus.Publish(new GameTick(StepSeconds, tickNo));
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
    var direction = _gameState.IsDisoriented ? -parDirection : parDirection;
    _gameState.DroneInternal.Velocity = new Vector2(direction * DroneSpeed, _gameState.DroneInternal.Velocity.Y);
  }
}
