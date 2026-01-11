using System;
using CosmicCollector.Avalonia.ViewModels;

namespace CosmicCollector.Avalonia.Navigation;

/// <summary>
/// Service for switching current view model.
/// </summary>
public sealed class NavigationService
{
  private readonly NavigationStore _navigationStore;
  private readonly Func<ViewModelBase> _viewModelFactory;

  /// <summary>
  /// Initializes a new instance of the <see cref="NavigationService"/> class.
  /// </summary>
  /// <param name="parNavigationStore">Navigation store instance.</param>
  /// <param name="parViewModelFactory">Factory for new view model.</param>
  public NavigationService(NavigationStore parNavigationStore, Func<ViewModelBase> parViewModelFactory)
  {
    _navigationStore = parNavigationStore;
    _viewModelFactory = parViewModelFactory;
  }

  /// <summary>
  /// Navigates to a new view model.
  /// </summary>
  public void Navigate()
  {
    _navigationStore.CurrentViewModel = _viewModelFactory();
  }
}
