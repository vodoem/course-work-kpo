using CosmicCollector.ConsoleApp.Controllers;
using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;
using CosmicCollector.Persistence.Records;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет работу контроллера рекордов.
/// </summary>
public sealed class RecordsControllerTests
{
  /// <summary>
  /// Проверяет, что Esc завершает работу экрана.
  /// </summary>
  [Xunit.Fact]
  public void Run_EscapeKey_ReturnsToCaller()
  {
    var input = new TestInputReader(new[] { ConsoleKey.Escape });
    var view = new TestRecordsView(pageSize: 3);
    var repository = new TestRecordsRepository(new[]
    {
      new RecordEntry("Player", 10, 1, "2024-01-01T00:00:00Z")
    });
    var controller = new RecordsController(view, input, repository);

    controller.Run();

    Xunit.Assert.True(view.RenderCalls > 0);
  }

  /// <summary>
  /// Проверяет, что прокрутка не выходит за границы при пустом списке.
  /// </summary>
  [Xunit.Fact]
  public void ScrollLines_ClampsStartIndex_WhenEmpty()
  {
    var input = new TestInputReader(Array.Empty<ConsoleKey>());
    var view = new TestRecordsView(pageSize: 3);
    var repository = new TestRecordsRepository(Array.Empty<RecordEntry>());
    var controller = new RecordsController(view, input, repository);

    controller.ScrollLines(10);

    Xunit.Assert.Equal(0, controller.StartIndex);
  }

  /// <summary>
  /// Проверяет, что прокрутка на страницу не выходит за границы списка.
  /// </summary>
  [Xunit.Fact]
  public void ScrollPage_ClampsStartIndex_WhenOverPage()
  {
    var input = new TestInputReader(Array.Empty<ConsoleKey>());
    var view = new TestRecordsView(pageSize: 2);
    var repository = new TestRecordsRepository(new[]
    {
      new RecordEntry("A", 10, 1, "2024-01-01T00:00:00Z"),
      new RecordEntry("B", 9, 1, "2024-01-02T00:00:00Z"),
      new RecordEntry("C", 8, 1, "2024-01-03T00:00:00Z"),
      new RecordEntry("D", 7, 1, "2024-01-04T00:00:00Z"),
      new RecordEntry("E", 6, 1, "2024-01-05T00:00:00Z")
    });
    var controller = new RecordsController(view, input, repository);

    controller.ScrollPage(1);
    controller.ScrollPage(1);

    Xunit.Assert.Equal(3, controller.StartIndex);
  }

  private sealed class TestRecordsView : IRecordsView
  {
    /// <summary>
    /// Проверяет сценарий TestRecordsView.
    /// </summary>
    public TestRecordsView(int pageSize)
    {
      PageSize = pageSize;
    }

    public int RenderCalls { get; private set; }

    public int PageSize { get; }

    /// <summary>
    /// Проверяет сценарий Render.
    /// </summary>
    public void Render(IReadOnlyList<RecordEntry> parRecords, int parStartIndex)
    {
      RenderCalls++;
    }
  }

  private sealed class TestRecordsRepository : IRecordsRepository
  {
    private readonly IReadOnlyList<RecordEntry> _records;

    /// <summary>
    /// Проверяет сценарий TestRecordsRepository.
    /// </summary>
    public TestRecordsRepository(IReadOnlyList<RecordEntry> parRecords)
    {
      _records = parRecords;
    }

    /// <summary>
    /// Проверяет сценарий LoadAll.
    /// </summary>
    public IReadOnlyList<RecordEntry> LoadAll()
    {
      return _records;
    }

    /// <summary>
    /// Проверяет сценарий SaveAll.
    /// </summary>
    public void SaveAll(IReadOnlyList<RecordEntry> parRecords)
    {
    }

    /// <summary>
    /// Проверяет сценарий Add.
    /// </summary>
    public void Add(RecordEntry parRecord)
    {
    }
  }

  private sealed class TestInputReader : IConsoleInputReader
  {
    private readonly Queue<ConsoleKey> _keys;

    /// <summary>
    /// Проверяет сценарий TestInputReader.
    /// </summary>
    public TestInputReader(IEnumerable<ConsoleKey> parKeys)
    {
      _keys = new Queue<ConsoleKey>(parKeys);
    }

    /// <summary>
    /// Проверяет сценарий ReadKey.
    /// </summary>
    public ConsoleKeyInfo ReadKey()
    {
      if (_keys.Count == 0)
      {
        return new ConsoleKeyInfo('\0', ConsoleKey.NoName, false, false, false);
      }

      ConsoleKey key = _keys.Dequeue();
      return new ConsoleKeyInfo('\0', key, false, false, false);
    }

    /// <summary>
    /// Проверяет сценарий ClearBuffer.
    /// </summary>
    public void ClearBuffer()
    {
    }
  }
}
