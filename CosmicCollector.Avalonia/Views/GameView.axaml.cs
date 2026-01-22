using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CosmicCollector.Avalonia.ViewModels;
using CosmicCollector.Avalonia.Views.Controls;

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
    DataContextChanged += OnDataContextChanged;
  }

  private void OnDataContextChanged(object? parSender, EventArgs parArgs)
  {
    if (_viewModel is not null)
    {
      _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    _viewModel = DataContext as GameViewModel;
    if (_viewModel is null)
    {
      return;
    }

    _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    ApplyViewModelState();
    ApplyCommands();
  }

  private void OnAttachedToVisualTree(object? parSender, VisualTreeAttachmentEventArgs parArgs)
  {
    Focus();
    if (DataContext is GameViewModel viewModel)
    {
      _viewModel = viewModel;
      _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
      _viewModel.PropertyChanged += OnViewModelPropertyChanged;
      ApplyViewModelState();
      ApplyCommands();
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
    if (_viewModel is null)
    {
      return;
    }

    ApplyViewModelState(parArgs.PropertyName);
  }

  private void OnKeyDown(object? parSender, KeyEventArgs parArgs)
  {
    if (HandleConfirmSaveNavigation(parArgs))
    {
      return;
    }

    if (HandlePauseOverlayNavigation(parArgs))
    {
      return;
    }

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

  private bool HandleConfirmSaveNavigation(KeyEventArgs parArgs)
  {
    if (_viewModel?.IsConfirmSaveVisible != true)
    {
      return false;
    }

    if (parArgs.Key != Key.Down && parArgs.Key != Key.Up)
    {
      return false;
    }

    var yesButton = this.FindControl<Button>("ConfirmSaveYesButton");
    var noButton = this.FindControl<Button>("ConfirmSaveNoButton");
    var cancelButton = this.FindControl<Button>("ConfirmSaveCancelButton");
    if (yesButton is null || noButton is null || cancelButton is null)
    {
      return false;
    }

    if (yesButton.IsFocused)
    {
      noButton.Focus();
    }
    else if (noButton.IsFocused)
    {
      cancelButton.Focus();
    }
    else if (cancelButton.IsFocused)
    {
      yesButton.Focus();
    }
    else
    {
      yesButton.Focus();
    }

    parArgs.Handled = true;
    return true;
  }

  private bool HandlePauseOverlayNavigation(KeyEventArgs parArgs)
  {
    if (_viewModel?.IsPauseOverlayVisible != true)
    {
      return false;
    }

    if (parArgs.Key != Key.Down && parArgs.Key != Key.Up)
    {
      return false;
    }

    var resumeButton = this.FindControl<Button>("ResumeButton");
    var exitButton = this.FindControl<Button>("ExitToMenuButton");
    if (resumeButton is null || exitButton is null)
    {
      return false;
    }

    if (resumeButton.IsFocused || (!resumeButton.IsFocused && !exitButton.IsFocused))
    {
      exitButton.Focus();
    }
    else
    {
      resumeButton.Focus();
    }

    parArgs.Handled = true;
    return true;
  }

  private void ApplyCommands()
  {
    if (_viewModel is null)
    {
      return;
    }

    var resumeButton = this.FindControl<Button>("ResumeButton");
    if (resumeButton is not null)
    {
      resumeButton.Command = _viewModel.ResumeCommand;
    }

    var exitButton = this.FindControl<Button>("ExitToMenuButton");
    if (exitButton is not null)
    {
      exitButton.Command = _viewModel.ExitToMenuCommand;
    }

    var confirmYesButton = this.FindControl<Button>("ConfirmSaveYesButton");
    if (confirmYesButton is not null)
    {
      confirmYesButton.Command = _viewModel.ConfirmSaveYesCommand;
    }

    var confirmNoButton = this.FindControl<Button>("ConfirmSaveNoButton");
    if (confirmNoButton is not null)
    {
      confirmNoButton.Command = _viewModel.ConfirmSaveNoCommand;
    }

    var confirmCancelButton = this.FindControl<Button>("ConfirmSaveCancelButton");
    if (confirmCancelButton is not null)
    {
      confirmCancelButton.Command = _viewModel.ConfirmSaveCancelCommand;
    }
  }

  private void ApplyViewModelState(string? parPropertyName = null)
  {
    if (_viewModel is null)
    {
      return;
    }

    if (parPropertyName is null || parPropertyName == nameof(GameViewModel.LatestSnapshot))
    {
      var fieldControl = this.FindControl<GameFieldControl>("GameFieldControl");
      if (fieldControl is not null)
      {
        fieldControl.Snapshot = _viewModel.LatestSnapshot;
      }
    }

    if (parPropertyName is null || parPropertyName == nameof(GameViewModel.IsPauseOverlayVisible))
    {
      var overlay = this.FindControl<Grid>("PauseOverlay");
      if (overlay is not null)
      {
        overlay.IsVisible = _viewModel.IsPauseOverlayVisible;
      }

      if (_viewModel.IsPauseOverlayVisible)
      {
        FocusPauseButton();
      }
      else
      {
        Focus();
      }
    }

    if (parPropertyName is null || parPropertyName == nameof(GameViewModel.IsConfirmSaveVisible))
    {
      var overlay = this.FindControl<Grid>("ConfirmSaveOverlay");
      if (overlay is not null)
      {
        overlay.IsVisible = _viewModel.IsConfirmSaveVisible;
      }

      if (_viewModel.IsConfirmSaveVisible)
      {
        FocusConfirmSaveButton();
      }
    }

    if (parPropertyName is null || parPropertyName == nameof(GameViewModel.IsCountdownVisible))
    {
      var overlay = this.FindControl<Grid>("CountdownOverlay");
      if (overlay is not null)
      {
        overlay.IsVisible = _viewModel.IsCountdownVisible;
      }
    }

    if (parPropertyName is null || parPropertyName == nameof(GameViewModel.CountdownValue))
    {
      var countdownText = this.FindControl<TextBlock>("CountdownValueText");
      if (countdownText is not null)
      {
        countdownText.Text = _viewModel.CountdownValue.ToString();
      }
    }

    if (parPropertyName is null || parPropertyName == nameof(GameViewModel.TimerText))
    {
      var hudView = this.FindControl<HudView>("HudView");
      if (hudView is not null)
      {
        hudView.SetViewModel(_viewModel);
      }
    }
  }

  private void FocusPauseButton()
  {
    var resumeButton = this.FindControl<Button>("ResumeButton");
    if (resumeButton is null)
    {
      return;
    }

    Dispatcher.UIThread.Post(() =>
    {
      resumeButton.BringIntoView();
      resumeButton.Focus();
    }, DispatcherPriority.Loaded);
  }

  private void FocusConfirmSaveButton()
  {
    var confirmButton = this.FindControl<Button>("ConfirmSaveYesButton");
    if (confirmButton is null)
    {
      return;
    }

    Dispatcher.UIThread.Post(() =>
    {
      confirmButton.BringIntoView();
      confirmButton.Focus();
    }, DispatcherPriority.Loaded);
  }
}
