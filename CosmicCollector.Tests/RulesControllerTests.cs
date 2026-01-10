using CosmicCollector.ConsoleApp.Controllers;
using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет работу контроллера правил.
/// </summary>
public sealed class RulesControllerTests
{
  /// <summary>
  /// Проверяет, что Esc завершает работу экрана.
  /// </summary>
  [Xunit.Fact]
  public void Run_EscapeKey_ReturnsToCaller()
  {
    var input = new TestInputReader(new[] { ConsoleKey.Escape });
    var view = new TestRulesView(pageSize: 3);
    var provider = new TestRulesTextProvider(new[] { "Строка 1", "Строка 2" });
    var controller = new RulesController(view, input, provider);

    controller.Run();

    Xunit.Assert.True(view.RenderCalls > 0);
  }

  /// <summary>
  /// Проверяет, что прокрутка не выходит за границы текста.
  /// </summary>
  [Xunit.Fact]
  public void ScrollLines_ClampsStartLineWithinBounds()
  {
    var input = new TestInputReader(Array.Empty<ConsoleKey>());
    var view = new TestRulesView(pageSize: 3);
    var provider = new TestRulesTextProvider(new[] { "1", "2", "3", "4", "5" });
    var controller = new RulesController(view, input, provider);

    controller.ScrollLines(-10);
    Xunit.Assert.Equal(0, controller.StartLine);

    controller.ScrollLines(10);
    Xunit.Assert.Equal(2, controller.StartLine);
  }

  /// <summary>
  /// Проверяет границы при постраничной прокрутке.
  /// </summary>
  [Xunit.Fact]
  public void ScrollPage_ClampsStartLineWithinBounds()
  {
    var input = new TestInputReader(Array.Empty<ConsoleKey>());
    var view = new TestRulesView(pageSize: 2);
    var provider = new TestRulesTextProvider(new[] { "1", "2", "3" });
    var controller = new RulesController(view, input, provider);

    controller.ScrollPage(1);
    Xunit.Assert.Equal(1, controller.StartLine);

    controller.ScrollPage(1);
    Xunit.Assert.Equal(1, controller.StartLine);
  }

  private sealed class TestRulesView : IRulesView
  {
    /// <summary>
    /// Проверяет сценарий TestRulesView.
    /// </summary>
    public TestRulesView(int pageSize)
    {
      PageSize = pageSize;
    }

    public int RenderCalls { get; private set; }

    public int PageSize { get; }

    /// <summary>
    /// Проверяет сценарий Render.
    /// </summary>
    public void Render(string[] parLines, int parStartLine)
    {
      RenderCalls++;
    }
  }

  private sealed class TestRulesTextProvider : IRulesTextProvider
  {
    private readonly string[] _lines;

    /// <summary>
    /// Проверяет сценарий TestRulesTextProvider.
    /// </summary>
    public TestRulesTextProvider(string[] parLines)
    {
      _lines = parLines;
    }

    /// <summary>
    /// Проверяет сценарий GetLines.
    /// </summary>
    public string[] GetLines()
    {
      return _lines;
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
