namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Контракт представления экрана правил.
/// </summary>
public interface IRulesView
{
  /// <summary>
  /// Отрисовывает правила с текущим смещением.
  /// </summary>
  /// <param name="parLines">Все строки правил.</param>
  /// <param name="parStartLine">Индекс первой отображаемой строки.</param>
  void Render(string[] parLines, int parStartLine);

  /// <summary>
  /// Количество строк, помещающихся на экране.
  /// </summary>
  int PageSize { get; }
}
