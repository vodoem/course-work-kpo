using CosmicCollector.Persistence.Records;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет работу репозитория рекордов.
/// </summary>
public sealed class RecordsRepositoryTests
{
  /// <summary>
  /// Проверяет сохранение и загрузку рекордов.
  /// </summary>
  [Xunit.Fact]
  public void SaveAndLoad_RoundTrip()
  {
    var directory = CreateTempDirectory();
    var filePath = Path.Combine(directory, "records.json");
    var repository = new RecordsRepository(filePath);
    var records = new List<RecordEntry>
    {
      new("Ada", 120, 3, "2025-01-01T00:00:00Z")
    };

    try
    {
      repository.SaveAll(records);
      var loaded = repository.LoadAll();

      Xunit.Assert.Single(loaded);
      Xunit.Assert.Equal(records[0], loaded[0]);
    }
    finally
    {
      Directory.Delete(directory, true);
    }
  }

  /// <summary>
  /// Проверяет, что отсутствие файла возвращает пустой список.
  /// </summary>
  [Xunit.Fact]
  public void LoadAll_MissingFile_ReturnsEmpty()
  {
    var directory = CreateTempDirectory();
    var filePath = Path.Combine(directory, "records.json");
    var repository = new RecordsRepository(filePath);

    try
    {
      var loaded = repository.LoadAll();

      Xunit.Assert.Empty(loaded);
    }
    finally
    {
      Directory.Delete(directory, true);
    }
  }

  /// <summary>
  /// Проверяет, что неверный JSON не вызывает исключение.
  /// </summary>
  [Xunit.Fact]
  public void LoadAll_InvalidJson_ReturnsEmpty()
  {
    var directory = CreateTempDirectory();
    var filePath = Path.Combine(directory, "records.json");
    var repository = new RecordsRepository(filePath);

    try
    {
      File.WriteAllText(filePath, "not-json");

      var loaded = repository.LoadAll();

      Xunit.Assert.Empty(loaded);
    }
    finally
    {
      Directory.Delete(directory, true);
    }
  }

  private static string CreateTempDirectory()
  {
    var directory = Path.Combine(Path.GetTempPath(), "cc-tests", Guid.NewGuid().ToString("N"));
    Directory.CreateDirectory(directory);
    return directory;
  }
}
