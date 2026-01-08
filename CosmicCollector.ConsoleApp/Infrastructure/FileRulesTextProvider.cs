namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Читает текст правил из файла.
/// </summary>
public sealed class FileRulesTextProvider : IRulesTextProvider
{
  private const string RulesRelativePath = "rules/rules-text.md";
  private static readonly string[] FallbackLines =
  {
    "Файл правил не найден.",
    "Проверьте наличие rules/rules-text.md рядом с приложением.",
    "Возврат в меню доступен по Esc."
  };

  /// <summary>
  /// Создаёт поставщик текста правил.
  /// </summary>
  /// <param name="parFilePath">Путь к файлу с правилами.</param>
  public FileRulesTextProvider(string parFilePath)
  {
  }

  /// <inheritdoc />
  public string[] GetLines()
  {
    string filePath = Path.Combine(AppContext.BaseDirectory, RulesRelativePath);

    if (!File.Exists(filePath))
    {
      return FallbackLines;
    }

    return File.ReadAllLines(filePath);
  }
}
