using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Threading;
using CosmicCollector.Avalonia.Commands;
using CosmicCollector.Avalonia.Infrastructure;
using CosmicCollector.Avalonia.Navigation;
using CosmicCollector.Avalonia.Rendering;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Events;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Snapshots;
using CosmicCollector.MVC.Commands;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// ViewModel игрового экрана.
/// </summary>
public sealed class GameViewModel : ViewModelBase
{
  private readonly GameRuntime _gameRuntime;
  private readonly NavigationService _mainMenuNavigation;
  private readonly NavigationService _gameOverNavigation;
  private readonly List<IDisposable> _subscriptions = new();
  private FrameSnapshot? _latestSnapshot;
  private bool _isActive;
  private int _currentLevel;
  private int _requiredScore;
  private int _score;
  private int _energy;
  private int _requiredBlue;
  private int _requiredGreen;
  private int _requiredRed;
  private int _collectedBlue;
  private int _collectedGreen;
  private int _collectedRed;
  private string _blueProgressText = "B: 0/0";
  private string _greenProgressText = "G: 0/0";
  private string _redProgressText = "R: 0/0";
  private string _timerText = "—";
  private bool _isPaused;
  private bool _isConfirmSaveVisible;
  private int _countdownValue;
  private string _hudTargetScoreText = "Цель очков: 0";
  private string _hudTimerValueText = "—";
  private string _hudStatusText = "Ур. 0 | Энергия: 0 | Очки: 0";
  private string _hudGoalsText = "Цели: B=0 G=0 R=0";
  private string _hudProgressText = "Прогресс: B=0/0 G=0/0 R=0/0";
  private double _fieldWidth;
  private double _fieldHeight;
  private bool _leftPressed;
  private bool _rightPressed;
  private WorldBounds _worldBounds;

  /// <summary>
  /// Инициализирует новый экземпляр <see cref="GameViewModel"/>.
  /// </summary>
  /// <param name="parGameRuntime">Игровой runtime.</param>
  /// <param name="parBackNavigation">Навигация назад в главное меню.</param>
  /// <param name="parGameOverNavigation">Навигация на экран завершения.</param>
  public GameViewModel(
    GameRuntime parGameRuntime,
    NavigationService parBackNavigation,
    NavigationService parGameOverNavigation)
  {
    _gameRuntime = parGameRuntime;
    _mainMenuNavigation = parBackNavigation;
    _gameOverNavigation = parGameOverNavigation;
    BackToMenuCommand = new DelegateCommand(HandleBackToMenu);
    ResumeCommand = new DelegateCommand(HandleResume);
    ExitToMenuCommand = new DelegateCommand(HandleBackToMenu);
    ConfirmSaveAndExitCommand = new DelegateCommand(HandleConfirmSaveAndExit);
    ConfirmExitWithoutSaveCommand = new DelegateCommand(HandleConfirmExitWithoutSave);
    CancelExitCommand = new DelegateCommand(HandleCancelExit);
  }

  /// <summary>
  /// Команда возврата в главное меню.
  /// </summary>
  public ICommand BackToMenuCommand { get; }

  /// <summary>
  /// Команда продолжения игры.
  /// </summary>
  public ICommand ResumeCommand { get; }

  /// <summary>
  /// Команда выхода в главное меню.
  /// </summary>
  public ICommand ExitToMenuCommand { get; }

  /// <summary>
  /// Команда сохранения и выхода в меню.
  /// </summary>
  public ICommand ConfirmSaveAndExitCommand { get; }

  /// <summary>
  /// Команда выхода в меню без сохранения.
  /// </summary>
  public ICommand ConfirmExitWithoutSaveCommand { get; }

  /// <summary>
  /// Команда отмены выхода.
  /// </summary>
  public ICommand CancelExitCommand { get; }

