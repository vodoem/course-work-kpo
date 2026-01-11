using System.Windows.Input;
using CosmicCollector.Avalonia.Commands;
using CosmicCollector.Avalonia.Navigation;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// View model for the game screen placeholder.
/// </summary>
public sealed class GameViewModel : ViewModelBase
{
  /// <summary>
  /// Initializes a new instance of the <see cref="GameViewModel"/> class.
  /// </summary>
  /// <param name="parBackNavigation">Navigation back to main menu.</param>
  /// <param name="parFinishNavigation">Navigation to the game over screen.</param>
  public GameViewModel(NavigationService parBackNavigation, NavigationService parFinishNavigation)
  {
    BackToMenuCommand = new NavigateCommand(parBackNavigation);
    FinishGameCommand = new NavigateCommand(parFinishNavigation);
  }

  /// <summary>
  /// Gets the command to return to the main menu.
  /// </summary>
  public ICommand BackToMenuCommand { get; }

  /// <summary>
  /// Gets the command to open the game over screen placeholder.
  /// </summary>
  public ICommand FinishGameCommand { get; }
}
