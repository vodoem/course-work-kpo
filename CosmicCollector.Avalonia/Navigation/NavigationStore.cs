using System;
using CosmicCollector.Avalonia.ViewModels;

namespace CosmicCollector.Avalonia.Navigation;

/// <summary>
/// Stores current view model for navigation.
/// </summary>
public sealed class NavigationStore
{
  private ViewModelBase? _currentViewModel;

  /// <summary>
  /// Occurs when the current view model changes.
  /// </summary>
  public event Action? CurrentViewModelChanged;

  /// <summary>
  /// Gets or sets the current view model.
  /// </summary>
  public ViewModelBase? CurrentViewModel
  {
    get => _currentViewModel;
    set
    {
      if (_currentViewModel == value)
      {
        return;
      }

      _currentViewModel = value;
      CurrentViewModelChanged?.Invoke();
    }
  }
}
