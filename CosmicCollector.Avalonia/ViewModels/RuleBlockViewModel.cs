using Avalonia;
using Avalonia.Media;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// Описывает блок текста правил для отображения.
/// </summary>
public sealed class RuleBlockViewModel
{
  /// <summary>
  /// Инициализирует новый экземпляр <see cref="RuleBlockViewModel"/>.
  /// </summary>
  /// <param name="parText">Текст блока.</param>
  /// <param name="parFontSize">Размер шрифта.</param>
  /// <param name="parFontWeight">Толщина шрифта.</param>
  /// <param name="parMargin">Отступы блока.</param>
  public RuleBlockViewModel(string parText, double parFontSize, FontWeight parFontWeight, Thickness parMargin)
  {
    Text = parText;
    FontSize = parFontSize;
    FontWeight = parFontWeight;
    Margin = parMargin;
  }

  /// <summary>
  /// Текст блока.
  /// </summary>
  public string Text { get; }

  /// <summary>
  /// Размер шрифта.
  /// </summary>
  public double FontSize { get; }

  /// <summary>
  /// Толщина шрифта.
  /// </summary>
  public FontWeight FontWeight { get; }

  /// <summary>
  /// Отступы блока.
  /// </summary>
  public Thickness Margin { get; }
}
