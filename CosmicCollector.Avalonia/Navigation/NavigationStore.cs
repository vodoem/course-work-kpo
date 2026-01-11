using System;
using CosmicCollector.Avalonia.ViewModels;

namespace CosmicCollector.Avalonia.Navigation;

/// <summary>
/// Хранилище текущей ViewModel для навигации.
/// </summary>
public sealed class NavigationStore
{
  private ViewModelBase? _currentViewModel;

  /// <summary>
  /// Событие изменения текущей ViewModel.
  /// </summary>
  public event Action? CurrentViewModelChanged;

  /// <summary>
  /// Текущая ViewModel.
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
