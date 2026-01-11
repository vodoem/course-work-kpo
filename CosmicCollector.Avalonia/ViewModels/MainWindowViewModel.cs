using CosmicCollector.Avalonia.Navigation;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// View model for the main window.
/// </summary>
public sealed class MainWindowViewModel : ViewModelBase
{
  private readonly NavigationStore _navigationStore;

  /// <summary>
  /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
  /// </summary>
  /// <param name="parNavigationStore">Navigation store instance.</param>
  public MainWindowViewModel(NavigationStore parNavigationStore)
  {
    _navigationStore = parNavigationStore;
    _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
  }

  /// <summary>
  /// Gets the current view model.
  /// </summary>
  public ViewModelBase? CurrentViewModel => _navigationStore.CurrentViewModel;

  private void OnCurrentViewModelChanged()
  {
    OnPropertyChanged(nameof(CurrentViewModel));
  }
}
