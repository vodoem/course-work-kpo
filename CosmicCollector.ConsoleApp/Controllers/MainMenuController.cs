using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;
using CosmicCollector.Persistence.Records;

namespace CosmicCollector.ConsoleApp.Controllers;

/// <summary>
/// Контроллер главного меню.
/// </summary>
public sealed class MainMenuController
{
  private readonly IMainMenuView _view;
  private readonly IConsoleInputReader _inputReader;
  private readonly IConsoleRenderer _renderer;
  private readonly IRulesTextProvider _rulesTextProvider;
  private readonly IGameSessionFactory _gameSessionFactory;
  private readonly IRecordsRepository _recordsRepository;

  /// <summary>
  /// Создаёт контроллер главного меню.
  /// </summary>
  /// <param name="parView">Представление главного меню.</param>
  /// <param name="parInputReader">Читатель ввода.</param>
  /// <param name="parRenderer">Рендерер консоли.</param>
  /// <param name="parRulesTextProvider">Поставщик текста правил.</param>
  /// <param name="parGameSessionFactory">Фабрика игровых сессий.</param>
  /// <param name="parRecordsRepository">Репозиторий рекордов.</param>
  public MainMenuController(
    IMainMenuView parView,
    IConsoleInputReader parInputReader,
    IConsoleRenderer parRenderer,
    IRulesTextProvider parRulesTextProvider,
    IGameSessionFactory parGameSessionFactory,
    IRecordsRepository parRecordsRepository)
  {
    _view = parView;
    _inputReader = parInputReader;
    _renderer = parRenderer;
    _rulesTextProvider = parRulesTextProvider;
    _gameSessionFactory = parGameSessionFactory;
    _recordsRepository = parRecordsRepository;
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
      ConsoleKeyInfo keyInfo = _inputReader.ReadKey();

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
          _renderer.Clear();
          return;
        }

        if (selectedItem.Kind == MenuItemKind.Rules)
        {
          RulesView rulesView = new RulesView(_renderer);
          RulesController rulesController = new RulesController(
            rulesView,
            _inputReader,
            _rulesTextProvider);
          rulesController.Run();
          _view.Render(selectedIndex);
          continue;
        }

        if (selectedItem.Kind == MenuItemKind.Records)
        {
          RecordsView recordsView = new RecordsView(_renderer);
          RecordsController recordsController = new RecordsController(
            recordsView,
            _inputReader,
            _recordsRepository);
          recordsController.Run();
          _view.Render(selectedIndex);
          continue;
        }

        if (selectedItem.Kind == MenuItemKind.Play)
        {
          GameSession session = _gameSessionFactory.Create();
          GameScreenView gameScreenView = new GameScreenView(_renderer, session.WorldBounds);
          GameScreenController gameScreenController = new GameScreenController(
            gameScreenView,
            session.EventBus,
            session.SnapshotProvider,
            session.GameLoopRunner,
            session.CommandQueue,
            _inputReader,
            session.Level);
          gameScreenController.Run();
          _view.Render(selectedIndex);
          continue;
        }

        _view.ShowNotImplemented(selectedItem.Title);
        _view.Render(selectedIndex);
      }
    }
  }
}
