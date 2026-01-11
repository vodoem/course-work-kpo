using CosmicCollector.Avalonia.Navigation;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// ViewModel главного окна.
/// </summary>
public sealed class MainWindowViewModel : ViewModelBase
{
  private readonly NavigationStore _navigationStore;

  /// <summary>
  /// Инициализирует новый экземпляр <see cref="MainWindowViewModel"/>.
  /// </summary>
  /// <param name="parNavigationStore">Хранилище навигации.</param>
  public MainWindowViewModel(NavigationStore parNavigationStore)
  {
    _navigationStore = parNavigationStore;
    _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
  }

  /// <summary>
  /// Текущая ViewModel, отображаемая в окне.
  /// </summary>
  public ViewModelBase? CurrentViewModel => _navigationStore.CurrentViewModel;

  private void OnCurrentViewModelChanged()
  {
    OnPropertyChanged(nameof(CurrentViewModel));
  }
}
