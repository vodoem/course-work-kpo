using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using CosmicCollector.Avalonia.ViewModels;

namespace CosmicCollector.Avalonia.Views;

/// <summary>
/// Представление экрана правил.
/// </summary>
public sealed partial class RulesView : UserControl
{
  /// <summary>
  /// Инициализирует новый экземпляр <see cref="RulesView"/>.
  /// </summary>
  public RulesView()
  {
    InitializeComponent();
    DataContextChanged += OnDataContextChanged;
  }

  private void OnDataContextChanged(object? parSender, EventArgs parArgs)
  {
    if (DataContext is not RulesViewModel viewModel)
    {
      return;
    }

    var backButton = this.FindControl<Button>("BackButton");
    if (backButton is not null)
    {
      backButton.Command = viewModel.BackCommand;
    }

    PopulateBlocks("DescriptionItems", viewModel.DescriptionBlocks);
    PopulateBlocks("ControlsItems", viewModel.ControlsBlocks);
  }

  private void PopulateBlocks(string parItemsName, IReadOnlyList<RuleBlockViewModel> parBlocks)
  {
    var itemsControl = this.FindControl<ItemsControl>(parItemsName);
    if (itemsControl is null)
    {
      return;
    }

    var items = new List<TextBlock>(parBlocks.Count);
    foreach (var block in parBlocks)
    {
      var textBlock = new TextBlock
      {
        Text = block.Text,
        TextWrapping = TextWrapping.Wrap,
        FontSize = block.FontSize,
        FontWeight = block.FontWeight,
        Margin = block.Margin
      };
      items.Add(textBlock);
    }
    itemsControl.Items = items;
  }
}
