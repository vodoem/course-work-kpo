using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;
using CosmicCollector.Persistence.Records;

namespace CosmicCollector.ConsoleApp.Controllers;

/// <summary>
/// Контроллер экрана рекордов.
/// </summary>
public sealed class RecordsController
{
  private readonly IRecordsView _view;
  private readonly IConsoleInputReader _inputReader;
  private readonly IReadOnlyList<RecordEntry> _records;
  private int _startIndex;

  /// <summary>
  /// Создаёт контроллер рекордов.
  /// </summary>
  /// <param name="parView">Представление рекордов.</param>
  /// <param name="parInputReader">Читатель ввода.</param>
  /// <param name="parRecordsRepository">Репозиторий рекордов.</param>
  public RecordsController(
    IRecordsView parView,
    IConsoleInputReader parInputReader,
    IRecordsRepository parRecordsRepository)
  {
    _view = parView;
    _inputReader = parInputReader;
    _records = parRecordsRepository.LoadAll();
    _startIndex = 0;
  }

  /// <summary>
  /// Индекс первой отображаемой записи.
  /// </summary>
  public int StartIndex => _startIndex;

  /// <summary>
  /// Количество рекордов.
  /// </summary>
  public int TotalRecords => _records.Count;

  /// <summary>
  /// Запускает цикл обработки ввода на экране рекордов.
  /// </summary>
  public void Run()
  {
    // Порядок записей сохраняется таким, как его возвращает репозиторий.
    Render();

    while (true)
    {
      ConsoleKeyInfo keyInfo = _inputReader.ReadKey();

      if (keyInfo.Key == ConsoleKey.Escape)
      {
        return;
      }

      if (keyInfo.Key == ConsoleKey.UpArrow)
      {
        ScrollLines(-1);
        continue;
      }

      if (keyInfo.Key == ConsoleKey.DownArrow)
      {
        ScrollLines(1);
        continue;
      }

      if (keyInfo.Key == ConsoleKey.PageUp)
      {
        ScrollPage(-1);
        continue;
      }

      if (keyInfo.Key == ConsoleKey.PageDown)
      {
        ScrollPage(1);
      }
    }
  }

  /// <summary>
  /// Прокручивает список на указанное число строк.
  /// </summary>
  /// <param name="parDelta">Смещение.</param>
  public void ScrollLines(int parDelta)
  {
    _startIndex = ClampStartIndex(_startIndex + parDelta);
    Render();
  }

  /// <summary>
  /// Прокручивает список на страницу.
  /// </summary>
  /// <param name="parDirection">Направление прокрутки.</param>
  public void ScrollPage(int parDirection)
  {
    int pageSize = Math.Max(1, _view.PageSize);
    _startIndex = ClampStartIndex(_startIndex + (pageSize * parDirection));
    Render();
  }

  private int ClampStartIndex(int parValue)
  {
    int maxStart = Math.Max(0, _records.Count - Math.Max(1, _view.PageSize));
    return Math.Clamp(parValue, 0, maxStart);
  }

  private void Render()
  {
    _view.Render(_records, _startIndex);
  }
}
