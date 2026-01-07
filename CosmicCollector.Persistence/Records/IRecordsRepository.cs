namespace CosmicCollector.Persistence.Records;

/// <summary>
/// Предоставляет доступ к сохранённым рекордам.
/// </summary>
public interface IRecordsRepository
{
  /// <summary>
  /// Загружает все записи рекордов.
  /// </summary>
  /// <returns>Список записей.</returns>
  IReadOnlyList<RecordEntry> LoadAll();

  /// <summary>
  /// Сохраняет список рекордов.
  /// </summary>
  /// <param name="parRecords">Записи для сохранения.</param>
  void SaveAll(IReadOnlyList<RecordEntry> parRecords);

  /// <summary>
  /// Добавляет запись и сохраняет файл.
  /// </summary>
  /// <param name="parRecord">Новая запись.</param>
  void Add(RecordEntry parRecord);
}
