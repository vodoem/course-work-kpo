using CosmicCollector.Persistence.Records;

namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Контракт представления экрана рекордов.
/// </summary>
public interface IRecordsView
{
  /// <summary>
  /// Отрисовывает таблицу рекордов.
  /// </summary>
  /// <param name="parRecords">Список рекордов.</param>
  /// <param name="parStartIndex">Индекс первой отображаемой записи.</param>
  void Render(IReadOnlyList<RecordEntry> parRecords, int parStartIndex);

  /// <summary>
  /// Количество строк, помещающихся на экране.
  /// </summary>
  int PageSize { get; }
}
