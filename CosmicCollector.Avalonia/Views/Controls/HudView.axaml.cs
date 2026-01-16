using System;
using System.ComponentModel;
using Avalonia.Controls;
using CosmicCollector.Avalonia.ViewModels;

namespace CosmicCollector.Avalonia.Views.Controls;

/// <summary>
/// HUD-слой игрового экрана.
/// </summary>
public sealed partial class HudView : UserControl
{
  private GameViewModel? _viewModel;

  /// <summary>
  /// Инициализирует новый экземпляр <see cref="HudView"/>.
  /// </summary>
  public HudView()
  {
    InitializeComponent();
    DataContextChanged += OnDataContextChanged;
  }

  /// <summary>
  /// Обновляет HUD из переданной ViewModel.
  /// </summary>
  /// <param name="parViewModel">ViewModel игрового экрана.</param>
  public void SetViewModel(GameViewModel parViewModel)
  {
    if (_viewModel == parViewModel)
    {
      return;
    }

    if (_viewModel is not null)
    {
      _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    _viewModel = parViewModel;
    _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    UpdateHud();
  }

  private void OnDataContextChanged(object? parSender, EventArgs parArgs)
  {
    if (DataContext is GameViewModel viewModel)
    {
      SetViewModel(viewModel);
      return;
    }

    if (_viewModel is not null)
    {
      _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
      _viewModel = null;
    }
  }

  private void OnViewModelPropertyChanged(object? parSender, PropertyChangedEventArgs parArgs)
  {
    if (_viewModel is null)
    {
      return;
    }

    switch (parArgs.PropertyName)
    {
      case nameof(GameViewModel.RequiredBlue):
      case nameof(GameViewModel.RequiredGreen):
      case nameof(GameViewModel.RequiredRed):
      case nameof(GameViewModel.RequiredScore):
      case nameof(GameViewModel.TimerText):
      case nameof(GameViewModel.CurrentLevel):
      case nameof(GameViewModel.CollectedBlue):
      case nameof(GameViewModel.CollectedGreen):
      case nameof(GameViewModel.CollectedRed):
      case nameof(GameViewModel.Score):
      case nameof(GameViewModel.Energy):
        UpdateHud();
        break;
    }
  }

  private void UpdateHud()
  {
    if (_viewModel is null)
    {
      return;
    }

    var requiredBlue = this.FindControl<TextBlock>("RequiredBlueText");
    var requiredGreen = this.FindControl<TextBlock>("RequiredGreenText");
    var requiredRed = this.FindControl<TextBlock>("RequiredRedText");
    var requiredScore = this.FindControl<TextBlock>("RequiredScoreText");
    var timer = this.FindControl<TextBlock>("TimerText");
    var currentLevel = this.FindControl<TextBlock>("CurrentLevelText");
    var collectedBlue = this.FindControl<TextBlock>("CollectedBlueText");
    var collectedGreen = this.FindControl<TextBlock>("CollectedGreenText");
    var collectedRed = this.FindControl<TextBlock>("CollectedRedText");
    var score = this.FindControl<TextBlock>("ScoreText");
    var energy = this.FindControl<TextBlock>("EnergyText");

    if (requiredBlue is not null)
    {
      requiredBlue.Text = _viewModel.RequiredBlue.ToString();
    }

    if (requiredGreen is not null)
    {
      requiredGreen.Text = _viewModel.RequiredGreen.ToString();
    }

    if (requiredRed is not null)
    {
      requiredRed.Text = _viewModel.RequiredRed.ToString();
    }

    if (requiredScore is not null)
    {
      requiredScore.Text = _viewModel.RequiredScore.ToString();
    }

    if (timer is not null)
    {
      timer.Text = _viewModel.TimerText;
    }

    if (currentLevel is not null)
    {
      currentLevel.Text = _viewModel.CurrentLevel.ToString();
    }

    if (collectedBlue is not null)
    {
      collectedBlue.Text = _viewModel.CollectedBlue.ToString();
    }

    if (collectedGreen is not null)
    {
      collectedGreen.Text = _viewModel.CollectedGreen.ToString();
    }

    if (collectedRed is not null)
    {
      collectedRed.Text = _viewModel.CollectedRed.ToString();
    }

    if (score is not null)
    {
      score.Text = _viewModel.Score.ToString();
    }

    if (energy is not null)
    {
      energy.Text = _viewModel.Energy.ToString();
    }
  }
}
