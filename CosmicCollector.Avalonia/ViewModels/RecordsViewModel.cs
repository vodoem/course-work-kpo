using System.Windows.Input;
using CosmicCollector.Avalonia.Commands;
using CosmicCollector.Avalonia.Navigation;

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
    BackCommand = new NavigateCommand(parBackNavigation);
  }

  /// <summary>
  /// Команда возврата в главное меню.
  /// </summary>
  public ICommand BackCommand { get; }
}
