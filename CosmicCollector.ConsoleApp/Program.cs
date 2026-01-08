using CosmicCollector.ConsoleApp.Controllers;
using CosmicCollector.ConsoleApp.Infrastructure;
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

    IConsoleRenderer renderer = new ConsoleRenderer();
    IConsoleInputReader inputReader = new ConsoleInputReader();
    IRulesTextProvider rulesTextProvider = new FileRulesTextProvider("rules/rules-text.md");

    IMainMenuView view = new MainMenuView(renderer);
    MainMenuController controller = new MainMenuController(
      view,
      inputReader,
      renderer,
      rulesTextProvider);

    controller.Run();
  }
}
