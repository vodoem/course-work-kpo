using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.Persistence.Records;

namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Представление экрана рекордов.
/// </summary>
public sealed class RecordsView : IRecordsView
{
  private readonly IConsoleRenderer _renderer;

  /// <summary>
  /// Создаёт представление рекордов.
  /// </summary>
  /// <param name="parRenderer">Рендерер консоли.</param>
  public RecordsView(IConsoleRenderer parRenderer)
  {
    _renderer = parRenderer;
  }

  /// <inheritdoc />
  public void Render(IReadOnlyList<RecordEntry> parRecords, int parStartIndex)
  {
    _renderer.Clear();
    _renderer.WriteLine("РЕКОРДЫ");
    _renderer.WriteLine(string.Empty);
    _renderer.WriteLine("Место | Имя | Уровень | Очки | Дата (UTC)");
    _renderer.WriteLine(new string('-', 50));

    if (parRecords.Count == 0)
    {
      _renderer.WriteLine("Записей нет.");
      FillEmptyLines(PageSize - 1);
      _renderer.WriteLine(string.Empty);
      _renderer.WriteLine("Esc — назад в меню");
      return;
    }

    int availableHeight = PageSize;
    int startIndex = Math.Clamp(parStartIndex, 0, Math.Max(0, parRecords.Count - availableHeight));
    int endIndex = Math.Min(parRecords.Count, startIndex + availableHeight);

    for (int i = startIndex; i < endIndex; i++)
    {
      RecordEntry record = parRecords[i];
      string place = (i + 1).ToString().PadLeft(5);
      string name = TrimValue(record.parPlayerName, 20).PadRight(20);
      string level = record.parLevel.ToString().PadLeft(6);
      string score = record.parScore.ToString().PadLeft(4);
      string date = record.parTimestampUtc;
      _renderer.WriteLine($"{place} | {name} | {level} | {score} | {date}");
    }

    FillEmptyLines(startIndex + availableHeight - endIndex);
    _renderer.WriteLine(string.Empty);
    _renderer.WriteLine("Esc — назад в меню");
  }

  /// <inheritdoc />
  public int PageSize => Math.Max(1, _renderer.BufferHeight - 6);

  /// <summary>
  /// Выполняет FillEmptyLines.
  /// </summary>
  private void FillEmptyLines(int parCount)
  {
    for (int i = 0; i < parCount; i++)
    {
      _renderer.WriteLine(string.Empty);
    }
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
