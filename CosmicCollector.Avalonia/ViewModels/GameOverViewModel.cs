using System.Windows.Input;
using CosmicCollector.Avalonia.Commands;
using CosmicCollector.Avalonia.Navigation;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// ViewModel заглушки экрана завершения игры.
/// </summary>
public sealed class GameOverViewModel : ViewModelBase
{
  /// <summary>
  /// Инициализирует новый экземпляр <see cref="GameOverViewModel"/>.
  /// </summary>
  /// <param name="parRestartNavigation">Навигация для перезапуска игры.</param>
  /// <param name="parBackNavigation">Навигация назад в главное меню.</param>
  public GameOverViewModel(NavigationService parRestartNavigation, NavigationService parBackNavigation)
  {
    RestartCommand = new NavigateCommand(parRestartNavigation);
    BackToMenuCommand = new NavigateCommand(parBackNavigation);
    SaveRecordCommand = new DelegateCommand(() => { });
  }

  /// <summary>
  /// Команда перезапуска игры.
  /// </summary>
  public ICommand RestartCommand { get; }

  /// <summary>
  /// Команда возврата в главное меню.
  /// </summary>
  public ICommand BackToMenuCommand { get; }

  /// <summary>
  /// Команда сохранения рекорда (заглушка).
  /// </summary>
  public ICommand SaveRecordCommand { get; }
}
