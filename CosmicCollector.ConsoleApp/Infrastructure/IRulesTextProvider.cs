namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Контракт получения текста правил.
/// </summary>
public interface IRulesTextProvider
{
  /// <summary>
  /// Возвращает текст правил построчно.
  /// </summary>
  /// <returns>Массив строк правил.</returns>
  string[] GetLines();
}
