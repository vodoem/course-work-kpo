using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CosmicCollector.Core.Model;

/// <summary>
/// Представляет изменяемое состояние игры для ядра.
/// </summary>
public sealed class GameState
{
  private readonly object _lockObject = new();
  private readonly Drone _drone;
  private readonly WorldBounds _worldBounds;
  private readonly List<Crystal> _crystals = new();
  private readonly List<Asteroid> _asteroids = new();
  private readonly List<Bonus> _bonuses = new();
  private readonly List<BlackHole> _blackHoles = new();
  private bool _isPaused;
  private long _tickNo;
  private int _score;
  private int _requiredScore;
  private double _acceleratorRemainingSec;
  private double _timeStabilizerRemainingSec;
  private double _magnetRemainingSec;
  private double _levelTimeRemainingSec;
  private bool _hasLevelTimer;
  private bool _collectedBlue;
  private bool _collectedGreen;
  private bool _collectedRed;
  private bool _isLevelCompleted;
  private bool _isGameOver;
  private bool _isResumeCountdownActive;
  private int _resumeCountdownValue;
  private double _resumeCountdownAccumulatedSec;

  /// <summary>
  /// Инициализирует состояние игры со стандартным дроном.
  /// </summary>
  public GameState()
    : this(
      new Drone(Guid.NewGuid(), Vector2.Zero, Vector2.Zero, new Aabb(32, 32), 100),
      new WorldBounds(0, 0, 800, 600))
  {
  }

  /// <summary>
  /// Инициализирует состояние игры.
  /// </summary>
  /// <param name="parDrone">Начальное состояние дрона.</param>
  public GameState(Drone parDrone, WorldBounds parWorldBounds)
  {
    _drone = parDrone;
    _worldBounds = parWorldBounds;
  }

  /// <summary>
  /// Объект синхронизации состояния.
  /// </summary>
  internal object SyncRoot => _lockObject;

  /// <summary>
  /// Доступ к списку кристаллов внутри блокировки.
  /// </summary>
  internal List<Crystal> CrystalsInternal => _crystals;

  /// <summary>
  /// Доступ к списку астероидов внутри блокировки.
  /// </summary>
  internal List<Asteroid> AsteroidsInternal => _asteroids;

  /// <summary>
  /// Доступ к списку бонусов внутри блокировки.
  /// </summary>
  internal List<Bonus> BonusesInternal => _bonuses;

  /// <summary>
  /// Доступ к списку чёрных дыр внутри блокировки.
  /// </summary>
  internal List<BlackHole> BlackHolesInternal => _blackHoles;

  /// <summary>
  /// Доступ к дрону внутри блокировки.
  /// </summary>
  internal Drone DroneInternal => _drone;

  /// <summary>
  /// Границы игрового мира.
  /// </summary>
  public WorldBounds WorldBounds => _worldBounds;

  /// <summary>
  /// Остаток действия ускорителя в секундах.
  /// </summary>
  internal double AcceleratorRemainingSec
  {
    get => _acceleratorRemainingSec;
    set => _acceleratorRemainingSec = value;
  }

  /// <summary>
  /// Остаток действия стабилизатора времени в секундах.
  /// </summary>
  internal double TimeStabilizerRemainingSec
  {
    get => _timeStabilizerRemainingSec;
    set => _timeStabilizerRemainingSec = value;
  }

  /// <summary>
  /// Остаток действия магнита в секундах.
  /// </summary>
  internal double MagnetRemainingSec
  {
    get => _magnetRemainingSec;
    set => _magnetRemainingSec = value;
  }

  /// <summary>
  /// Оставшееся время уровня в секундах.
  /// </summary>
  public double LevelTimeRemainingSec
  {
    get
    {
      lock (_lockObject)
      {
        return _levelTimeRemainingSec;
      }
    }
    set
    {
      lock (_lockObject)
      {
        _levelTimeRemainingSec = value;
        _hasLevelTimer = true;
      }
    }
  }

