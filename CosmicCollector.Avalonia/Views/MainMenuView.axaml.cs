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
    AttachedToVisualTree += OnAttachedToVisualTree;
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
}
