using System.Text;
using System.Text.Json;
using System.Linq;

namespace CosmicCollector.Persistence.Records;

/// <summary>
/// Реализует хранение рекордов в JSON-файле.
/// </summary>
public sealed class RecordsRepository : IRecordsRepository
{
  private readonly string _filePath;
  private readonly JsonSerializerOptions _options = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
  };

  /// <summary>
  /// Инициализирует репозиторий.
  /// </summary>
  /// <param name="parFilePath">Путь к файлу records.json.</param>
  public RecordsRepository(string parFilePath)
  {
    _filePath = parFilePath;
  }

  /// <inheritdoc />
  public IReadOnlyList<RecordEntry> LoadAll()
  {
    if (!File.Exists(_filePath))
    {
      return new List<RecordEntry>();
    }

    try
    {
      var json = File.ReadAllText(_filePath, Encoding.UTF8);
      var data = JsonSerializer.Deserialize<RecordsFile>(json, _options);
      return data?.Records ?? new List<RecordEntry>();
    }
    catch (JsonException)
    {
      return new List<RecordEntry>();
    }
    catch (IOException)
    {
      return new List<RecordEntry>();
    }
  }

  /// <inheritdoc />
  public void SaveAll(IReadOnlyList<RecordEntry> parRecords)
  {
    var directory = Path.GetDirectoryName(_filePath);

    if (!string.IsNullOrWhiteSpace(directory))
    {
      Directory.CreateDirectory(directory);
    }

    var file = new RecordsFile
    {
      Records = parRecords.ToList()
    };
    var json = JsonSerializer.Serialize(file, _options);
    File.WriteAllText(_filePath, json, Encoding.UTF8);
  }

  /// <inheritdoc />
  public void Add(RecordEntry parRecord)
  {
    var records = LoadAll().ToList();
    records.Add(parRecord);
    SaveAll(records);
  }
}
