using System.Windows.Input;
using CosmicCollector.Avalonia.Commands;
using CosmicCollector.Avalonia.Navigation;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// View model for the rules screen.
/// </summary>
public sealed class RulesViewModel : ViewModelBase
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RulesViewModel"/> class.
  /// </summary>
  /// <param name="parBackNavigation">Navigation back to main menu.</param>
  public RulesViewModel(NavigationService parBackNavigation)
  {
    BackCommand = new NavigateCommand(parBackNavigation);
  }

  /// <summary>
  /// Gets the command to return to the main menu.
  /// </summary>
  public ICommand BackCommand { get; }
}
