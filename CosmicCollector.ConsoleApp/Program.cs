using CosmicCollector.ConsoleApp.Controllers;
using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;
using CosmicCollector.Persistence.Records;
using System.Text;

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
    Console.OutputEncoding = Encoding.UTF8;

    IConsoleRenderer renderer = new ConsoleRenderer();
    IConsoleInputReader inputReader = new ConsoleInputReader();
    IRulesTextProvider rulesTextProvider = new FileRulesTextProvider("rules/rules-text.md");
    IGameSessionFactory gameSessionFactory = new GameSessionFactory();
    IRecordsRepository recordsRepository = new RecordsRepository(
      Path.Combine(AppContext.BaseDirectory, "records.json"));

    IMainMenuView view = new MainMenuView(renderer);
    MainMenuController controller = new MainMenuController(
      view,
      inputReader,
      renderer,
      rulesTextProvider,
      gameSessionFactory,
      recordsRepository);

    controller.Run();
  }
}
