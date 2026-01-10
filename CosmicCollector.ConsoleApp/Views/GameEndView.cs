using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.Core.Snapshots;
using CosmicCollector.Persistence.Records;

namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Представление экрана завершения игры.
/// </summary>
public sealed class GameEndView : IGameEndView
{
  private readonly IConsoleRenderer _renderer;

  /// <summary>
  /// Создаёт представление экрана завершения.
  /// </summary>
  /// <param name="parRenderer">Рендерер консоли.</param>
  public GameEndView(IConsoleRenderer parRenderer)
  {
    _renderer = parRenderer;
  }

  /// <inheritdoc />
  public void Render(
    GameEndReason parReason,
    GameSnapshot parSnapshot,
    int parLevel,
    bool parIsHighScore,
    string parPlayerName,
    bool parIsSaved,
    IReadOnlyList<RecordEntry> parTopRecords)
  {
    _renderer.Clear();
    _renderer.SetCursorPosition(0, 0);
    _renderer.SetForegroundColor(ConsoleColor.White);

    string title = parIsHighScore
      ? "РЕКОРДЫ"
      : parReason == GameEndReason.LevelCompleted
        ? "УРОВЕНЬ ПРОЙДЕН"
        : "ИГРА ОКОНЧЕНА";

    _renderer.WriteLine(title);

    if (parIsHighScore)
    {
      _renderer.WriteLine(parReason == GameEndReason.LevelCompleted
        ? "Победа!"
        : "Поражение.");
    }

    _renderer.WriteLine(string.Empty);
    _renderer.WriteLine($"Уровень: {parLevel}");
    _renderer.WriteLine($"Очки: {parSnapshot.parDrone.parScore}");
    _renderer.WriteLine($"Энергия: {parSnapshot.parDrone.parEnergy}");

    string timeText = parSnapshot.parHasLevelTimer
      ? $"{Math.Max(0, Math.Floor(parSnapshot.parLevelTimeRemainingSec)):0}с"
      : "—";
    _renderer.WriteLine($"Время: {timeText}");
    _renderer.WriteLine(string.Empty);

    if (parIsHighScore)
    {
      RenderHighScoreBlock(parPlayerName, parIsSaved, parTopRecords);
      return;
    }

    _renderer.WriteLine("Enter/Esc — в меню, R — заново");
  }

  /// <summary>
  /// Выполняет RenderHighScoreBlock.
  /// </summary>
  private void RenderHighScoreBlock(
    string parPlayerName,
    bool parIsSaved,
    IReadOnlyList<RecordEntry> parTopRecords)
  {
    if (!parIsSaved)
    {
      _renderer.WriteLine("Новый рекорд!");
      _renderer.WriteLine("Введите имя (1–16):");
      _renderer.WriteLine($"Имя: {parPlayerName}");
      _renderer.WriteLine(string.Empty);
      _renderer.WriteLine("Enter — сохранить");
      _renderer.WriteLine("Esc — в меню, R — заново");
      return;
    }

    _renderer.WriteLine("Сохранено.");
    _renderer.WriteLine(string.Empty);
    _renderer.WriteLine("Место | Имя | Уровень | Очки | Дата (UTC)");
    _renderer.WriteLine(new string('-', 50));

    if (parTopRecords.Count == 0)
    {
      _renderer.WriteLine("Записей нет.");
    }
    else
    {
      for (int i = 0; i < parTopRecords.Count; i++)
      {
        RecordEntry record = parTopRecords[i];
        string place = (i + 1).ToString().PadLeft(5);
        string name = TrimValue(record.parPlayerName, 20).PadRight(20);
        string level = record.parLevel.ToString().PadLeft(6);
        string score = record.parScore.ToString().PadLeft(4);
        string date = record.parTimestampUtc;
        _renderer.WriteLine($"{place} | {name} | {level} | {score} | {date}");
      }
    }

    _renderer.WriteLine(string.Empty);
    _renderer.WriteLine("Enter/Esc — в меню, R — заново");
  }

  /// <summary>
  /// Выполняет TrimValue.
  /// </summary>
  private static string TrimValue(string parValue, int parMaxLength)
  {
    if (parValue.Length <= parMaxLength)
    {
      return parValue;
    }

    return parValue[..parMaxLength];
  }
}
