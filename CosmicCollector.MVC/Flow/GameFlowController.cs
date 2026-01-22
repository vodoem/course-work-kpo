using System;
using System.Collections.Generic;
using CosmicCollector.Core.Events;
using CosmicCollector.Core.Model;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Services;

namespace CosmicCollector.MVC.Flow;

/// <summary>
/// Управляет переходами между состояниями игрового процесса.
/// </summary>
public sealed class GameFlowController : IGameFlowController, IGameFlowContext, IDisposable
{
  private readonly object _stateLock = new();
  private readonly List<IDisposable> _subscriptions = new();
  private IGameFlowState _currentState;

  /// <summary>
  /// Инициализирует контроллер игрового процесса.
  /// </summary>
  /// <param name="parGameState">Состояние игры.</param>
  /// <param name="parEventBus">Шина событий.</param>
  /// <param name="parSaveService">Сервис сохранения.</param>
  /// <param name="parInitialState">Начальное состояние.</param>
  public GameFlowController(
    GameState parGameState,
    IEventBus parEventBus,
    IGameSaveService parSaveService,
    IGameFlowState parInitialState)
  {
    GameState = parGameState ?? throw new ArgumentNullException(nameof(parGameState));
    EventBus = parEventBus ?? throw new ArgumentNullException(nameof(parEventBus));
    SaveService = parSaveService ?? throw new ArgumentNullException(nameof(parSaveService));
    _currentState = parInitialState ?? throw new ArgumentNullException(nameof(parInitialState));

    _currentState.OnEnter(this);
    SubscribeToEvents();
  }

  /// <inheritdoc />
  public IGameFlowState CurrentState
  {
    get
    {
      lock (_stateLock)
      {
        return _currentState;
      }
    }
  }

  /// <inheritdoc />
  public GameState GameState { get; }

  /// <inheritdoc />
  public IEventBus EventBus { get; }

  /// <inheritdoc />
  public IGameSaveService SaveService { get; }

  /// <inheritdoc />
  public bool AllowsWorldTick
  {
    get
    {
      lock (_stateLock)
      {
        return _currentState.AllowsWorldTick;
      }
    }
  }

  /// <inheritdoc />
  public void HandleCommand(IGameCommand parCommand)
  {
    if (parCommand is null)
    {
      return;
    }

    IGameFlowState state;
    lock (_stateLock)
    {
      state = _currentState;
    }

    state.HandleCommand(this, parCommand);
  }

  /// <inheritdoc />
  public void TransitionTo(IGameFlowState parNextState)
  {
    if (parNextState is null)
    {
      return;
    }

    IGameFlowState previousState;
    lock (_stateLock)
    {
      previousState = _currentState;
      _currentState = parNextState;
    }

    previousState.OnExit(this);
    parNextState.OnEnter(this);
  }

  /// <inheritdoc />
  public void Dispose()
  {
    foreach (var subscription in _subscriptions)
    {
      subscription.Dispose();
    }

    _subscriptions.Clear();
  }

  private void SubscribeToEvents()
  {
    _subscriptions.Add(EventBus.Subscribe<GameOver>(_ => TransitionTo(new GameOverState())));
    _subscriptions.Add(EventBus.Subscribe<LevelCompleted>(_ => TransitionTo(new LevelCompletedState())));
    _subscriptions.Add(EventBus.Subscribe<PauseToggled>(OnPauseToggled));
  }

  private void OnPauseToggled(PauseToggled parEvent)
  {
    if (!parEvent.parIsPaused && CurrentState is ResumeCountdownState)
    {
      TransitionTo(new PlayingState());
    }
  }
}
