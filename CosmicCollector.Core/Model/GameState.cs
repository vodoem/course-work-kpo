namespace CosmicCollector.Core.Model;

/// <summary>
/// Представляет изменяемое состояние игры для ядра.
/// </summary>
public sealed class GameState
{
  private readonly object _lockObject = new();
  private bool _isPaused;
  private long _tickNo;

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
      _isPaused = !_isPaused;
      return _isPaused;
    }
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
      return new Snapshots.GameSnapshot(_isPaused, _tickNo);
    }
  }
}
