using System;
using CosmicCollector.Avalonia.ViewModels;

namespace CosmicCollector.Avalonia.Navigation;

/// <summary>
/// Сервис переключения текущей ViewModel.
/// </summary>
public sealed class NavigationService
{
  private readonly NavigationStore _navigationStore;
  private readonly Func<ViewModelBase> _viewModelFactory;

  /// <summary>
  /// Инициализирует новый экземпляр <see cref="NavigationService"/>.
  /// </summary>
  /// <param name="parNavigationStore">Хранилище навигации.</param>
  /// <param name="parViewModelFactory">Фабрика ViewModel.</param>
  public NavigationService(NavigationStore parNavigationStore, Func<ViewModelBase> parViewModelFactory)
  {
    _navigationStore = parNavigationStore;
    _viewModelFactory = parViewModelFactory;
  }

  /// <summary>
  /// Выполняет навигацию на новую ViewModel.
  /// </summary>
  public void Navigate()
  {
    _navigationStore.CurrentViewModel = _viewModelFactory();
  }
}
