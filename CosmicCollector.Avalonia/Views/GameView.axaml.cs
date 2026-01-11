using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CosmicCollector.Avalonia.ViewModels;

namespace CosmicCollector.Avalonia.Views;

/// <summary>
/// Представление игрового экрана.
/// </summary>
public sealed partial class GameView : UserControl
{
  /// <summary>
  /// Инициализирует новый экземпляр <see cref="GameView"/>.
  /// </summary>
  public GameView()
  {
    InitializeComponent();
    Focusable = true;
    AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
    AddHandler(KeyUpEvent, OnKeyUp, RoutingStrategies.Tunnel);
    AttachedToVisualTree += OnAttachedToVisualTree;
    DetachedFromVisualTree += OnDetachedFromVisualTree;
  }

  private void OnAttachedToVisualTree(object? parSender, VisualTreeAttachmentEventArgs parArgs)
  {
    Focus();
    if (DataContext is GameViewModel viewModel)
    {
      viewModel.Activate();
    }
  }

  private void OnDetachedFromVisualTree(object? parSender, VisualTreeAttachmentEventArgs parArgs)
  {
    if (DataContext is GameViewModel viewModel)
    {
      viewModel.Deactivate();
    }
  }

  private void OnKeyDown(object? parSender, KeyEventArgs parArgs)
  {
    if (DataContext is GameViewModel viewModel)
    {
      viewModel.HandleKeyDown(parArgs.Key);
    }
  }

  private void OnKeyUp(object? parSender, KeyEventArgs parArgs)
  {
    if (DataContext is GameViewModel viewModel)
    {
      viewModel.HandleKeyUp(parArgs.Key);
    }
  }
}
