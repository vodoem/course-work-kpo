using CosmicCollector.ConsoleApp.Infrastructure;

namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Представление главного меню в консоли.
/// </summary>
public sealed class MainMenuView : IMainMenuView
{
  private const string GameTitle = "Космический сборщик";
  private const string GameVersion = "v0.1";

  /// <inheritdoc />
  public void Render(int parSelectedIndex)
  {
    Console.Clear();
    Console.WriteLine("=============================");
    Console.WriteLine($"{GameTitle}");
    Console.WriteLine($"Версия: {GameVersion}");
    Console.WriteLine("=============================");
    Console.WriteLine();

    MenuItem[] items = MenuItem.Items;

    for (int i = 0; i < items.Length; i++)
    {
      string prefix = i == parSelectedIndex ? ">" : " ";
      Console.WriteLine($"{prefix} {items[i].Title}");
    }

    Console.WriteLine();
    Console.WriteLine("Используйте стрелки Вверх/Вниз и Enter.");
  }

  /// <inheritdoc />
  public void ShowNotImplemented(string parScreenTitle)
  {
    Console.Clear();
    Console.WriteLine($"Экран \"{parScreenTitle}\" будет добавлен в следующих этапах.");
    Console.WriteLine();
    Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню...");
    Console.ReadKey(true);
  }
}
