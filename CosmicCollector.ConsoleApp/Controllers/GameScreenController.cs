using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;
using CosmicCollector.Core.Events;
using CosmicCollector.Core.Snapshots;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Loop;
using System;
using System.Diagnostics;
using System.Threading;

namespace CosmicCollector.ConsoleApp.Controllers;

/// <summary>
/// Контроллер игрового экрана (только отображение).
/// </summary>
public sealed class GameScreenController
{
  private readonly IGameScreenView _view;
  private readonly IEventBus _eventBus;
  private readonly IGameSnapshotProvider _snapshotProvider;
  private readonly IGameLoopRunner _gameLoopRunner;
  private readonly CommandQueue _commandQueue;
  private readonly IKeyStateProvider _keyStateProvider;
  private readonly AutoResetEvent _tickSignal = new(false);
  private readonly object _snapshotLock = new();
  private IDisposable? _gameStartedSubscription;
  private IDisposable? _gameTickSubscription;
  private IDisposable? _pauseSubscription;
  private IDisposable? _countdownSubscription;
  private IDisposable? _gameOverSubscription;
  private IDisposable? _levelCompletedSubscription;
  private Thread? _inputThread;
  private int _level;
  private bool _isPaused;
  private int _countdownValue;
  private bool _isRunning;
  private bool _leftHeld;
  private bool _rightHeld;
  private bool _pauseHeld;
  private int _moveDirection;
  private GameSnapshot? _latestSnapshot;

  /// <summary>
  /// Создаёт контроллер игрового экрана.
  /// </summary>
  /// <param name="parView">Представление игрового экрана.</param>
  /// <param name="parEventBus">Шина событий.</param>
  /// <param name="parSnapshotProvider">Поставщик снимков.</param>
  /// <param name="parGameLoopRunner">Игровой цикл.</param>
  /// <param name="parCommandQueue">Очередь команд.</param>
  /// <param name="parKeyStateProvider">Поставщик состояния клавиш.</param>
  /// <param name="parLevel">Стартовый уровень.</param>
  public GameScreenController(
    IGameScreenView parView,
    IEventBus parEventBus,
    IGameSnapshotProvider parSnapshotProvider,
    IGameLoopRunner parGameLoopRunner,
    CommandQueue parCommandQueue,
    IKeyStateProvider parKeyStateProvider,
    int parLevel)
  {
    _view = parView;
    _eventBus = parEventBus;
    _snapshotProvider = parSnapshotProvider;
    _gameLoopRunner = parGameLoopRunner;
    _commandQueue = parCommandQueue;
    _keyStateProvider = parKeyStateProvider;
    _level = parLevel;
  }

  /// <summary>
  /// Запускает игровой экран в режиме отображения.
  /// </summary>
  public void Run()
  {
    _gameStartedSubscription = _eventBus.Subscribe<GameStarted>(OnGameStarted);
    _gameTickSubscription = _eventBus.Subscribe<GameTick>(OnGameTick);
    _pauseSubscription = _eventBus.Subscribe<PauseToggled>(OnPauseToggled);
    _countdownSubscription = _eventBus.Subscribe<CountdownTick>(OnCountdownTick);
    _gameOverSubscription = _eventBus.Subscribe<GameOver>(OnGameOver);
    _levelCompletedSubscription = _eventBus.Subscribe<LevelCompleted>(OnLevelCompleted);

    _eventBus.Publish(new GameStarted(_level));
    _gameLoopRunner.Start();
    StartInputLoop();

    while (_isRunning)
    {
      _tickSignal.WaitOne();
      RenderLatestSnapshot();
    }
  }

  private void OnGameStarted(GameStarted parEvent)
  {
    _level = parEvent.parLevel;
  }

  private void OnGameTick(GameTick parEvent)
  {
    var snapshot = _snapshotProvider.GetSnapshot();

    lock (_snapshotLock)
    {
      _latestSnapshot = snapshot;
    }

    _tickSignal.Set();
  }

  private void OnPauseToggled(PauseToggled parEvent)
  {
    _isPaused = parEvent.parIsPaused;

    if (parEvent.parIsPaused)
    {
      SetMoveDirection(0);
    }
  }

  private void OnCountdownTick(CountdownTick parEvent)
  {
    _countdownValue = parEvent.parValue;
  }

