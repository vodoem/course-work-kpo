using System.ComponentModel;
using Avalonia.Controls;
using CosmicCollector.Avalonia.ViewModels;

namespace CosmicCollector.Avalonia.Views;

/// <summary>
/// Представление заглушки экрана завершения игры.
/// </summary>
public sealed partial class GameOverView : UserControl
{
  private GameOverViewModel? _viewModel;

  /// <summary>
  /// Инициализирует новый экземпляр <see cref="GameOverView"/>.
  /// </summary>
  public GameOverView()
  {
    InitializeComponent();
    DataContextChanged += OnDataContextChanged;
  }

  private void OnDataContextChanged(object? parSender, EventArgs parArgs)
  {
    if (_viewModel is not null)
    {
      _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    _viewModel = DataContext as GameOverViewModel;
    if (_viewModel is null)
    {
      return;
    }

    _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    FocusPlayerNameIfNeeded();
  }

  private void OnViewModelPropertyChanged(object? parSender, PropertyChangedEventArgs parArgs)
  {
    if (parArgs.PropertyName == nameof(GameOverViewModel.IsNameInputActive))
    {
      FocusPlayerNameIfNeeded();
    }
  }

  private void FocusPlayerNameIfNeeded()
  {
    if (_viewModel is null || !_viewModel.IsNameInputActive)
    {
      return;
    }

    var textBox = this.FindControl<TextBox>("PlayerNameTextBox");
    textBox?.Focus();
  }
}
