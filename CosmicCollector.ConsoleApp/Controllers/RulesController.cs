using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;

namespace CosmicCollector.ConsoleApp.Controllers;

/// <summary>
/// Контроллер экрана правил.
/// </summary>
public sealed class RulesController
{
  private readonly IRulesView _view;
  private readonly IConsoleInputReader _inputReader;
  private readonly string[] _lines;
  private int _startLine;

  /// <summary>
  /// Создаёт контроллер правил.
  /// </summary>
  /// <param name="parView">Представление правил.</param>
  /// <param name="parInputReader">Читатель ввода.</param>
  /// <param name="parRulesTextProvider">Поставщик текста правил.</param>
  public RulesController(
    IRulesView parView,
    IConsoleInputReader parInputReader,
    IRulesTextProvider parRulesTextProvider)
  {
    _view = parView;
    _inputReader = parInputReader;
    _lines = parRulesTextProvider.GetLines();
    _startLine = 0;
  }

  /// <summary>
  /// Текущий индекс первой строки.
  /// </summary>
  public int StartLine => _startLine;

  /// <summary>
  /// Количество строк в тексте правил.
  /// </summary>
  public int TotalLines => _lines.Length;

  /// <summary>
  /// Запускает цикл обработки ввода на экране правил.
  /// </summary>
  public void Run()
  {
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
  /// Прокручивает текст на указанное число строк.
  /// </summary>
  /// <param name="parDelta">Смещение по строкам.</param>
  public void ScrollLines(int parDelta)
  {
    _startLine = ClampStartLine(_startLine + parDelta);
    Render();
  }

  /// <summary>
  /// Прокручивает текст на страницу.
  /// </summary>
  /// <param name="parDirection">Направление прокрутки.</param>
  public void ScrollPage(int parDirection)
  {
    int pageSize = Math.Max(1, _view.PageSize);
    _startLine = ClampStartLine(_startLine + (pageSize * parDirection));
    Render();
  }

  /// <summary>
  /// Выполняет ClampStartLine.
  /// </summary>
  private int ClampStartLine(int parValue)
  {
    int maxStart = Math.Max(0, _lines.Length - Math.Max(1, _view.PageSize));
    return Math.Clamp(parValue, 0, maxStart);
  }

  /// <summary>
  /// Выполняет Render.
  /// </summary>
  private void Render()
  {
    _view.Render(_lines, _startLine);
  }
}
