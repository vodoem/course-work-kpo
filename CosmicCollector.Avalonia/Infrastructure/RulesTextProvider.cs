using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CosmicCollector.Avalonia.Infrastructure;

/// <summary>
/// Загружает и подготавливает текст правил для экранов UI.
/// </summary>
internal static class RulesTextProvider
{
  /// <summary>
  /// Загружает описания правил и управления.
  /// </summary>
  /// <returns>Кортеж с описанием правил и управлением.</returns>
  public static (string Description, string Controls) LoadSections()
  {
    var path = AppDataPaths.GetRulesFilePath();

    if (!File.Exists(path))
    {
      return ("Правила недоступны.", string.Empty);
    }

    var lines = File.ReadAllLines(path, Encoding.UTF8);
    var controlStartIndex = FindHeadingIndex(lines, "## Управление");
    var controlEndIndex = controlStartIndex == -1
      ? -1
      : FindNextHeadingIndex(lines, controlStartIndex + 1);

    if (controlStartIndex != -1 && controlEndIndex == -1)
    {
      controlEndIndex = lines.Length;
    }

    if (controlStartIndex == -1)
    {
      var fullText = NormalizeLines(RemoveFirstHeading(lines));
      return (fullText, "Раздел \"Управление\" не найден.");
    }

    var controlsLines = ExtractSection(lines, controlStartIndex + 1, controlEndIndex);
    var descriptionLines = new List<string>();

    for (var i = 0; i < lines.Length; i++)
    {
      if (i >= controlStartIndex && i < controlEndIndex)
      {
        continue;
      }

      if (i == 0 && IsHeading(lines[i]))
      {
        continue;
      }

      descriptionLines.Add(lines[i]);
    }

    var description = NormalizeLines(descriptionLines);
    var controls = NormalizeLines(controlsLines);

    return (description, controls);
  }

  private static int FindHeadingIndex(string[] parLines, string parHeading)
  {
    for (var i = 0; i < parLines.Length; i++)
    {
      if (string.Equals(parLines[i].Trim(), parHeading, StringComparison.OrdinalIgnoreCase))
      {
        return i;
      }
    }

    return -1;
  }

  private static int FindNextHeadingIndex(string[] parLines, int parStartIndex)
  {
    for (var i = parStartIndex; i < parLines.Length; i++)
    {
      if (IsHeading(parLines[i]))
      {
        return i;
      }
    }

    return -1;
  }

  private static IReadOnlyList<string> ExtractSection(string[] parLines, int parStartIndex, int parEndIndex)
  {
    var extracted = new List<string>();

    for (var i = parStartIndex; i < parEndIndex; i++)
    {
      extracted.Add(parLines[i]);
    }

    return extracted;
  }

  private static string NormalizeLines(IReadOnlyList<string> parLines)
  {
    return string.Join(Environment.NewLine, parLines).Trim();
  }

  private static bool IsHeading(string parLine)
  {
    return parLine.TrimStart().StartsWith("#", StringComparison.Ordinal);
  }

  private static IReadOnlyList<string> RemoveFirstHeading(IReadOnlyList<string> parLines)
  {
    if (parLines.Count == 0)
    {
      return parLines;
    }

    return IsHeading(parLines[0]) ? parLines.Skip(1).ToArray() : parLines;
  }
}
