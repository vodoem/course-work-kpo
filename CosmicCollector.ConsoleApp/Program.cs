using CosmicCollector.ConsoleApp.Controllers;
using CosmicCollector.ConsoleApp.Views;

namespace CosmicCollector.ConsoleApp;

/// <summary>
/// Точка входа консольного приложения.
/// </summary>
public static class Program
{
  /// <summary>
  /// Запуск приложения.
  /// </summary>
  public static void Main()
  {
    Console.CursorVisible = false;

    IMainMenuView view = new MainMenuView();
    MainMenuController controller = new MainMenuController(view);

    controller.Run();
  }
}