  /// <summary>
  /// Признак наличия таймера уровня.
  /// </summary>
  internal bool HasLevelTimer
  {
    get
    {
      lock (_lockObject)
      {
        return _hasLevelTimer;
      }
    }
  }

  /// <summary>
  /// Требуемые очки для завершения уровня.
  /// </summary>
  public int RequiredScore
  {
    get
    {
      lock (_lockObject)
      {
        return _requiredScore;
      }
    }
    set
    {
      lock (_lockObject)
      {
        _requiredScore = value;
      }
    }
  }

  /// <summary>
  /// Признак завершения уровня.
  /// </summary>
  public bool IsLevelCompleted
  {
    get
    {
      lock (_lockObject)
      {
        return _isLevelCompleted;
      }
    }
  }

  /// <summary>
  /// Признак окончания игры.
  /// </summary>
  public bool IsGameOver
  {
    get
    {
      lock (_lockObject)
      {
        return _isGameOver;
      }
    }
  }

  /// <summary>
  /// Получает признак паузы.
  /// </summary>
  public bool IsPaused
  {
    get
    {
      lock (_lockObject)
      {
        return _isPaused;
      }
    }
  }

  /// <summary>
  /// Переключает состояние паузы и возвращает новое значение.
  /// </summary>
  /// <returns>Новое состояние паузы.</returns>
  public bool TogglePause()
  {
    lock (_lockObject)
    {
      if (!_isPaused)
      {
        _isPaused = true;
        return _isPaused;
      }

      StartResumeCountdownLocked();
      return _isPaused;
    }
  }

  /// <summary>
  /// Признак активного отсчёта при снятии паузы.
  /// </summary>
  internal bool IsResumeCountdownActive
  {
    get
    {
      lock (_lockObject)
      {
        return _isResumeCountdownActive;
      }
    }
  }

  /// <summary>
  /// Текущее значение отсчёта.
  /// </summary>
  internal int ResumeCountdownValue
  {
    get => _resumeCountdownValue;
    set => _resumeCountdownValue = value;
  }

  /// <summary>
  /// Накопленное время для шага отсчёта.
  /// </summary>
  internal double ResumeCountdownAccumulatedSec
  {
    get => _resumeCountdownAccumulatedSec;
    set => _resumeCountdownAccumulatedSec = value;
  }

  /// <summary>
  /// Снимает паузу без запуска отсчёта.
  /// </summary>
  /// <param name="parIsPaused">Новое состояние паузы.</param>
  internal void SetPaused(bool parIsPaused)
  {
    _isPaused = parIsPaused;
  }

  /// <summary>
  /// Увеличивает номер тика на единицу.
  /// </summary>
  /// <returns>Новый номер тика.</returns>
  public long AdvanceTick()
  {
    lock (_lockObject)
    {
      _tickNo++;
      return _tickNo;
    }
  }

  /// <summary>
  /// Возвращает неизменяемый снимок состояния.
  /// </summary>
  /// <returns>Снимок состояния.</returns>
  public Snapshots.GameSnapshot GetSnapshot()
  {
    lock (_lockObject)
    {
      var droneSnapshot = new Snapshots.DroneSnapshot(
        _drone.Id,
        _drone.Position,
        _drone.Velocity,
        _drone.Bounds,
        _drone.Energy,
        _score);

      var crystals = _crystals
        .Select(crystal => new Snapshots.CrystalSnapshot(
          crystal.Id,
          crystal.Position,
          crystal.Velocity,
          crystal.Bounds,
          crystal.Type))
        .ToList();

      var asteroids = _asteroids
        .Select(asteroid => new Snapshots.AsteroidSnapshot(
          asteroid.Id,
          asteroid.Position,
          asteroid.Velocity,
          asteroid.Bounds))
        .ToList();

      var bonuses = _bonuses
        .Select(bonus => new Snapshots.BonusSnapshot(
          bonus.Id,
          bonus.Position,
          bonus.Velocity,
          bonus.Bounds,
          bonus.Type,
          bonus.DurationSec))
        .ToList();

      var blackHoles = _blackHoles
        .Select(blackHole => new Snapshots.BlackHoleSnapshot(
          blackHole.Id,
          blackHole.Position,
          blackHole.Velocity,
          blackHole.Bounds,
          blackHole.Radius,
          blackHole.CoreRadius))
        .ToList();

      return new Snapshots.GameSnapshot(
        _isPaused,
        _tickNo,
        droneSnapshot,
        crystals,
        asteroids,
        bonuses,
        blackHoles);
    }
  }

