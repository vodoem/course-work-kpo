namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Читает текст правил из файла.
/// </summary>
public sealed class FileRulesTextProvider : IRulesTextProvider
{
  private readonly string _filePath;

  /// <summary>
  /// Создаёт поставщик текста правил.
  /// </summary>
  /// <param name="parFilePath">Путь к файлу с правилами.</param>
  public FileRulesTextProvider(string parFilePath)
  {
    _filePath = parFilePath;
  }

  /// <inheritdoc />
  public string[] GetLines()
  {
    return File.ReadAllLines(_filePath);
  }
}
