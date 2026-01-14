using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using CosmicCollector.Avalonia.Commands;
using CosmicCollector.Avalonia.Infrastructure;
using CosmicCollector.Avalonia.Navigation;
using CosmicCollector.Core.Snapshots;
using CosmicCollector.Persistence.Records;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// ViewModel экрана завершения игры.
/// </summary>
public sealed class GameOverViewModel : ViewModelBase
{
  private const int MaxRecordsCount = 10;
  private const int MaxNameLength = 16;
  private readonly NavigationService _restartNavigation;
  private readonly NavigationService _backNavigation;
  private readonly IRecordsRepository _recordsRepository;
  private bool _isHighScore;
  private bool _isSaved;
  private string _playerName = string.Empty;
  private string _nameError = string.Empty;
  private IReadOnlyList<RecordsRowViewModel> _topRecords = Array.Empty<RecordsRowViewModel>();

  /// <summary>
  /// Инициализирует новый экземпляр <see cref="GameOverViewModel"/>.
  /// </summary>
  /// <param name="parSnapshot">Снимок состояния игры.</param>
  /// <param name="parRestartNavigation">Навигация для перезапуска игры.</param>
  /// <param name="parBackNavigation">Навигация назад в главное меню.</param>
  /// <param name="parRecordsRepository">Репозиторий рекордов.</param>
  public GameOverViewModel(
    GameSnapshot parSnapshot,
    NavigationService parRestartNavigation,
    NavigationService parBackNavigation,
    IRecordsRepository parRecordsRepository)
  {
    _restartNavigation = parRestartNavigation;
    _backNavigation = parBackNavigation;
    _recordsRepository = parRecordsRepository;
    RestartCommand = new NavigateCommand(parRestartNavigation);
    BackToMenuCommand = new NavigateCommand(parBackNavigation);
    SaveRecordCommand = new DelegateCommand(HandleSave);
    PrimaryActionCommand = new DelegateCommand(HandlePrimaryAction);
    Level = parSnapshot.parCurrentLevel;
    Score = parSnapshot.parDrone.parScore;
    Energy = parSnapshot.parDrone.parEnergy;
    TimerText = FormatTimer(parSnapshot.parHasLevelTimer, parSnapshot.parLevelTimeRemainingSec);
    LoadRecords();
    _isHighScore = CheckHighScore(Score, Level, _topRecords);
  }

  /// <summary>
  /// Команда перезапуска игры.
  /// </summary>
  public ICommand RestartCommand { get; }

  /// <summary>
  /// Команда возврата в главное меню.
  /// </summary>
  public ICommand BackToMenuCommand { get; }

  /// <summary>
  /// Команда сохранения рекорда (заглушка).
  /// </summary>
  public ICommand SaveRecordCommand { get; }

  /// <summary>
  /// Команда основного действия (Enter).
  /// </summary>
  public ICommand PrimaryActionCommand { get; }

  /// <summary>
  /// Уровень игрока.
  /// </summary>
  public int Level { get; }

  /// <summary>
  /// Итоговые очки.
  /// </summary>
  public int Score { get; }

  /// <summary>
  /// Энергия.
  /// </summary>
  public int Energy { get; }

  /// <summary>
  /// Текст времени уровня.
  /// </summary>
  public string TimerText { get; }

  /// <summary>
  /// Признак нового рекорда.
  /// </summary>
  public bool IsHighScore
  {
    get => _isHighScore;
    private set
    {
      _isHighScore = value;
      OnPropertyChanged();
      OnPropertyChanged(nameof(IsInputModeVisible));
      OnPropertyChanged(nameof(IsTableModeVisible));
    }
  }

  /// <summary>
  /// Признак сохранённого рекорда.
  /// </summary>
  public bool IsSaved
  {
    get => _isSaved;
    private set
    {
      _isSaved = value;
      OnPropertyChanged();
      OnPropertyChanged(nameof(IsInputModeVisible));
      OnPropertyChanged(nameof(IsTableModeVisible));
    }
  }

