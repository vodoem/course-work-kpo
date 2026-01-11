using System.Windows.Input;
using CosmicCollector.Avalonia.Commands;
using CosmicCollector.Avalonia.Navigation;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// View model for the game over screen placeholder.
/// </summary>
public sealed class GameOverViewModel : ViewModelBase
{
  /// <summary>
  /// Initializes a new instance of the <see cref="GameOverViewModel"/> class.
  /// </summary>
  /// <param name="parRestartNavigation">Navigation to restart the game.</param>
  /// <param name="parBackNavigation">Navigation back to main menu.</param>
  public GameOverViewModel(NavigationService parRestartNavigation, NavigationService parBackNavigation)
  {
    RestartCommand = new NavigateCommand(parRestartNavigation);
    BackToMenuCommand = new NavigateCommand(parBackNavigation);
    SaveRecordCommand = new DelegateCommand(() => { });
  }

  /// <summary>
  /// Gets the command to restart the game.
  /// </summary>
  public ICommand RestartCommand { get; }

  /// <summary>
  /// Gets the command to return to the main menu.
  /// </summary>
  public ICommand BackToMenuCommand { get; }

  /// <summary>
  /// Gets the command to save a record (placeholder).
  /// </summary>
  public ICommand SaveRecordCommand { get; }
}