  /// <summary>
  /// Последний снимок рендера.
  /// </summary>
  public FrameSnapshot? LatestSnapshot
  {
    get => _latestSnapshot;
    private set
    {
      _latestSnapshot = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Ширина игрового поля.
  /// </summary>
  public double FieldWidth
  {
    get => _fieldWidth;
    private set
    {
      _fieldWidth = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Высота игрового поля.
  /// </summary>
  public double FieldHeight
  {
    get => _fieldHeight;
    private set
    {
      _fieldHeight = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Текущий уровень.
  /// </summary>
  public int CurrentLevel
  {
    get => _currentLevel;
    private set
    {
      _currentLevel = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Требуемые очки уровня.
  /// </summary>
  public int RequiredScore
  {
    get => _requiredScore;
    private set
    {
      _requiredScore = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Текущие очки.
  /// </summary>
  public int Score
  {
    get => _score;
    private set
    {
      _score = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Энергия дрона.
  /// </summary>
  public int Energy
  {
    get => _energy;
    private set
    {
      _energy = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Прогресс по синим кристаллам.
  /// </summary>
  public string BlueProgressText
  {
    get => _blueProgressText;
    private set
    {
      _blueProgressText = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Прогресс по зелёным кристаллам.
  /// </summary>
  public string GreenProgressText
  {
    get => _greenProgressText;
    private set
    {
      _greenProgressText = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Прогресс по красным кристаллам.
  /// </summary>
  public string RedProgressText
  {
    get => _redProgressText;
    private set
    {
      _redProgressText = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Текстовое представление таймера уровня.
  /// </summary>
  public string TimerText
  {
    get => _timerText;
    private set
    {
      _timerText = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Признак паузы.
  /// </summary>
  public bool IsPaused
  {
    get => _isPaused;
    private set
    {
      _isPaused = value;
      OnPropertyChanged();
      OnPropertyChanged(nameof(IsPauseOverlayVisible));
      OnPropertyChanged(nameof(IsCountdownVisible));
    }
  }

  /// <summary>
  /// Признак отображения подтверждения сохранения.
  /// </summary>
  public bool IsConfirmSaveVisible
  {
    get => _isConfirmSaveVisible;
    private set
    {
      _isConfirmSaveVisible = value;
      OnPropertyChanged();
      OnPropertyChanged(nameof(IsPauseOverlayVisible));
      OnPropertyChanged(nameof(IsCountdownVisible));
    }
  }

  /// <summary>
  /// Значение обратного отсчёта.
  /// </summary>
  public int CountdownValue
  {
    get => _countdownValue;
    private set
    {
      _countdownValue = value;
      OnPropertyChanged();
      OnPropertyChanged(nameof(IsPauseOverlayVisible));
      OnPropertyChanged(nameof(IsCountdownVisible));
    }
  }

  /// <summary>
  /// Признак видимости оверлея паузы.
  /// </summary>
  public bool IsPauseOverlayVisible => IsPaused && CountdownValue <= 0 && !IsConfirmSaveVisible;

  /// <summary>
  /// Признак видимости оверлея отсчёта.
  /// </summary>
  public bool IsCountdownVisible => IsPaused && CountdownValue > 0 && !IsConfirmSaveVisible;

  /// <summary>
  /// Текст цели очков для HUD.
  /// </summary>
  public string HudTargetScoreText
  {
    get => _hudTargetScoreText;
    private set
    {
      _hudTargetScoreText = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Текст таймера для HUD.
  /// </summary>
  public string HudTimerValueText
  {
    get => _hudTimerValueText;
    private set
    {
      _hudTimerValueText = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Текст статуса для HUD.
  /// </summary>
  public string HudStatusText
  {
    get => _hudStatusText;
    private set
    {
      _hudStatusText = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Текст целей для HUD.
  /// </summary>
  public string HudGoalsText
  {
    get => _hudGoalsText;
    private set
    {
      _hudGoalsText = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Текст прогресса для HUD.
  /// </summary>
  public string HudProgressText
  {
    get => _hudProgressText;
    private set
    {
      _hudProgressText = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Требуемое количество синих кристаллов.
  /// </summary>
  public int RequiredBlue
  {
    get => _requiredBlue;
    private set
    {
      _requiredBlue = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Требуемое количество зелёных кристаллов.
  /// </summary>
  public int RequiredGreen
  {
    get => _requiredGreen;
    private set
    {
      _requiredGreen = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Требуемое количество красных кристаллов.
  /// </summary>
  public int RequiredRed
  {
    get => _requiredRed;
    private set
    {
      _requiredRed = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Собранные синие кристаллы.
  /// </summary>
  public int CollectedBlue
  {
    get => _collectedBlue;
    private set
    {
      _collectedBlue = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Собранные зелёные кристаллы.
  /// </summary>
  public int CollectedGreen
  {
    get => _collectedGreen;
    private set
    {
      _collectedGreen = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Собранные красные кристаллы.
  /// </summary>
  public int CollectedRed
  {
    get => _collectedRed;
    private set
    {
      _collectedRed = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  /// Активирует игровую сессию.
  /// </summary>
  public void Activate()
  {
    if (_isActive)
    {
      return;
    }

    _isActive = true;
    _gameRuntime.Start();
    InitializeFieldBounds();
    UpdateFromSnapshot(_gameRuntime.GetSnapshot());
    SubscribeToEvents();
  }

  /// <summary>
  /// Останавливает игровую сессию.
  /// </summary>
  public void Deactivate()
  {
    if (!_isActive)
    {
      return;
    }

    _isActive = false;
    ResetMoveState();
    IsConfirmSaveVisible = false;
    UnsubscribeFromEvents();
    _gameRuntime.Stop();
  }

  /// <summary>
  /// Обрабатывает нажатия клавиш.
  /// </summary>
  /// <param name="parKey">Нажатая клавиша.</param>
  public void HandleKeyDown(Key parKey)
  {
    if (!_isActive)
    {
      return;
    }

    switch (parKey)
    {
      case Key.A:
      case Key.Left:
        _leftPressed = true;
        EnqueueMove(ComputeMoveDirection());
        break;
      case Key.D:
      case Key.Right:
        _rightPressed = true;
        EnqueueMove(ComputeMoveDirection());
        break;
      case Key.P:
      case Key.Escape:
        _gameRuntime.CommandQueue.Enqueue(new TogglePauseCommand());
        break;
    }
  }

  /// <summary>
  /// Обрабатывает отпускание клавиш.
  /// </summary>
  /// <param name="parKey">Отпущенная клавиша.</param>
  public void HandleKeyUp(Key parKey)
  {
    if (!_isActive)
    {
      return;
    }

    switch (parKey)
    {
      case Key.A:
      case Key.Left:
        _leftPressed = false;
        EnqueueMove(ComputeMoveDirection());
        break;
      case Key.D:
      case Key.Right:
        _rightPressed = false;
        EnqueueMove(ComputeMoveDirection());
        break;
    }
  }

  private void InitializeFieldBounds()
  {
    var bounds = _gameRuntime.GameState.WorldBounds;
    _worldBounds = bounds;
    FieldWidth = bounds.Right - bounds.Left;
    FieldHeight = bounds.Bottom - bounds.Top;
  }

  private void SubscribeToEvents()
  {
    _subscriptions.Add(_gameRuntime.EventBus.Subscribe<GameTick>(OnGameTick));
    _subscriptions.Add(_gameRuntime.EventBus.Subscribe<GameOver>(OnGameOver));
    _subscriptions.Add(_gameRuntime.EventBus.Subscribe<LevelCompleted>(OnLevelCompleted));
    _subscriptions.Add(_gameRuntime.EventBus.Subscribe<PauseToggled>(OnPauseToggled));
    _subscriptions.Add(_gameRuntime.EventBus.Subscribe<CountdownTick>(OnCountdownTick));
    _subscriptions.Add(_gameRuntime.EventBus.Subscribe<ConfirmSaveBeforeMenuRequested>(OnConfirmSaveBeforeMenuRequested));
    _subscriptions.Add(_gameRuntime.EventBus.Subscribe<ConfirmSaveBeforeMenuClosed>(OnConfirmSaveBeforeMenuClosed));
    _subscriptions.Add(_gameRuntime.EventBus.Subscribe<MenuNavigationRequested>(OnMenuNavigationRequested));
  }

  private void UnsubscribeFromEvents()
  {
    foreach (var subscription in _subscriptions)
    {
      subscription.Dispose();
    }

    _subscriptions.Clear();
  }

  private void OnGameTick(GameTick parEvent)
  {
    if (!_isActive)
    {
      return;
    }

    var snapshot = _gameRuntime.GetSnapshot();
    Dispatcher.UIThread.Post(() => UpdateFromSnapshot(snapshot));
  }

  private void OnGameOver(GameOver parEvent)
  {
    var snapshot = _gameRuntime.GetSnapshot();
    Dispatcher.UIThread.Post(() =>
    {
      if (!_isActive)
      {
        return;
      }

      _isActive = false;
      ResetMoveState();
      UnsubscribeFromEvents();
      _gameOverNavigation.Navigate();
      if (_gameOverNavigation.CurrentViewModel is GameOverViewModel gameOverViewModel)
      {
        gameOverViewModel.SetFinalStats(snapshot);
      }

      Task.Run(_gameRuntime.Stop);
    });
  }

  private void OnLevelCompleted(LevelCompleted parEvent)
  {
    var snapshot = _gameRuntime.GetSnapshot();
    Dispatcher.UIThread.Post(() => UpdateFromSnapshot(snapshot));
  }

  private void OnPauseToggled(PauseToggled parEvent)
  {
    Dispatcher.UIThread.Post(() =>
    {
      IsPaused = parEvent.parIsPaused;
      if (!parEvent.parIsPaused)
      {
        CountdownValue = 0;
        IsConfirmSaveVisible = false;
        return;
      }

      ResetMoveState();
    });
  }

  private void OnCountdownTick(CountdownTick parEvent)
  {
    Dispatcher.UIThread.Post(() => CountdownValue = parEvent.parValue);
  }

  private void OnConfirmSaveBeforeMenuRequested(ConfirmSaveBeforeMenuRequested parEvent)
  {
    Dispatcher.UIThread.Post(() =>
    {
      if (!_isActive)
      {
        return;
      }

      IsConfirmSaveVisible = true;
    });
  }

  private void OnConfirmSaveBeforeMenuClosed(ConfirmSaveBeforeMenuClosed parEvent)
  {
    Dispatcher.UIThread.Post(() =>
    {
      if (!_isActive)
      {
        return;
      }

      IsConfirmSaveVisible = false;
    });
  }

  private void OnMenuNavigationRequested(MenuNavigationRequested parEvent)
  {
    Dispatcher.UIThread.Post(() =>
    {
      if (!_isActive)
      {
        return;
      }

      IsConfirmSaveVisible = false;
      Deactivate();
      _mainMenuNavigation.Navigate();
    });
  }

  private void UpdateFromSnapshot(GameSnapshot parSnapshot)
  {
    if (!_isActive)
    {
      return;
    }

    CurrentLevel = parSnapshot.parCurrentLevel;
    RequiredScore = parSnapshot.parRequiredScore;
    Score = parSnapshot.parDrone.parScore;
    Energy = parSnapshot.parDrone.parEnergy;
    RequiredBlue = parSnapshot.parLevelGoals.parRequiredBlue;
    RequiredGreen = parSnapshot.parLevelGoals.parRequiredGreen;
    RequiredRed = parSnapshot.parLevelGoals.parRequiredRed;
    CollectedBlue = parSnapshot.parLevelProgress.parCollectedBlue;
    CollectedGreen = parSnapshot.parLevelProgress.parCollectedGreen;
    CollectedRed = parSnapshot.parLevelProgress.parCollectedRed;
    BlueProgressText = $"B: {parSnapshot.parLevelProgress.parCollectedBlue}/{parSnapshot.parLevelGoals.parRequiredBlue}";
    GreenProgressText = $"G: {parSnapshot.parLevelProgress.parCollectedGreen}/{parSnapshot.parLevelGoals.parRequiredGreen}";
    RedProgressText = $"R: {parSnapshot.parLevelProgress.parCollectedRed}/{parSnapshot.parLevelGoals.parRequiredRed}";
    TimerText = FormatTimer(parSnapshot.parHasLevelTimer, parSnapshot.parLevelTimeRemainingSec);
    HudTargetScoreText = $"Цель очков: {parSnapshot.parRequiredScore}";
    HudTimerValueText = FormatTimer(parSnapshot.parHasLevelTimer, parSnapshot.parLevelTimeRemainingSec);
    HudStatusText = $"Ур. {parSnapshot.parCurrentLevel} | Энергия: {parSnapshot.parDrone.parEnergy} | Очки: {parSnapshot.parDrone.parScore}";
    HudGoalsText = $"Цели: B={parSnapshot.parLevelGoals.parRequiredBlue} G={parSnapshot.parLevelGoals.parRequiredGreen} R={parSnapshot.parLevelGoals.parRequiredRed}";
    HudProgressText = $"Прогресс: B={parSnapshot.parLevelProgress.parCollectedBlue}/{parSnapshot.parLevelGoals.parRequiredBlue} " +
                      $"G={parSnapshot.parLevelProgress.parCollectedGreen}/{parSnapshot.parLevelGoals.parRequiredGreen} " +
                      $"R={parSnapshot.parLevelProgress.parCollectedRed}/{parSnapshot.parLevelGoals.parRequiredRed}";
    PublishSnapshot(parSnapshot);
  }

  private IReadOnlyList<RenderItem> BuildRenderItems(GameSnapshot parSnapshot)
  {
    var items = new List<RenderItem>();
    var order = 0;

    AppendItem(items, parSnapshot.parDrone.parPosition, parSnapshot.parDrone.parBounds, "drone", RenderLayer.Drone, order++);

    foreach (var crystal in parSnapshot.parCrystals)
    {
      AppendItem(items, crystal.parPosition, crystal.parBounds, GetCrystalSpriteKey(crystal.parType), RenderLayer.Crystal, order++);
    }

    foreach (var asteroid in parSnapshot.parAsteroids)
    {
      AppendItem(items, asteroid.parPosition, asteroid.parBounds, "asteroid", RenderLayer.Asteroid, order++);
    }

    foreach (var bonus in parSnapshot.parBonuses)
    {
      AppendItem(items, bonus.parPosition, bonus.parBounds, GetBonusSpriteKey(bonus.parType), RenderLayer.Bonus, order++);
    }

    foreach (var blackHole in parSnapshot.parBlackHoles)
    {
      if (blackHole.parRadius > 0)
      {
        var fieldBounds = new Aabb(blackHole.parRadius * 2, blackHole.parRadius * 2);
        AppendItem(items, blackHole.parPosition, fieldBounds, "black_hole_field", RenderLayer.BlackHoleField, order++);
      }

      AppendItem(items, blackHole.parPosition, blackHole.parBounds, "blackhole", RenderLayer.BlackHole, order++);
    }

    items.Sort((left, right) =>
    {
      var layerComparison = left.Layer.CompareTo(right.Layer);
      return layerComparison != 0 ? layerComparison : left.Order.CompareTo(right.Order);
    });

    return items;
  }

  private void AppendItem(
    ICollection<RenderItem> parItems,
    Vector2 parPosition,
    Aabb parBounds,
    string parSpriteKey,
    RenderLayer parLayer,
    int parOrder)
  {
    var width = parBounds.Width;
    var height = parBounds.Height;
    var left = parPosition.X - (parBounds.Width / 2.0);
    var top = parPosition.Y - (parBounds.Height / 2.0);

    parItems.Add(new RenderItem(left, top, width, height, parSpriteKey, (int)parLayer, parOrder));
  }

  private static string GetCrystalSpriteKey(CrystalType parType)
  {
    return parType switch
    {
      CrystalType.Blue => "crystal_blue",
      CrystalType.Green => "crystal_green",
      CrystalType.Red => "crystal_red",
      _ => "crystal_blue"
    };
  }

  private static string GetBonusSpriteKey(BonusType parType)
  {
    return parType switch
    {
      BonusType.Magnet => "bonus_magnet",
      BonusType.Accelerator => "bonus_accelerator",
      BonusType.TimeStabilizer => "bonus_time",
      _ => "bonus_time"
    };
  }

  private static string FormatTimer(bool parHasTimer, double parSeconds)
  {
    if (!parHasTimer)
    {
      return "—";
    }

    var time = TimeSpan.FromSeconds(Math.Max(0, parSeconds));
    return time.ToString("mm\\:ss", CultureInfo.InvariantCulture);
  }

  private void EnqueueMove(int parDirection)
  {
    _gameRuntime.CommandQueue.Enqueue(new SetMoveDirectionCommand(parDirection));
  }

  private int ComputeMoveDirection()
  {
    if (_leftPressed == _rightPressed)
    {
      return 0;
    }

    return _leftPressed ? -1 : 1;
  }

  private void ResetMoveState()
  {
    _leftPressed = false;
    _rightPressed = false;
    EnqueueMove(0);
  }

  private void PublishSnapshot(GameSnapshot parSnapshot)
  {
    var items = BuildRenderItems(parSnapshot);
    var timestamp = System.Diagnostics.Stopwatch.GetTimestamp();
    LatestSnapshot = new FrameSnapshot(timestamp, items, _worldBounds);
  }

  private void HandleResume()
  {
    if (IsPaused && CountdownValue <= 0)
    {
      _gameRuntime.CommandQueue.Enqueue(new TogglePauseCommand());
    }
  }

  private void HandleBackToMenu()
  {
    if (!_isActive)
    {
      return;
    }

    _gameRuntime.CommandQueue.Enqueue(new RequestBackToMenuCommand());
  }

  private void HandleConfirmSaveAndExit()
  {
    if (!_isActive)
    {
      return;
    }

    _gameRuntime.CommandQueue.Enqueue(new ConfirmSaveAndExitCommand());
  }

  private void HandleConfirmExitWithoutSave()
  {
    if (!_isActive)
    {
      return;
    }

    _gameRuntime.CommandQueue.Enqueue(new ConfirmExitWithoutSaveCommand());
  }

  private void HandleCancelExit()
  {
    if (!_isActive)
    {
      return;
    }

    _gameRuntime.CommandQueue.Enqueue(new CancelExitCommand());
  }
}
