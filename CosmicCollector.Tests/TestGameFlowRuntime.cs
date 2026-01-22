using CosmicCollector.Core.Events;
using CosmicCollector.Core.Model;
using CosmicCollector.Core.Services;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Flow;
using CosmicCollector.MVC.Loop;

namespace CosmicCollector.Tests;

/// <summary>
/// Тестовый runtime для проверки состояний без запуска потоков.
/// </summary>
internal sealed class TestGameFlowRuntime : IGameFlowRuntime, IGameCommandHandler
{
  private readonly GameWorldUpdateService _updateService;
  private readonly ResumeCountdownService _resumeCountdownService;
  private IGameFlowState _currentState;

  /// <summary>
  /// Инициализирует тестовый runtime.
  /// </summary>
  /// <param name="parGameState">Состояние игры.</param>
  /// <param name="parEventBus">Шина событий.</param>
  /// <param name="parUpdateService">Сервис обновления мира.</param>
  /// <param name="parResumeCountdownService">Сервис обратного отсчёта.</param>
  /// <param name="parInitialState">Начальное состояние.</param>
  public TestGameFlowRuntime(
    GameState parGameState,
    EventBus parEventBus,
    GameWorldUpdateService parUpdateService,
    ResumeCountdownService parResumeCountdownService,
    IGameFlowState parInitialState)
  {
    GameState = parGameState;
    EventPublisher = parEventBus;
    _updateService = parUpdateService;
    _resumeCountdownService = parResumeCountdownService;
    _currentState = parInitialState;
    _currentState.OnEnter(this);
  }

  /// <summary>
  /// Текущее состояние.
  /// </summary>
  public IGameFlowState CurrentState => _currentState;

  /// <inheritdoc />
  public GameState GameState { get; }

  /// <inheritdoc />
  public IEventPublisher EventPublisher { get; }

  /// <summary>
  /// Выполняет один тик обновления.
  /// </summary>
  /// <param name="parDt">Длительность шага.</param>
  public void Tick(double parDt)
  {
    _currentState.OnTick(this, parDt);
    if (_currentState.AllowsWorldTick)
    {
      _updateService.Update(GameState, parDt, GameState.CurrentLevel, EventPublisher);
    }
  }

  /// <inheritdoc />
  public void HandleCommand(IGameCommand parCommand)
  {
    _currentState.HandleCommand(this, parCommand);
  }

  /// <inheritdoc />
  public bool ProcessResumeCountdown(double parDt)
  {
    return _resumeCountdownService.Process(GameState, parDt, EventPublisher);
  }

  /// <inheritdoc />
  public void TransitionTo(IGameFlowState parNextState)
  {
    _currentState.OnExit(this);
    _currentState = parNextState;
    _currentState.OnEnter(this);
  }
}
