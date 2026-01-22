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

  /// <summary>
  /// Событие команды не используется: доступность всегда true.
  /// </summary>
  public event EventHandler? CanExecuteChanged
  {
    add
    {
    }
    remove
    {
    }
  }

  /// <summary>
  /// Всегда разрешает выполнение команды.
  /// </summary>
  /// <param name="parParameter">Параметр команды (не используется).</param>
  /// <returns>true.</returns>
  public bool CanExecute(object? parParameter)
  {
    return true;
  }

  /// <summary>
  /// Выполняет переход через <see cref="NavigationService"/>.
  /// </summary>
  /// <param name="parParameter">Параметр команды (не используется).</param>
  public void Execute(object? parParameter)
  {
    _navigationService.Navigate();
  }
}
