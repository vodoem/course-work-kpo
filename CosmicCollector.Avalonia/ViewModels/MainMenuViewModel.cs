using System;
using System.Windows.Input;
using CosmicCollector.Avalonia.Commands;
using CosmicCollector.Avalonia.Navigation;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// View model for the main menu.
/// </summary>
public sealed class MainMenuViewModel : ViewModelBase
{
  /// <summary>
  /// Initializes a new instance of the <see cref="MainMenuViewModel"/> class.
  /// </summary>
  /// <param name="parExitAction">Action that closes the application.</param>
  /// <param name="parGameNavigation">Navigation to the game screen.</param>
  /// <param name="parRulesNavigation">Navigation to the rules screen.</param>
  /// <param name="parRecordsNavigation">Navigation to the records screen.</param>
  public MainMenuViewModel(
    Action parExitAction,
    NavigationService parGameNavigation,
    NavigationService parRulesNavigation,
    NavigationService parRecordsNavigation)
  {
    StartGameCommand = new NavigateCommand(parGameNavigation);
    ShowRulesCommand = new NavigateCommand(parRulesNavigation);
    ShowRecordsCommand = new NavigateCommand(parRecordsNavigation);
    ExitCommand = new DelegateCommand(parExitAction);
  }

  /// <summary>
  /// Gets the command to start the game.
  /// </summary>
  public ICommand StartGameCommand { get; }

  /// <summary>
  /// Gets the command to show rules.
  /// </summary>
  public ICommand ShowRulesCommand { get; }

  /// <summary>
  /// Gets the command to show records.
  /// </summary>
  public ICommand ShowRecordsCommand { get; }

  /// <summary>
  /// Gets the command to exit the application.
  /// </summary>
  public ICommand ExitCommand { get; }
}
