using System;
using System.Windows.Input;
using CosmicCollector.Avalonia.Navigation;

namespace CosmicCollector.Avalonia.Commands;

/// <summary>
/// Command that triggers navigation.
/// </summary>
public sealed class NavigateCommand : ICommand
{
  private readonly NavigationService _navigationService;

  /// <summary>
  /// Initializes a new instance of the <see cref="NavigateCommand"/> class.
  /// </summary>
  /// <param name="parNavigationService">Navigation service instance.</param>
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
