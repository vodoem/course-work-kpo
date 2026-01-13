using Avalonia;
using Avalonia.Controls;

namespace CosmicCollector.Avalonia.Views.Controls;

/// <summary>
/// Контейнер, сохраняющий заданное соотношение сторон для дочернего элемента.
/// </summary>
public sealed class AspectRatioDecorator : Decorator
{
  /// <summary>
  /// Свойство соотношения сторон (ширина/высота).
  /// </summary>
  public static readonly StyledProperty<double> AspectRatioProperty =
    AvaloniaProperty.Register<AspectRatioDecorator, double>(nameof(AspectRatio), 1.0);

  /// <summary>
  /// Соотношение сторон (ширина/высота).
  /// </summary>
  public double AspectRatio
  {
    get => GetValue(AspectRatioProperty);
    set => SetValue(AspectRatioProperty, value);
  }

  /// <inheritdoc />
  protected override Size MeasureOverride(Size availableSize)
  {
    if (Child is null)
    {
      return new Size();
    }

    var targetSize = CalculateTargetSize(availableSize);
    Child.Measure(targetSize);
    return targetSize;
  }

  /// <inheritdoc />
  protected override Size ArrangeOverride(Size finalSize)
  {
    if (Child is null)
    {
      return finalSize;
    }

    var targetSize = CalculateTargetSize(finalSize);
    var offsetX = (finalSize.Width - targetSize.Width) / 2.0;
    var offsetY = (finalSize.Height - targetSize.Height) / 2.0;
    Child.Arrange(new Rect(new Point(offsetX, offsetY), targetSize));
    return finalSize;
  }

  private Size CalculateTargetSize(Size parAvailableSize)
  {
    var ratio = AspectRatio;
    if (ratio <= 0 || double.IsNaN(ratio) || double.IsInfinity(ratio))
    {
      return parAvailableSize;
    }

    var availableWidth = parAvailableSize.Width;
    var availableHeight = parAvailableSize.Height;

    if (double.IsInfinity(availableWidth) && double.IsInfinity(availableHeight))
    {
      return new Size();
    }

    if (double.IsInfinity(availableWidth))
    {
      availableWidth = availableHeight * ratio;
    }

    if (double.IsInfinity(availableHeight))
    {
      availableHeight = availableWidth / ratio;
    }

    var targetWidth = availableWidth;
    var targetHeight = availableWidth / ratio;

    if (targetHeight > availableHeight)
    {
      targetHeight = availableHeight;
      targetWidth = availableHeight * ratio;
    }

    return new Size(Math.Max(0, targetWidth), Math.Max(0, targetHeight));
  }
}
