using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;

namespace CosmicCollector.ConsoleApp.Controllers;

/// <summary>
/// Контроллер главного меню.
/// </summary>
public sealed class MainMenuController
{
  private readonly IMainMenuView _view;

  /// <summary>
  /// Создаёт контроллер главного меню.
  /// </summary>
  /// <param name="parView">Представление главного меню.</param>
  public MainMenuController(IMainMenuView parView)
  {
    _view = parView;
  }

  /// <summary>
  /// Запускает цикл обработки ввода в главном меню.
  /// </summary>
  public void Run()
  {
    int selectedIndex = 0;
    _view.Render(selectedIndex);

    while (true)
    {
      ConsoleKeyInfo keyInfo = Console.ReadKey(true);

      if (keyInfo.Key == ConsoleKey.UpArrow)
      {
        selectedIndex = (selectedIndex - 1 + MenuItem.Items.Length) % MenuItem.Items.Length;
        _view.Render(selectedIndex);
        continue;
      }

      if (keyInfo.Key == ConsoleKey.DownArrow)
      {
        selectedIndex = (selectedIndex + 1) % MenuItem.Items.Length;
        _view.Render(selectedIndex);
        continue;
      }

      if (keyInfo.Key == ConsoleKey.Enter)
      {
        MenuItem selectedItem = MenuItem.Items[selectedIndex];

        if (selectedItem.Kind == MenuItemKind.Exit)
        {
          Console.Clear();
          return;
        }

        _view.ShowNotImplemented(selectedItem.Title);
        _view.Render(selectedIndex);
      }
    }
  }
}
