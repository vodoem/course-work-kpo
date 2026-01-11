using System;
using System.Windows.Input;
using CosmicCollector.Avalonia.Navigation;

namespace CosmicCollector.Avalonia.Commands;

/// <summary>
/// Команда для перехода на другой экран.
/// </summary>
public sealed class NavigateCommand : ICommand
{
  private readonly NavigationService _navigationService;

  /// <summary>
  /// Инициализирует новый экземпляр <see cref="NavigateCommand"/>.
  /// </summary>
  /// <param name="parNavigationService">Сервис навигации.</param>
  public NavigateCommand(NavigationService parNavigationService)
  {
    _navigationService = parNavigationService;
  }

  /// <inheritdoc />
  public event EventHandler? CanExecuteChanged;

  /// <inheritdoc />
  public bool CanExecute(object? parParameter)
  {
    return true;
  }

  /// <inheritdoc />
  public void Execute(object? parParameter)
  {
    _navigationService.Navigate();
  }
}
