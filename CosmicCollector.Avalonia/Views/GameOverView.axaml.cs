using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
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
    ApplyCommands();
    ApplyViewModelState();
    FocusPlayerNameIfNeeded();
  }

  private void OnViewModelPropertyChanged(object? parSender, PropertyChangedEventArgs parArgs)
  {
    ApplyViewModelState(parArgs.PropertyName);
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

  private void ApplyCommands()
  {
    if (_viewModel is null)
    {
      return;
    }

    var saveButton = this.FindControl<Button>("SaveRecordButton");
    if (saveButton is not null)
    {
      saveButton.Command = _viewModel.SaveRecordCommand;
    }

    var backButton = this.FindControl<Button>("BackToMenuButton");
    if (backButton is not null)
    {
      backButton.Command = _viewModel.BackToMenuCommand;
    }

    var restartButton = this.FindControl<Button>("RestartButton");
    if (restartButton is not null)
    {
      restartButton.Command = _viewModel.RestartCommand;
    }

    var playerNameTextBox = this.FindControl<TextBox>("PlayerNameTextBox");
    if (playerNameTextBox is not null)
    {
      playerNameTextBox.TextChanged -= OnPlayerNameTextChanged;
      playerNameTextBox.TextChanged += OnPlayerNameTextChanged;
    }
  }

  private void ApplyViewModelState(string? parPropertyName = null)
  {
    if (_viewModel is null)
    {
      return;
    }

    if (parPropertyName is null || parPropertyName == nameof(GameOverViewModel.Level))
    {
      SetText("LevelValueText", _viewModel.Level.ToString());
    }

    if (parPropertyName is null || parPropertyName == nameof(GameOverViewModel.Score))
    {
      SetText("ScoreValueText", _viewModel.Score.ToString());
      SetText("FinalScoreText", _viewModel.Score.ToString());
    }

    if (parPropertyName is null || parPropertyName == nameof(GameOverViewModel.Energy))
    {
      SetText("EnergyValueText", _viewModel.Energy.ToString());
    }

    if (parPropertyName is null || parPropertyName == nameof(GameOverViewModel.TimerText))
    {
      SetText("TimerValueText", _viewModel.TimerText);
    }

    if (parPropertyName is null || parPropertyName == nameof(GameOverViewModel.IsInputModeVisible) ||
        parPropertyName == nameof(GameOverViewModel.IsTableModeVisible) ||
        parPropertyName == nameof(GameOverViewModel.IsHighScore) ||
        parPropertyName == nameof(GameOverViewModel.IsSaved))
    {
      var inputPanel = this.FindControl<StackPanel>("InputPanel");
      if (inputPanel is not null)
      {
        inputPanel.IsVisible = _viewModel.IsInputModeVisible;
      }

      var tablePanel = this.FindControl<Border>("TablePanel");
      if (tablePanel is not null)
      {
        tablePanel.IsVisible = _viewModel.IsTableModeVisible;
      }
    }

    if (parPropertyName is null || parPropertyName == nameof(GameOverViewModel.PlayerName))
    {
      var playerNameTextBox = this.FindControl<TextBox>("PlayerNameTextBox");
      if (playerNameTextBox is not null && playerNameTextBox.Text != _viewModel.PlayerName)
      {
        playerNameTextBox.Text = _viewModel.PlayerName;
      }
    }

    if (parPropertyName is null || parPropertyName == nameof(GameOverViewModel.NameError) ||
        parPropertyName == nameof(GameOverViewModel.HasNameError))
    {
      var errorText = this.FindControl<TextBlock>("NameErrorText");
      if (errorText is not null)
      {
        errorText.Text = _viewModel.NameError;
        errorText.IsVisible = _viewModel.HasNameError;
      }
    }

    if (parPropertyName is null || parPropertyName == nameof(GameOverViewModel.TopRecords) ||
        parPropertyName == nameof(GameOverViewModel.NoRecords) ||
        parPropertyName == nameof(GameOverViewModel.HasRecords))
    {
      RenderRecords();
    }

    if (parPropertyName == nameof(GameOverViewModel.IsNameInputActive))
    {
      FocusPlayerNameIfNeeded();
    }
  }

  private void RenderRecords()
  {
    if (_viewModel is null)
    {
      return;
    }

    var noRecordsText = this.FindControl<TextBlock>("NoRecordsText");
    var panel = this.FindControl<StackPanel>("RecordsPanel");
    var scrollViewer = this.FindControl<ScrollViewer>("RecordsScroll");
    if (panel is null || scrollViewer is null)
    {
      return;
    }

    panel.Children.Clear();

    if (_viewModel.TopRecords.Count == 0)
    {
      if (noRecordsText is not null)
      {
        noRecordsText.IsVisible = true;
      }

      scrollViewer.IsVisible = false;
      return;
    }

    if (noRecordsText is not null)
    {
      noRecordsText.IsVisible = false;
    }

    scrollViewer.IsVisible = true;
    panel.Children.Add(CreateHeaderRow());

    foreach (var record in _viewModel.TopRecords)
    {
      panel.Children.Add(CreateRecordRow(record));
    }
  }

  private static Control CreateHeaderRow()
  {
    var grid = CreateRowGrid();
    AddCell(grid, "Место", FontWeight.Bold);
    AddCell(grid, "Имя", FontWeight.Bold);
    AddCell(grid, "Уровень", FontWeight.Bold);
    AddCell(grid, "Очки", FontWeight.Bold);
    AddCell(grid, "Дата (UTC)", FontWeight.Bold);
    return grid;
  }

  private static Control CreateRecordRow(RecordsRowViewModel parRecord)
  {
    var grid = CreateRowGrid();
    AddCell(grid, parRecord.Position.ToString(), FontWeight.Normal);
    AddCell(grid, parRecord.PlayerName, FontWeight.Normal);
    AddCell(grid, parRecord.Level.ToString(), FontWeight.Normal);
    AddCell(grid, parRecord.Score.ToString(), FontWeight.Normal);
    AddCell(grid, parRecord.Time, FontWeight.Normal);
    return grid;
  }

  private static Grid CreateRowGrid()
  {
    return new Grid
    {
      ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto,Auto,Auto"),
      Margin = new Thickness(4, 0),
      HorizontalAlignment = HorizontalAlignment.Stretch
    };
  }

  private static void AddCell(Grid parGrid, string parText, FontWeight parWeight)
  {
    var columnIndex = parGrid.Children.Count;
    var textBlock = new TextBlock
    {
      Text = parText,
      FontWeight = parWeight,
      Foreground = Brushes.White,
      Margin = new Thickness(6, 2)
    };
    Grid.SetColumn(textBlock, columnIndex);
    parGrid.Children.Add(textBlock);
  }

  private void OnPlayerNameTextChanged(object? parSender, TextChangedEventArgs parArgs)
  {
    if (_viewModel is null)
    {
      return;
    }

    if (parSender is TextBox textBox)
    {
      _viewModel.PlayerName = textBox.Text ?? string.Empty;
    }
  }

  private void SetText(string parControlName, string parText)
  {
    var textBlock = this.FindControl<TextBlock>(parControlName);
    if (textBlock is not null)
    {
      textBlock.Text = parText;
    }
  }
}
