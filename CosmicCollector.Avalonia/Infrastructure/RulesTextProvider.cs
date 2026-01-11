using System;
using System.Collections.Generic;
using System.IO;
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

    var controlsLines = controlStartIndex == -1
      ? Array.Empty<string>()
      : ExtractSection(lines, controlStartIndex + 1, controlEndIndex);

    var descriptionLines = new List<string>();

    for (var i = 0; i < lines.Length; i++)
    {
      if (controlStartIndex != -1 && i >= controlStartIndex && i < controlEndIndex)
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
    var normalized = new List<string>();

    foreach (var line in parLines)
    {
      var value = IsHeading(line) ? StripHeading(line) : line;
      normalized.Add(value);
    }

    return string.Join(Environment.NewLine, normalized).Trim();
  }

  private static bool IsHeading(string parLine)
  {
    return parLine.TrimStart().StartsWith("#", StringComparison.Ordinal);
  }

  private static string StripHeading(string parLine)
  {
    var trimmed = parLine.TrimStart();
    var index = 0;

    while (index < trimmed.Length && trimmed[index] == '#')
    {
      index++;
    }

    if (index < trimmed.Length)
    {
      trimmed = trimmed[index..];
    }

    return trimmed.TrimStart();
  }
}