  /// <summary>
  /// Возвращает текущие очки.
  /// </summary>
  public int Score
  {
    get
    {
      lock (_lockObject)
      {
        return _score;
      }
    }
  }

  /// <summary>
  /// Возвращает ссылку на дрон.
  /// </summary>
  public Drone Drone
  {
    get
    {
      lock (_lockObject)
      {
        return _drone;
      }
    }
  }

  /// <summary>
  /// Добавляет кристалл.
  /// </summary>
  /// <param name="parCrystal">Кристалл.</param>
  public void AddCrystal(Crystal parCrystal)
  {
    lock (_lockObject)
    {
      _crystals.Add(parCrystal);
    }
  }

  /// <summary>
  /// Добавляет астероид.
  /// </summary>
  /// <param name="parAsteroid">Астероид.</param>
  public void AddAsteroid(Asteroid parAsteroid)
  {
    lock (_lockObject)
    {
      _asteroids.Add(parAsteroid);
    }
  }

  /// <summary>
  /// Добавляет бонус.
  /// </summary>
  /// <param name="parBonus">Бонус.</param>
  public void AddBonus(Bonus parBonus)
  {
    lock (_lockObject)
    {
      _bonuses.Add(parBonus);
    }
  }

  /// <summary>
  /// Добавляет чёрную дыру.
  /// </summary>
  /// <param name="parBlackHole">Чёрная дыра.</param>
  public void AddBlackHole(BlackHole parBlackHole)
  {
    lock (_lockObject)
    {
      _blackHoles.Add(parBlackHole);
    }
  }

  /// <summary>
  /// Добавляет очки.
  /// </summary>
  /// <param name="parPoints">Количество очков.</param>
  internal void AddScore(int parPoints)
  {
    _score += parPoints;
  }

  /// <summary>
  /// Отмечает сбор кристалла указанного типа.
  /// </summary>
  /// <param name="parType">Тип кристалла.</param>
  internal void MarkCrystalCollected(CrystalType parType)
  {
    switch (parType)
    {
      case CrystalType.Blue:
        _collectedBlue = true;
        break;
      case CrystalType.Green:
        _collectedGreen = true;
        break;
      case CrystalType.Red:
        _collectedRed = true;
        break;
    }
  }

  /// <summary>
  /// Проверяет, что все типы кристаллов собраны.
  /// </summary>
  /// <returns>Истина, если все типы собраны.</returns>
  internal bool HasAllCrystalTypes()
  {
    return _collectedBlue && _collectedGreen && _collectedRed;
  }

  /// <summary>
  /// Помечает уровень завершённым.
  /// </summary>
  internal void MarkLevelCompleted()
  {
    _isLevelCompleted = true;
  }

  /// <summary>
  /// Помечает игру завершённой.
  /// </summary>
  internal void MarkGameOver()
  {
    _isGameOver = true;
  }

  private void StartResumeCountdownLocked()
  {
    if (_isResumeCountdownActive)
    {
      return;
    }

    _isResumeCountdownActive = true;
    _resumeCountdownValue = 3;
    _resumeCountdownAccumulatedSec = 0;
  }

  internal void StopResumeCountdown()
  {
    _isResumeCountdownActive = false;
    _resumeCountdownValue = 0;
    _resumeCountdownAccumulatedSec = 0;
  }
}
