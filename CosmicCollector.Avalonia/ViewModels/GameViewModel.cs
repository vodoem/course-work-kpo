using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
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
  private const double PixelsPerUnit = 0.8;
  private readonly GameRuntime _gameRuntime;
  private readonly NavigationService _mainMenuNavigation;
  private readonly NavigationService _gameOverNavigation;
  private readonly List<IDisposable> _subscriptions = new();
  private IReadOnlyList<RenderItem> _renderItems = Array.Empty<RenderItem>();
  private bool _isActive;
  private int _currentLevel;
  private int _requiredScore;
  private int _score;
  private int _energy;
  private string _blueProgressText = "B: 0/0";
  private string _greenProgressText = "G: 0/0";
  private string _redProgressText = "R: 0/0";
  private string _timerText = "—";
  private bool _isPaused;
  private int _countdownValue;
  private string _hudTargetScoreText = "Цель очков: 0";
  private string _hudTimerValueText = "—";
  private string _hudStatusText = "Ур. 0 | Энергия: 0 | Очки: 0";
  private string _hudGoalsText = "Цели: B=0 G=0 R=0";
  private string _hudProgressText = "Прогресс: B=0/0 G=0/0 R=0/0";
  private double _fieldWidth;
  private double _fieldHeight;

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
  }

  /// <summary>
  /// Команда возврата в главное меню.
  /// </summary>
  public ICommand BackToMenuCommand { get; }

  /// <summary>
  /// Элементы рендера игрового поля.
  /// </summary>
  public IReadOnlyList<RenderItem> RenderItems
  {
    get => _renderItems;
    private set
    {
      _renderItems = value;
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
    }
  }

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
        EnqueueMove(-1);
        break;
      case Key.D:
      case Key.Right:
        EnqueueMove(1);
        break;
      case Key.P:
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
      case Key.D:
      case Key.Right:
        EnqueueMove(0);
        break;
    }
  }

  private void InitializeFieldBounds()
  {
    var bounds = _gameRuntime.GameState.WorldBounds;
    FieldWidth = (bounds.Right - bounds.Left) * PixelsPerUnit;
    FieldHeight = (bounds.Bottom - bounds.Top) * PixelsPerUnit;
  }

  private void SubscribeToEvents()
  {
    _subscriptions.Add(_gameRuntime.EventBus.Subscribe<GameTick>(OnGameTick));
    _subscriptions.Add(_gameRuntime.EventBus.Subscribe<GameOver>(OnGameOver));
    _subscriptions.Add(_gameRuntime.EventBus.Subscribe<LevelCompleted>(OnLevelCompleted));
    _subscriptions.Add(_gameRuntime.EventBus.Subscribe<PauseToggled>(OnPauseToggled));
    _subscriptions.Add(_gameRuntime.EventBus.Subscribe<CountdownTick>(OnCountdownTick));
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
    var snapshot = _gameRuntime.GetSnapshot();
    Dispatcher.UIThread.Post(() => UpdateFromSnapshot(snapshot));
  }

  private void OnGameOver(GameOver parEvent)
  {
    Dispatcher.UIThread.Post(() =>
    {
      Deactivate();
      _gameOverNavigation.Navigate();
    });
  }

  private void OnLevelCompleted(LevelCompleted parEvent)
  {
    var snapshot = _gameRuntime.GetSnapshot();
    Dispatcher.UIThread.Post(() => UpdateFromSnapshot(snapshot));
  }

  private void OnPauseToggled(PauseToggled parEvent)
  {
    Dispatcher.UIThread.Post(() => IsPaused = parEvent.parIsPaused);
  }

  private void OnCountdownTick(CountdownTick parEvent)
  {
    Dispatcher.UIThread.Post(() => CountdownValue = parEvent.parValue);
  }

  private void UpdateFromSnapshot(GameSnapshot parSnapshot)
  {
    CurrentLevel = parSnapshot.parCurrentLevel;
    RequiredScore = parSnapshot.parRequiredScore;
    Score = parSnapshot.parDrone.parScore;
    Energy = parSnapshot.parDrone.parEnergy;
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
    RenderItems = BuildRenderItems(parSnapshot, _gameRuntime.GameState.WorldBounds);
  }

  private IReadOnlyList<RenderItem> BuildRenderItems(GameSnapshot parSnapshot, WorldBounds parBounds)
  {
    var items = new List<RenderItem>();

    AppendItem(items, parSnapshot.parDrone.parPosition, parSnapshot.parDrone.parBounds, parBounds, "drone");

    foreach (var crystal in parSnapshot.parCrystals)
    {
      AppendItem(items, crystal.parPosition, crystal.parBounds, parBounds, GetCrystalSpriteKey(crystal.parType));
    }

    foreach (var asteroid in parSnapshot.parAsteroids)
    {
      AppendItem(items, asteroid.parPosition, asteroid.parBounds, parBounds, "asteroid");
    }

    foreach (var bonus in parSnapshot.parBonuses)
    {
      AppendItem(items, bonus.parPosition, bonus.parBounds, parBounds, GetBonusSpriteKey(bonus.parType));
    }

    foreach (var blackHole in parSnapshot.parBlackHoles)
    {
      AppendItem(items, blackHole.parPosition, blackHole.parBounds, parBounds, "blackhole");
    }

    return items;
  }

  private void AppendItem(
    ICollection<RenderItem> parItems,
    Vector2 parPosition,
    Aabb parBounds,
    WorldBounds parWorldBounds,
    string parSpriteKey)
  {
    var width = parBounds.Width * PixelsPerUnit;
    var height = parBounds.Height * PixelsPerUnit;
    var left = (parPosition.X - (parBounds.Width / 2.0) - parWorldBounds.Left) * PixelsPerUnit;
    var top = (parPosition.Y - (parBounds.Height / 2.0) - parWorldBounds.Top) * PixelsPerUnit;

    parItems.Add(new RenderItem(left, top, width, height, parSpriteKey));
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

  private void HandleBackToMenu()
  {
    Deactivate();
    _mainMenuNavigation.Navigate();
  }
}
