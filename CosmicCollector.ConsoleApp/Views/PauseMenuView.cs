using CosmicCollector.ConsoleApp.Infrastructure;

namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Представление меню паузы.
/// </summary>
public sealed class PauseMenuView : IPauseMenuView
{
  private readonly IConsoleRenderer _renderer;

  /// <summary>
  /// Создаёт представление меню паузы.
  /// </summary>
  /// <param name="parRenderer">Рендерер консоли.</param>
  public PauseMenuView(IConsoleRenderer parRenderer)
  {
    _renderer = parRenderer;
  }

  /// <inheritdoc />
  public void Render(PauseMenuMode parMode, PauseMenuOption[] parOptions, int parSelectedIndex)
  {
    int width = Math.Max(20, _renderer.WindowWidth);
    int height = Math.Max(10, _renderer.WindowHeight);
    int fieldOffsetY = RenderConfig.HudHeight;
    int fieldHeight = Math.Max(6, height - fieldOffsetY);
    int centerY = fieldOffsetY + (fieldHeight / 2);

    if (parMode == PauseMenuMode.ConfirmExit)
    {
      RenderConfirm(width, centerY);
      return;
    }

    RenderMenu(parOptions, parSelectedIndex, width, centerY);
  }

  /// <summary>
  /// Выполняет RenderMenu.
  /// </summary>
  private void RenderMenu(PauseMenuOption[] parOptions, int parSelectedIndex, int parWidth, int parCenterY)
  {
    _renderer.SetForegroundColor(ConsoleColor.Yellow);
    WriteCentered(parCenterY - 2, parWidth, "ПАУЗА");

    for (int i = 0; i < parOptions.Length; i++)
    {
      string title = GetOptionTitle(parOptions[i]);
      string prefix = i == parSelectedIndex ? "> " : "  ";
      WriteCentered(parCenterY + i, parWidth, prefix + title);
    }

    _renderer.ResetColor();
  }

  /// <summary>
  /// Выполняет RenderConfirm.
  /// </summary>
  private void RenderConfirm(int parWidth, int parCenterY)
  {
    _renderer.SetForegroundColor(ConsoleColor.Yellow);
    WriteCentered(parCenterY, parWidth, "Прогресс не сохранится. Выйти в меню? (Y/N)");
    _renderer.ResetColor();
  }

  /// <summary>
  /// Выполняет WriteCentered.
  /// </summary>
  private void WriteCentered(int parRow, int parWidth, string parText)
  {
    int clampedRow = Math.Max(0, parRow);
    string text = parText.Length >= parWidth ? parText[..parWidth] : parText;
    int left = Math.Max(0, (parWidth - text.Length) / 2);
    _renderer.SetCursorPosition(left, clampedRow);
    _renderer.Write(text);
  }

  /// <summary>
  /// Выполняет GetOptionTitle.
  /// </summary>
  private static string GetOptionTitle(PauseMenuOption parOption)
  {
    return parOption switch
    {
      PauseMenuOption.Resume => "Продолжить",
      PauseMenuOption.ExitToMenu => "Выйти в меню",
      _ => string.Empty
    };
  }
}
