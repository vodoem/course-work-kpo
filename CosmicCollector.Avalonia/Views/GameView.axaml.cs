using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CosmicCollector.Avalonia.ViewModels;

namespace CosmicCollector.Avalonia.Views;

/// <summary>
/// Представление игрового экрана.
/// </summary>
public sealed partial class GameView : UserControl
{
  private GameViewModel? _viewModel;

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
      _viewModel = viewModel;
      _viewModel.PropertyChanged += OnViewModelPropertyChanged;
      viewModel.Activate();
    }
  }

  private void OnDetachedFromVisualTree(object? parSender, VisualTreeAttachmentEventArgs parArgs)
  {
    if (_viewModel is not null)
    {
      _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
      _viewModel.Deactivate();
      _viewModel = null;
    }
  }

  private void OnViewModelPropertyChanged(object? parSender, PropertyChangedEventArgs parArgs)
  {
    if (parArgs.PropertyName != nameof(GameViewModel.IsPauseOverlayVisible))
    {
      return;
    }

    if (_viewModel?.IsPauseOverlayVisible != true)
    {
      return;
    }

    var resumeButton = this.FindControl<Button>("ResumeButton");
    if (resumeButton is null)
    {
      return;
    }

    Dispatcher.UIThread.Post(() => resumeButton.Focus(), DispatcherPriority.Background);
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
