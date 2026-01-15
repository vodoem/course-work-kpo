using System;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using CosmicCollector.Avalonia.ViewModels;

namespace CosmicCollector.Avalonia.Views;

/// <summary>
/// Представление экрана рекордов.
/// </summary>
public sealed partial class RecordsView : UserControl
{
  /// <summary>
  /// Инициализирует новый экземпляр <see cref="RecordsView"/>.
  /// </summary>
  public RecordsView()
  {
    InitializeComponent();
    DataContextChanged += OnDataContextChanged;
  }

  private void OnDataContextChanged(object? parSender, EventArgs parArgs)
  {
    if (DataContext is not RecordsViewModel viewModel)
    {
      return;
    }

    var backButton = this.FindControl<Button>("BackButton");
    if (backButton is not null)
    {
      backButton.Command = viewModel.BackCommand;
    }

    RenderRecords(viewModel);
  }

  private void RenderRecords(RecordsViewModel parViewModel)
  {
    var noRecordsText = this.FindControl<TextBlock>("NoRecordsText");
    var panel = this.FindControl<StackPanel>("RecordsPanel");
    var scrollViewer = this.FindControl<ScrollViewer>("RecordsScroll");
    if (panel is null || scrollViewer is null)
    {
      return;
    }

    panel.Children.Clear();

    if (parViewModel.Records.Count == 0)
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

    foreach (var record in parViewModel.Records)
    {
      panel.Children.Add(CreateRecordRow(record));
    }
  }

  private static Control CreateHeaderRow()
  {
    var grid = CreateRowGrid();
    AddCell(grid, "№", FontWeight.Bold);
    AddCell(grid, "Имя", FontWeight.Bold);
    AddCell(grid, "Очки", FontWeight.Bold);
    AddCell(grid, "Уровень", FontWeight.Bold);
    AddCell(grid, "Время", FontWeight.Bold);
    return grid;
  }

  private static Control CreateRecordRow(RecordsRowViewModel parRecord)
  {
    var grid = CreateRowGrid();
    AddCell(grid, parRecord.Position.ToString(), FontWeight.Normal);
    AddCell(grid, parRecord.PlayerName, FontWeight.Normal);
    AddCell(grid, parRecord.Score.ToString(), FontWeight.Normal);
    AddCell(grid, parRecord.Level.ToString(), FontWeight.Normal);
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
      Foreground = Brushes.Black,
      Margin = new Thickness(6, 2)
    };
    Grid.SetColumn(textBlock, columnIndex);
    parGrid.Children.Add(textBlock);
  }
}