  /// <summary>
  /// Имя игрока.
  /// </summary>
  public string PlayerName
  {
    get => _playerName;
    set
    {
      if (_playerName == value)
      {
        return;
      }

      _playerName = value;
      OnPropertyChanged();
      if (!string.IsNullOrWhiteSpace(_nameError))
      {
        _nameError = string.Empty;
        OnPropertyChanged(nameof(NameError));
      }
    }
  }

  /// <summary>
  /// Сообщение об ошибке имени.
  /// </summary>
  public string NameError
  {
    get => _nameError;
    private set
    {
      _nameError = value;
      OnPropertyChanged();
      OnPropertyChanged(nameof(HasNameError));
    }
  }

  /// <summary>
  /// Признак наличия ошибки имени.
  /// </summary>
  public bool HasNameError => !string.IsNullOrWhiteSpace(NameError);

  /// <summary>
  /// Режим ввода имени.
  /// </summary>
  public bool IsInputModeVisible => IsHighScore && !IsSaved;

  /// <summary>
  /// Режим таблицы рекордов.
  /// </summary>
  public bool IsTableModeVisible => !IsInputModeVisible;

  /// <summary>
  /// Коллекция рекордов.
  /// </summary>
  public IReadOnlyList<RecordsRowViewModel> TopRecords
  {
    get => _topRecords;
    private set
    {
      _topRecords = value;
      OnPropertyChanged();
      OnPropertyChanged(nameof(HasRecords));
      OnPropertyChanged(nameof(NoRecords));
    }
  }

  /// <summary>
  /// Признак наличия записей.
  /// </summary>
  public bool HasRecords => TopRecords.Count > 0;

  /// <summary>
  /// Признак отсутствия записей.
  /// </summary>
  public bool NoRecords => !HasRecords;

  private void HandlePrimaryAction()
  {
    if (IsInputModeVisible)
    {
      HandleSave();
      return;
    }

    _backNavigation.Navigate();
  }

  private void HandleSave()
  {
    if (!IsInputModeVisible)
    {
      return;
    }

    if (!ValidateName(PlayerName, out var error))
    {
      NameError = error;
      return;
    }

    var timestamp = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
    var record = new RecordEntry(PlayerName.Trim(), Score, Level, timestamp);
    _recordsRepository.Add(record);
    IsSaved = true;
    LoadRecords();
  }

  private void LoadRecords()
  {
    var records = _recordsRepository.LoadAll();
    TopRecords = BuildRecordsList(records);
  }

  private static IReadOnlyList<RecordsRowViewModel> BuildRecordsList(IReadOnlyList<RecordEntry> parRecords)
  {
    return parRecords
      .OrderByDescending(record => record.parScore)
      .ThenByDescending(record => record.parLevel)
      .Take(MaxRecordsCount)
      .Select((record, index) => new RecordsRowViewModel(
        index + 1,
        record.parPlayerName,
        record.parScore,
        record.parLevel,
        FormatTimestamp(record.parTimestampUtc)))
      .ToList();
  }

  private static bool CheckHighScore(int parScore, int parLevel, IReadOnlyList<RecordsRowViewModel> parTop)
  {
    if (parTop.Count < MaxRecordsCount)
    {
      return true;
    }

    var worst = parTop.Last();
    if (parScore > worst.Score)
    {
      return true;
    }

    return parScore == worst.Score && parLevel > worst.Level;
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

  private static string FormatTimestamp(string parTimestampUtc)
  {
    if (DateTime.TryParse(parTimestampUtc, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var value))
    {
      return value.ToUniversalTime().ToString("dd.MM.yyyy HH:mm 'UTC'", CultureInfo.InvariantCulture);
    }

    return parTimestampUtc;
  }

  private static bool ValidateName(string parName, out string parError)
  {
    var trimmed = parName.Trim();
    if (trimmed.Length < 1 || trimmed.Length > MaxNameLength)
    {
      parError = $"Имя должно быть длиной 1–{MaxNameLength} символов.";
      return false;
    }

    foreach (var symbol in trimmed)
    {
      if (char.IsLetterOrDigit(symbol) || symbol == ' ' || symbol == '_' || symbol == '-')
      {
        continue;
      }

      parError = "Допустимы буквы, цифры, пробел, _ и -.";
      return false;
    }

    parError = string.Empty;
    return true;
  }
}
