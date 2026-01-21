using CosmicCollector.ConsoleApp.Infrastructure;

namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Контракт представления меню паузы.
/// </summary>
public interface IPauseMenuView
{
  /// <summary>
  /// Отрисовывает меню паузы.
  /// </summary>
  /// <param name="parMode">Режим меню.</param>
  /// <param name="parOptions">Пункты меню.</param>
  /// <param name="parSelectedIndex">Индекс выбранного пункта.</param>
  void Render(PauseMenuMode parMode, PauseMenuOption[] parOptions, int parSelectedIndex);
}
