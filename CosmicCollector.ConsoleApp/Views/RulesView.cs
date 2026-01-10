using CosmicCollector.ConsoleApp.Infrastructure;

namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Представление экрана правил.
/// </summary>
public sealed class RulesView : IRulesView
{
  private readonly IConsoleRenderer _renderer;

  /// <summary>
  /// Создаёт представление правил.
  /// </summary>
  /// <param name="parRenderer">Рендерер консоли.</param>
  public RulesView(IConsoleRenderer parRenderer)
  {
    _renderer = parRenderer;
  }

  /// <inheritdoc />
  public void Render(string[] parLines, int parStartLine)
  {
    _renderer.Clear();
    _renderer.WriteLine("ПРАВИЛА");
    _renderer.WriteLine(string.Empty);

    int availableHeight = PageSize;
    int startLine = Math.Clamp(parStartLine, 0, Math.Max(0, parLines.Length - availableHeight));
    int endLine = Math.Min(parLines.Length, startLine + availableHeight);

    for (int i = startLine; i < endLine; i++)
    {
      _renderer.WriteLine(parLines[i]);
    }

    for (int i = endLine; i < startLine + availableHeight; i++)
    {
      _renderer.WriteLine(string.Empty);
    }

    _renderer.WriteLine(string.Empty);
    _renderer.WriteLine("Esc — назад в меню");
  }

  /// <inheritdoc />
  public int PageSize => Math.Max(1, _renderer.BufferHeight - 4);
}
