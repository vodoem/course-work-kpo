using System;
using System.Windows.Input;
using CosmicCollector.Avalonia.Commands;
using CosmicCollector.Avalonia.Navigation;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// ViewModel главного меню.
/// </summary>
public sealed class MainMenuViewModel : ViewModelBase
{
  /// <summary>
  /// Инициализирует новый экземпляр <see cref="MainMenuViewModel"/>.
  /// </summary>
  /// <param name="parExitAction">Действие выхода из приложения.</param>
  /// <param name="parGameNavigation">Навигация на экран игры.</param>
  /// <param name="parRulesNavigation">Навигация на экран правил.</param>
  /// <param name="parRecordsNavigation">Навигация на экран рекордов.</param>
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
  /// Команда запуска игры.
  /// </summary>
  public ICommand StartGameCommand { get; }

  /// <summary>
  /// Команда открытия правил.
  /// </summary>
  public ICommand ShowRulesCommand { get; }

  /// <summary>
  /// Команда открытия рекордов.
  /// </summary>
  public ICommand ShowRecordsCommand { get; }

  /// <summary>
  /// Команда выхода из приложения.
  /// </summary>
  public ICommand ExitCommand { get; }
}
