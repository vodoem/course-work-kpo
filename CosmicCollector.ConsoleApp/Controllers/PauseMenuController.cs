using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;

namespace CosmicCollector.ConsoleApp.Controllers;

/// <summary>
/// Контроллер меню паузы.
/// </summary>
public sealed class PauseMenuController
{
  private readonly IPauseMenuView _view;
  private readonly PauseMenuOption[] _options =
  {
    PauseMenuOption.Resume,
    PauseMenuOption.ExitToMenu
  };
  private int _selectedIndex;
  private PauseMenuMode _mode = PauseMenuMode.Menu;

  /// <summary>
  /// Создаёт контроллер меню паузы.
  /// </summary>
  /// <param name="parView">Представление меню паузы.</param>
  public PauseMenuController(IPauseMenuView parView)
  {
    _view = parView;
  }

  /// <summary>
  /// Открывает меню паузы.
  /// </summary>
  public void OpenMenu()
  {
    _mode = PauseMenuMode.Menu;
    _selectedIndex = 0;
  }

  /// <summary>
  /// Возвращает режим меню.
  /// </summary>
  public PauseMenuMode Mode => _mode;

  /// <summary>
  /// Возвращает индекс выбранного пункта.
  /// </summary>
  public int SelectedIndex => _selectedIndex;

  /// <summary>
  /// Обрабатывает ввод меню паузы.
  /// </summary>
  /// <param name="parInput">Снимок ввода.</param>
  /// <returns>Действие меню.</returns>
  public PauseMenuAction HandleInput(PauseMenuInput parInput)
  {
    if (_mode == PauseMenuMode.ConfirmExit)
    {
      if (parInput.parConfirmYes)
      {
        return PauseMenuAction.ExitToMenu;
      }

      if (parInput.parConfirmNo || parInput.parEscape)
      {
        _mode = PauseMenuMode.Menu;
      }

      return PauseMenuAction.None;
    }

    if (parInput.parUp)
    {
      _selectedIndex = (_selectedIndex - 1 + _options.Length) % _options.Length;
      return PauseMenuAction.None;
    }

    if (parInput.parDown)
    {
      _selectedIndex = (_selectedIndex + 1) % _options.Length;
      return PauseMenuAction.None;
    }

    if (parInput.parEnter)
    {
      var option = _options[_selectedIndex];
      if (option == PauseMenuOption.Resume)
      {
        return PauseMenuAction.Resume;
      }

      _mode = PauseMenuMode.ConfirmExit;
      return PauseMenuAction.None;
    }

    if (parInput.parEscape)
    {
      return PauseMenuAction.Resume;
    }

    return PauseMenuAction.None;
  }

  /// <summary>
  /// Отрисовывает меню паузы.
  /// </summary>
  public void Render()
  {
    _view.Render(_mode, _options, _selectedIndex);
  }
}
