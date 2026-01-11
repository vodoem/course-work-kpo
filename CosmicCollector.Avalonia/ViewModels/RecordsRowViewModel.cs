namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// Представляет строку таблицы рекордов.
/// </summary>
public sealed class RecordsRowViewModel
{
  /// <summary>
  /// Инициализирует новый экземпляр <see cref="RecordsRowViewModel"/>.
  /// </summary>
  /// <param name="parPosition">Позиция в таблице.</param>
  /// <param name="parPlayerName">Имя игрока.</param>
  /// <param name="parScore">Очки.</param>
  /// <param name="parLevel">Уровень.</param>
  public RecordsRowViewModel(int parPosition, string parPlayerName, int parScore, int parLevel)
  {
    Position = parPosition;
    PlayerName = parPlayerName;
    Score = parScore;
    Level = parLevel;
  }

  /// <summary>
  /// Позиция в таблице.
  /// </summary>
  public int Position { get; }

  /// <summary>
  /// Имя игрока.
  /// </summary>
  public string PlayerName { get; }

  /// <summary>
  /// Очки.
  /// </summary>
  public int Score { get; }

  /// <summary>
  /// Уровень.
  /// </summary>
  public int Level { get; }
}
