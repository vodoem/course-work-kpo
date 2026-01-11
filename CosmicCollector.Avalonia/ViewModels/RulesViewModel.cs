using System.Windows.Input;
using CosmicCollector.Avalonia.Commands;
using CosmicCollector.Avalonia.Infrastructure;
using CosmicCollector.Avalonia.Navigation;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// ViewModel экрана правил.
/// </summary>
public sealed class RulesViewModel : ViewModelBase
{
  /// <summary>
  /// Инициализирует новый экземпляр <see cref="RulesViewModel"/>.
  /// </summary>
  /// <param name="parBackNavigation">Навигация назад в главное меню.</param>
  public RulesViewModel(NavigationService parBackNavigation)
  {
    var sections = RulesTextProvider.LoadSections();
    DescriptionText = sections.Description;
    ControlsText = sections.Controls;
    BackCommand = new NavigateCommand(parBackNavigation);
  }

  /// <summary>
  /// Текст описания игры и правил.
  /// </summary>
  public string DescriptionText { get; }

  /// <summary>
  /// Текст управления.
  /// </summary>
  public string ControlsText { get; }

  /// <summary>
  /// Команда возврата в главное меню.
  /// </summary>
  public ICommand BackCommand { get; }
}