  private void OnGameOver(GameOver parEvent)
  {
    _isRunning = false;
    _tickSignal.Set();
  }

  private void OnLevelCompleted(LevelCompleted parEvent)
  {
    _isRunning = false;
    _tickSignal.Set();
  }

  private void RenderLatestSnapshot()
  {
    GameSnapshot? snapshot;

    lock (_snapshotLock)
    {
      snapshot = _latestSnapshot;
    }

    if (snapshot is null)
    {
      return;
    }

    _view.Render(snapshot, _level, _isPaused, _countdownValue);

    if (!_isPaused && _countdownValue > 0)
    {
      _countdownValue = 0;
    }
  }

  /// <summary>
  /// Обрабатывает состояние ввода для игровых команд.
  /// </summary>
  /// <param name="parLeftHeld">Признак удержания влево.</param>
  /// <param name="parRightHeld">Признак удержания вправо.</param>
  /// <param name="parPauseHeld">Признак удержания паузы.</param>
  public void ApplyInputState(bool parLeftHeld, bool parRightHeld, bool parPauseHeld)
  {
    if (parPauseHeld && !_pauseHeld)
    {
      _commandQueue.Enqueue(new TogglePauseCommand());
    }

    int direction = 0;

    if (parLeftHeld ^ parRightHeld)
    {
      direction = parLeftHeld ? -1 : 1;
    }

    if (!CanMove())
    {
      direction = 0;
    }

    SetMoveDirection(direction);

    _leftHeld = parLeftHeld;
    _rightHeld = parRightHeld;
    _pauseHeld = parPauseHeld;
  }

  /// <summary>
  /// Обновляет значение отсчёта для тестов и событий.
  /// </summary>
  /// <param name="parValue">Текущее значение отсчёта.</param>
  public void UpdateCountdown(int parValue)
  {
    _countdownValue = parValue;
  }

  /// <summary>
  /// Обновляет состояние паузы для тестов и событий.
  /// </summary>
  /// <param name="parIsPaused">Признак паузы.</param>
  public void UpdatePauseState(bool parIsPaused)
  {
    _isPaused = parIsPaused;

    if (parIsPaused)
    {
      SetMoveDirection(0);
    }
  }

  /// <summary>
  /// Принудительно отрисовывает кадр для тестов.
  /// </summary>
  /// <param name="parSnapshot">Снимок состояния игры.</param>
  public void RenderSnapshot(GameSnapshot parSnapshot)
  {
    _view.Render(parSnapshot, _level, _isPaused, _countdownValue);
  }

  private void StartInputLoop()
  {
    _isRunning = true;
    _inputThread = new Thread(ReadInputLoop)
    {
      IsBackground = true,
      Name = "ConsoleInput"
    };
    _inputThread.Start();
  }

  private void ReadInputLoop()
  {
    const int pollIntervalMs = 5;
    var stopwatch = Stopwatch.StartNew();
    long lastPoll = 0;

    while (_isRunning)
    {
      long elapsed = stopwatch.ElapsedMilliseconds;
      if (elapsed - lastPoll < pollIntervalMs)
      {
        Thread.SpinWait(50);
        continue;
      }

      lastPoll = elapsed;

      bool leftHeld = _keyStateProvider.IsKeyDown(ConsoleKey.A) || _keyStateProvider.IsKeyDown(ConsoleKey.LeftArrow);
      bool rightHeld = _keyStateProvider.IsKeyDown(ConsoleKey.D) || _keyStateProvider.IsKeyDown(ConsoleKey.RightArrow);
      bool pauseHeld = _keyStateProvider.IsKeyDown(ConsoleKey.P) || _keyStateProvider.IsKeyDown(ConsoleKey.Spacebar);

      ApplyInputState(leftHeld, rightHeld, pauseHeld);
    }
  }

  private bool CanMove()
  {
    return !_isPaused && _countdownValue <= 0;
  }

  private void SetMoveDirection(int parDirection)
  {
    int clamped = parDirection switch
    {
      < 0 => -1,
      > 0 => 1,
      _ => 0
    };

    if (clamped == _moveDirection)
    {
      return;
    }

    _moveDirection = clamped;
    _commandQueue.Enqueue(new SetMoveDirectionCommand(clamped));
  }
}
