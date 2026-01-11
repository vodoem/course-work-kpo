using System;
using System.IO;

namespace CosmicCollector.Avalonia.Infrastructure;

/// <summary>
/// Определяет пути к файлам данных приложения.
/// </summary>
internal static class AppDataPaths
{
  /// <summary>
  /// Возвращает путь к файлу рекордов.
  /// </summary>
  /// <returns>Путь к records.json.</returns>
  public static string GetRecordsFilePath()
  {
    return FindPath("records.json");
  }

  /// <summary>
  /// Возвращает путь к файлу правил.
  /// </summary>
  /// <returns>Путь к rules-text.md.</returns>
  public static string GetRulesFilePath()
  {
    return FindPath(Path.Combine("rules", "rules-text.md"));
  }

  private static string FindPath(string parRelativePath)
  {
    var directory = new DirectoryInfo(AppContext.BaseDirectory);

    while (directory is not null)
    {
      var candidate = Path.Combine(directory.FullName, parRelativePath);

      if (File.Exists(candidate))
      {
        return candidate;
      }

      directory = directory.Parent;
    }

    return Path.Combine(AppContext.BaseDirectory, parRelativePath);
  }
}
