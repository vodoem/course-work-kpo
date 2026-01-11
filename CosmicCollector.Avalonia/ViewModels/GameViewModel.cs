using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia.Threading;
using CosmicCollector.Avalonia.Commands;
using CosmicCollector.Avalonia.Infrastructure;
using CosmicCollector.Avalonia.Navigation;
using CosmicCollector.Core.Events;
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
  private bool _isActive;
  private int _currentLevel;
  private int _requiredScore;
  private int _score;
  private int _energy;
  private string _blueProgressText = "B: 0/0";
  private string _greenProgressText = "G: 0/0";
  private string _redProgressText = "R: 0/0";
  private string _timerText = "Таймер: —";
  private bool _isPaused;
  private int _countdownValue;

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
  public void HandleKeyDown(Avalonia.Input.Key parKey)
  {
    if (!_isActive)
    {
      return;
    }

    switch (parKey)
    {
      case Avalonia.Input.Key.A:
      case Avalonia.Input.Key.Left:
        EnqueueMove(-1);
        break;
      case Avalonia.Input.Key.D:
      case Avalonia.Input.Key.Right:
        EnqueueMove(1);
        break;
      case Avalonia.Input.Key.P:
        _gameRuntime.CommandQueue.Enqueue(new TogglePauseCommand());
        break;
    }
  }

  /// <summary>
  /// Обрабатывает отпускание клавиш.
  /// </summary>
  /// <param name="parKey">Отпущенная клавиша.</param>
  public void HandleKeyUp(Avalonia.Input.Key parKey)
  {
    if (!_isActive)
    {
      return;
    }

    switch (parKey)
    {
      case Avalonia.Input.Key.A:
      case Avalonia.Input.Key.Left:
      case Avalonia.Input.Key.D:
      case Avalonia.Input.Key.Right:
        EnqueueMove(0);
        break;
    }
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
    TimerText = parSnapshot.parHasLevelTimer
      ? $"Таймер: {parSnapshot.parLevelTimeRemainingSec:0}"
      : "Таймер: —";
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
