using CosmicCollector.ConsoleApp.Controllers;
using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет контроллер меню паузы.
/// </summary>
public sealed class PauseMenuControllerTests
{
  /// <summary>
  /// Проверяет переключение пунктов меню.
  /// </summary>
  [Xunit.Fact]
  public void HandleInput_ChangesSelection_WithUpDown()
  {
    var view = new TestPauseMenuView();
    var controller = new PauseMenuController(view);
    controller.OpenMenu();

    controller.HandleInput(new PauseMenuInput(false, true, false, false, false, false));
    Xunit.Assert.Equal(1, controller.SelectedIndex);

    controller.HandleInput(new PauseMenuInput(true, false, false, false, false, false));
    Xunit.Assert.Equal(0, controller.SelectedIndex);
  }

  /// <summary>
  /// Проверяет подтверждение выхода через Y.
  /// </summary>
  [Xunit.Fact]
  public void HandleInput_ConfirmExit_ReturnsExitAction()
  {
    var view = new TestPauseMenuView();
    var controller = new PauseMenuController(view);
    controller.OpenMenu();

    controller.HandleInput(new PauseMenuInput(false, true, false, false, false, false));
    controller.HandleInput(new PauseMenuInput(false, false, true, false, false, false));

    var action = controller.HandleInput(new PauseMenuInput(false, false, false, false, true, false));

    Xunit.Assert.Equal(PauseMenuAction.ExitToMenu, action);
  }

  /// <summary>
  /// Проверяет отмену подтверждения выхода через N.
  /// </summary>
  [Xunit.Fact]
  public void HandleInput_ConfirmExit_CancelsOnNo()
  {
    var view = new TestPauseMenuView();
    var controller = new PauseMenuController(view);
    controller.OpenMenu();

    controller.HandleInput(new PauseMenuInput(false, true, false, false, false, false));
    controller.HandleInput(new PauseMenuInput(false, false, true, false, false, false));

    controller.HandleInput(new PauseMenuInput(false, false, false, false, false, true));

    Xunit.Assert.Equal(PauseMenuMode.Menu, controller.Mode);
  }

  /// <summary>
  /// Проверяет продолжение по Enter на пункте Resume.
  /// </summary>
  [Xunit.Fact]
  public void HandleInput_Resume_ReturnsResumeAction()
  {
    var view = new TestPauseMenuView();
    var controller = new PauseMenuController(view);
    controller.OpenMenu();

    var action = controller.HandleInput(new PauseMenuInput(false, false, true, false, false, false));

    Xunit.Assert.Equal(PauseMenuAction.Resume, action);
  }

  private sealed class TestPauseMenuView : IPauseMenuView
  {
    /// <summary>
    /// Проверяет сценарий Render.
    /// </summary>
    public void Render(PauseMenuMode parMode, PauseMenuOption[] parOptions, int parSelectedIndex)
    {
    }
  }
}
