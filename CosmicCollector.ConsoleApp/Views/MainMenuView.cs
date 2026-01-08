using CosmicCollector.ConsoleApp.Infrastructure;

namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Представление главного меню в консоли.
/// </summary>
public sealed class MainMenuView : IMainMenuView
{
  private const string GameTitle = "Космический сборщик";
  private const string GameVersion = "v0.1";
  private readonly IConsoleRenderer _renderer;

  /// <summary>
  /// Создаёт представление главного меню.
  /// </summary>
  /// <param name="parRenderer">Рендерер консоли.</param>
  public MainMenuView(IConsoleRenderer parRenderer)
  {
    _renderer = parRenderer;
  }

  /// <inheritdoc />
  public void Render(int parSelectedIndex)
  {
    _renderer.Clear();
    _renderer.WriteLine("=============================");
    _renderer.WriteLine(GameTitle);
    _renderer.WriteLine($"Версия: {GameVersion}");
    _renderer.WriteLine("=============================");
    _renderer.WriteLine(string.Empty);

    MenuItem[] items = MenuItem.Items;

    for (int i = 0; i < items.Length; i++)
    {
      string prefix = i == parSelectedIndex ? ">" : " ";
      _renderer.WriteLine($"{prefix} {items[i].Title}");
    }

    _renderer.WriteLine(string.Empty);
    _renderer.WriteLine("Используйте стрелки Вверх/Вниз и Enter.");
  }

  /// <inheritdoc />
  public void ShowNotImplemented(string parScreenTitle)
  {
    _renderer.Clear();
    _renderer.WriteLine($"Экран \"{parScreenTitle}\" будет добавлен в следующих этапах.");
    _renderer.WriteLine(string.Empty);
    _renderer.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню...");
    _renderer.ReadKey();
  }
}
