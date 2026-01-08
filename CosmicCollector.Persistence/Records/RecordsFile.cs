using System.Text.Json.Serialization;

namespace CosmicCollector.Persistence.Records;

/// <summary>
/// Представляет файл рекордов.
/// </summary>
public sealed class RecordsFile
{
  /// <summary>
  /// Версия формата.
  /// </summary>
  [JsonPropertyName("formatVersion")]
  public int FormatVersion { get; init; } = 1;

  /// <summary>
  /// Список рекордов.
  /// </summary>
  [JsonPropertyName("records")]
  public List<RecordEntry> Records { get; init; } = new();
}
