using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;

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

  /// <summary>
  /// Создаёт контроллер главного меню.
  /// </summary>
  /// <param name="parView">Представление главного меню.</param>
  /// <param name="parInputReader">Читатель ввода.</param>
  /// <param name="parRenderer">Рендерер консоли.</param>
  /// <param name="parRulesTextProvider">Поставщик текста правил.</param>
  public MainMenuController(
    IMainMenuView parView,
    IConsoleInputReader parInputReader,
    IConsoleRenderer parRenderer,
    IRulesTextProvider parRulesTextProvider)
  {
    _view = parView;
    _inputReader = parInputReader;
    _renderer = parRenderer;
    _rulesTextProvider = parRulesTextProvider;
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

        _view.ShowNotImplemented(selectedItem.Title);
        _view.Render(selectedIndex);
      }
    }
  }
}
