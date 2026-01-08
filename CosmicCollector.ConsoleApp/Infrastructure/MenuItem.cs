namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Описание пункта главного меню.
/// </summary>
public sealed class MenuItem
{
  /// <summary>
  /// Массив пунктов меню в отображаемом порядке.
  /// </summary>
  public static readonly MenuItem[] Items =
  {
    new MenuItem(MenuItemKind.Play, "Играть"),
    new MenuItem(MenuItemKind.Records, "Рекорды"),
    new MenuItem(MenuItemKind.Rules, "Правила"),
    new MenuItem(MenuItemKind.Exit, "Выход")
  };

  /// <summary>
  /// Создаёт пункт меню.
  /// </summary>
  /// <param name="parKind">Тип пункта.</param>
  /// <param name="parTitle">Отображаемое название.</param>
  public MenuItem(MenuItemKind parKind, string parTitle)
  {
    Kind = parKind;
    Title = parTitle;
  }

  /// <summary>
  /// Тип пункта.
  /// </summary>
  public MenuItemKind Kind { get; }

  /// <summary>
  /// Название пункта.
  /// </summary>
  public string Title { get; }
}
