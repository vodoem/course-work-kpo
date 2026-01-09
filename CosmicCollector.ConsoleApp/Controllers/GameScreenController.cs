using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;
using CosmicCollector.Core.Events;
using CosmicCollector.Core.Snapshots;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Loop;
using CosmicCollector.Persistence.Records;
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
  private readonly IConsoleRenderer _renderer;
  private readonly IConsoleInputReader _inputReader;
  private readonly IRecordsRepository _recordsRepository;
  private readonly AutoResetEvent _tickSignal = new(false);
  private readonly object _snapshotLock = new();
  private IDisposable? _gameStartedSubscription;
  private IDisposable? _gameTickSubscription;
  private IDisposable? _pauseSubscription;
  private IDisposable? _countdownSubscription;
  private IDisposable? _gameOverSubscription;
  private IDisposable? _levelCompletedSubscription;
  private Thread? _inputThread;
  private CancellationTokenSource? _inputCancellation;
  private int _level;
  private bool _isPaused;
  private int _countdownValue;
  private bool _isRunning;
  private bool _leftHeld;
  private bool _rightHeld;
  private bool _pauseHeld;
  private int _moveDirection;
  private bool _shouldExitGameScreen;
  private bool _isInputEnabled = true;
  private GameSnapshot? _finalSnapshot;
  private GameEndReason? _endReason;
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
  /// <param name="parRenderer">Рендерер консоли.</param>
  /// <param name="parInputReader">Читатель ввода.</param>
  /// <param name="parLevel">Стартовый уровень.</param>
  public GameScreenController(
    IGameScreenView parView,
    IEventBus parEventBus,
    IGameSnapshotProvider parSnapshotProvider,
    IGameLoopRunner parGameLoopRunner,
    CommandQueue parCommandQueue,
    IKeyStateProvider parKeyStateProvider,
    IConsoleRenderer parRenderer,
    IConsoleInputReader parInputReader,
    IRecordsRepository parRecordsRepository,
    int parLevel)
  {
    _view = parView;
    _eventBus = parEventBus;
    _snapshotProvider = parSnapshotProvider;
    _gameLoopRunner = parGameLoopRunner;
    _commandQueue = parCommandQueue;
    _keyStateProvider = parKeyStateProvider;
    _renderer = parRenderer;
    _inputReader = parInputReader;
    _recordsRepository = parRecordsRepository;
    _level = parLevel;
  }

  /// <summary>
  /// Запускает игровой экран в режиме отображения.
  /// </summary>
  public GameEndAction Run()
  {
    _shouldExitGameScreen = false;
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
      if (_shouldExitGameScreen)
      {
        _isRunning = false;
        break;
      }

      RenderLatestSnapshot();
    }

    _gameLoopRunner.Stop();
    StopInputLoop();

    if (_finalSnapshot is null || _endReason is null)
    {
      return GameEndAction.ReturnToMenu;
    }

    GameEndView view = new GameEndView(_renderer);
    GameEndController controller = new GameEndController(view, _inputReader, _recordsRepository);
    return controller.Run(_endReason.Value, _finalSnapshot, _level);
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
    HandleGameEnd(GameEndReason.GameOver);
  }

  private void OnLevelCompleted(LevelCompleted parEvent)
  {
    HandleGameEnd(GameEndReason.LevelCompleted);
  }

  private void HandleGameEnd(GameEndReason parReason)
  {
    _finalSnapshot = _snapshotProvider.GetSnapshot();
    _endReason = parReason;
    _isInputEnabled = false;
    _shouldExitGameScreen = true;
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
    if (!_isInputEnabled)
    {
      return;
    }

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
    if (!_isRunning)
    {
      return;
    }

    _view.Render(parSnapshot, _level, _isPaused, _countdownValue);
  }

  private void StartInputLoop()
  {
    _isRunning = true;
    _inputCancellation = new CancellationTokenSource();
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

    while (_isRunning &&
           !_shouldExitGameScreen &&
           _inputCancellation is not null &&
           !_inputCancellation.IsCancellationRequested)
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

  private void StopInputLoop()
  {
    if (_inputCancellation is null)
    {
      return;
    }

    _inputCancellation.Cancel();
    _inputThread?.Join();
  }

  /// <summary>
  /// Устанавливает состояние запуска для тестов без потоков.
  /// </summary>
  public void StartForTests()
  {
    _isRunning = true;
    _isInputEnabled = true;
  }

  /// <summary>
  /// Инициализирует подписки на события для тестов без запуска цикла.
  /// </summary>
  public void InitializeForTests()
  {
    _isRunning = true;
    _isInputEnabled = true;
    _shouldExitGameScreen = false;
    _gameOverSubscription = _eventBus.Subscribe<GameOver>(OnGameOver);
    _levelCompletedSubscription = _eventBus.Subscribe<LevelCompleted>(OnLevelCompleted);
  }

  /// <summary>
  /// Завершает игру для тестов и фиксирует финальный снимок.
  /// </summary>
  /// <param name="parReason">Причина завершения.</param>
  /// <param name="parSnapshot">Финальный снимок.</param>
  public void HandleGameEndForTests(GameEndReason parReason, GameSnapshot parSnapshot)
  {
    _finalSnapshot = parSnapshot;
    _endReason = parReason;
    _isInputEnabled = false;
    _shouldExitGameScreen = true;
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

  /// <summary>
  /// Возвращает признак работы контроллера.
  /// </summary>
  public bool IsRunning => _isRunning;

  /// <summary>
  /// Возвращает признак активного ввода.
  /// </summary>
  public bool IsInputEnabled => _isInputEnabled;

  /// <summary>
  /// Возвращает финальный снимок.
  /// </summary>
  public GameSnapshot? FinalSnapshot => _finalSnapshot;

  /// <summary>
  /// Возвращает причину завершения.
  /// </summary>
  public GameEndReason? EndReason => _endReason;

  /// <summary>
  /// Возвращает признак выхода с игрового экрана.
  /// </summary>
  public bool ShouldExitGameScreen => _shouldExitGameScreen;
}
