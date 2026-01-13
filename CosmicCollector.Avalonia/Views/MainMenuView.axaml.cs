using Avalonia.Controls;
using Avalonia.Threading;

namespace CosmicCollector.Avalonia.Views;

/// <summary>
/// Представление главного меню.
/// </summary>
public sealed partial class MainMenuView : UserControl
{
  /// <summary>
  /// Инициализирует новый экземпляр <see cref="MainMenuView"/>.
  /// </summary>
  public MainMenuView()
  {
    InitializeComponent();
    Focusable = true;
    AttachedToVisualTree += OnAttachedToVisualTree;
    AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
  }

  private void OnAttachedToVisualTree(object? parSender, VisualTreeAttachmentEventArgs parArgs)
  {
    var startButton = this.FindControl<Button>("StartGameButton");
    if (startButton is null)
    {
      return;
    }

    Dispatcher.UIThread.Post(() =>
    {
      startButton.BringIntoView();
      startButton.Focus();
    }, DispatcherPriority.Loaded);
  }

  private void OnKeyDown(object? parSender, KeyEventArgs parArgs)
  {
    if (parArgs.Key != Key.Down && parArgs.Key != Key.Up)
    {
      return;
    }

    var buttons = new[]
    {
      this.FindControl<Button>("StartGameButton"),
      this.FindControl<Button>("RulesButton"),
      this.FindControl<Button>("RecordsButton"),
      this.FindControl<Button>("ExitButton")
    };

    var focusedIndex = -1;
    for (var index = 0; index < buttons.Length; index++)
    {
      if (buttons[index]?.IsFocused == true)
      {
        focusedIndex = index;
        break;
      }
    }

    var direction = parArgs.Key == Key.Down ? 1 : -1;
    var startIndex = focusedIndex >= 0 ? focusedIndex : 0;
    var nextIndex = (startIndex + direction + buttons.Length) % buttons.Length;
    buttons[nextIndex]?.Focus();
    parArgs.Handled = true;
  }
}
