using System.Windows.Input;
using CosmicCollector.Avalonia.Commands;
using CosmicCollector.Avalonia.Navigation;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// ViewModel заглушки игрового экрана.
/// </summary>
public sealed class GameViewModel : ViewModelBase
{
  /// <summary>
  /// Инициализирует новый экземпляр <see cref="GameViewModel"/>.
  /// </summary>
  /// <param name="parBackNavigation">Навигация назад в главное меню.</param>
  /// <param name="parFinishNavigation">Навигация на экран завершения.</param>
  public GameViewModel(NavigationService parBackNavigation, NavigationService parFinishNavigation)
  {
    BackToMenuCommand = new NavigateCommand(parBackNavigation);
    FinishGameCommand = new NavigateCommand(parFinishNavigation);
  }

  /// <summary>
  /// Команда возврата в главное меню.
  /// </summary>
  public ICommand BackToMenuCommand { get; }

  /// <summary>
  /// Команда перехода на экран завершения.
  /// </summary>
  public ICommand FinishGameCommand { get; }
}
