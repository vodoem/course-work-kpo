using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using CosmicCollector.Avalonia.Commands;
using CosmicCollector.Avalonia.Infrastructure;
using CosmicCollector.Avalonia.Navigation;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// ViewModel экрана правил.
/// </summary>
public sealed class RulesViewModel : ViewModelBase
{
  /// <summary>
  /// Инициализирует новый экземпляр <see cref="RulesViewModel"/>.
  /// </summary>
  /// <param name="parBackNavigation">Навигация назад в главное меню.</param>
  public RulesViewModel(NavigationService parBackNavigation)
  {
    var sections = RulesTextProvider.LoadSections();
    DescriptionBlocks = BuildBlocks(sections.Description);
    ControlsBlocks = BuildBlocks(sections.Controls);
    BackCommand = new NavigateCommand(parBackNavigation);
  }

  /// <summary>
  /// Блоки описания игры и правил.
  /// </summary>
  public IReadOnlyList<RuleBlockViewModel> DescriptionBlocks { get; }

  /// <summary>
  /// Блоки описания управления.
  /// </summary>
  public IReadOnlyList<RuleBlockViewModel> ControlsBlocks { get; }

  /// <summary>
  /// Команда возврата в главное меню.
  /// </summary>
  public ICommand BackCommand { get; }

  private static IReadOnlyList<RuleBlockViewModel> BuildBlocks(string parText)
  {
    var lines = parText
      .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
      .Select(line => line.TrimEnd())
      .Where(line => !string.IsNullOrWhiteSpace(line))
      .ToList();

    if (lines.Count == 0)
    {
      return new List<RuleBlockViewModel>();
    }

    var blocks = new List<RuleBlockViewModel>();

    foreach (var line in lines)
    {
      if (line.StartsWith("### ", StringComparison.Ordinal))
      {
        blocks.Add(CreateBlock(line[4..], 16, FontWeight.Bold, new Thickness(0, 8, 0, 4)));
        continue;
      }

      if (line.StartsWith("## ", StringComparison.Ordinal))
      {
        blocks.Add(CreateBlock(line[3..], 18, FontWeight.Bold, new Thickness(0, 10, 0, 6)));
        continue;
      }

      if (line.StartsWith("# ", StringComparison.Ordinal))
      {
        blocks.Add(CreateBlock(line[2..], 20, FontWeight.Bold, new Thickness(0, 12, 0, 8)));
        continue;
      }

      if (line.StartsWith("- ", StringComparison.Ordinal))
      {
        blocks.Add(CreateBlock($"• {line[2..]}", 14, FontWeight.Normal, new Thickness(0, 0, 0, 4)));
        continue;
      }

      var text = TrimEmphasis(line);
      blocks.Add(CreateBlock(text, 14, FontWeight.Normal, new Thickness(0, 0, 0, 6)));
    }

    return blocks;
  }

  private static RuleBlockViewModel CreateBlock(string parText, double parFontSize, FontWeight parFontWeight, Thickness parMargin)
  {
    return new RuleBlockViewModel(parText, parFontSize, parFontWeight, parMargin);
  }

  private static string TrimEmphasis(string parText)
  {
    return parText
      .Replace("**", string.Empty, StringComparison.Ordinal)
      .Replace("__", string.Empty, StringComparison.Ordinal)
      .Replace("*", string.Empty, StringComparison.Ordinal)
      .Replace("_", string.Empty, StringComparison.Ordinal);
  }
}
