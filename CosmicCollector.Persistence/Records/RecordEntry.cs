using System.Text.Json.Serialization;

namespace CosmicCollector.Persistence.Records;

/// <summary>
/// Описывает запись таблицы рекордов.
/// </summary>
public sealed record RecordEntry(
  [property: JsonPropertyName("playerName")] string parPlayerName,
  [property: JsonPropertyName("score")] int parScore,
  [property: JsonPropertyName("level")] int parLevel,
  [property: JsonPropertyName("timestampUtc")] string parTimestampUtc);
