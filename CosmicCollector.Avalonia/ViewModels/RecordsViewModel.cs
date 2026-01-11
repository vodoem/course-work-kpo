using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CosmicCollector.Avalonia.Commands;
using CosmicCollector.Avalonia.Infrastructure;
using CosmicCollector.Avalonia.Navigation;
using CosmicCollector.Persistence.Records;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// ViewModel экрана рекордов.
/// </summary>
public sealed class RecordsViewModel : ViewModelBase
{
  /// <summary>
  /// Инициализирует новый экземпляр <see cref="RecordsViewModel"/>.
  /// </summary>
  /// <param name="parBackNavigation">Навигация назад в главное меню.</param>
  public RecordsViewModel(NavigationService parBackNavigation)
  {
    var recordsRepository = new RecordsRepository(AppDataPaths.GetRecordsFilePath());
    Records = BuildRecordsList(recordsRepository.LoadAll());
    BackCommand = new NavigateCommand(parBackNavigation);
  }

  /// <summary>
  /// Коллекция строк таблицы рекордов.
  /// </summary>
  public IReadOnlyList<RecordsRowViewModel> Records { get; }

  /// <summary>
  /// Команда возврата в главное меню.
  /// </summary>
  public ICommand BackCommand { get; }

  private static IReadOnlyList<RecordsRowViewModel> BuildRecordsList(IReadOnlyList<RecordEntry> parRecords)
  {
    return parRecords
      .OrderByDescending(record => record.parScore)
      .ThenByDescending(record => record.parLevel)
      .Select((record, index) => new RecordsRowViewModel(
        index + 1,
        record.parPlayerName,
        record.parScore,
        record.parLevel))
      .ToList();
  }
}
