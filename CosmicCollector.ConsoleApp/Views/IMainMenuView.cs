namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Контракт представления главного меню.
/// </summary>
public interface IMainMenuView
{
  /// <summary>
  /// Отрисовывает меню с текущим выбором.
  /// </summary>
  /// <param name="parSelectedIndex">Индекс выбранного пункта.</param>
  void Render(int parSelectedIndex);

  /// <summary>
  /// Отрисовывает сообщение о недоступном экране.
  /// </summary>
  /// <param name="parScreenTitle">Название экрана.</param>
  void ShowNotImplemented(string parScreenTitle);
}
