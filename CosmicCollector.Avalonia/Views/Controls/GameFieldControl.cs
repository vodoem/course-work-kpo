using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CosmicCollector.Avalonia.Rendering;

namespace CosmicCollector.Avalonia.Views.Controls;

/// <summary>
/// Контрол отрисовки игрового поля через снимки.
/// </summary>
public sealed class GameFieldControl : Control
{
  /// <summary>
  /// Свойство снимка рендера.
  /// </summary>
  public static readonly StyledProperty<FrameSnapshot?> SnapshotProperty =
    AvaloniaProperty.Register<GameFieldControl, FrameSnapshot?>(nameof(Snapshot));

  private readonly SpriteAtlas _spriteAtlas = new();
  private readonly Typeface _fallbackTypeface = new("Inter");

  /// <summary>
  /// Снимок рендера.
  /// </summary>
  public FrameSnapshot? Snapshot
  {
    get => GetValue(SnapshotProperty);
    set => SetValue(SnapshotProperty, value);
  }

  /// <inheritdoc />
  public override void Render(DrawingContext parContext)
  {
    base.Render(parContext);

    var snapshot = Snapshot;

    if (snapshot is null)
    {
      return;
    }

    foreach (var item in snapshot.Items)
    {
      var targetRect = new Rect(item.X, item.Y, item.Width, item.Height);

      if (_spriteAtlas.TryGetBitmap(item.SpriteKey, out var bitmap) && bitmap is not null)
      {
        parContext.DrawImage(bitmap, new Rect(bitmap.Size), targetRect);
        continue;
      }

      var color = SpriteFallbackProvider.GetColor(item.SpriteKey);
      var label = SpriteFallbackProvider.GetLabel(item.SpriteKey);
      var brush = new SolidColorBrush(color);
      parContext.DrawRectangle(brush, new Pen(Brushes.White, 1), targetRect);

      var text = new FormattedText(new FormattedTextOptions
      {
        Text = label,
        Typeface = _fallbackTypeface,
        FontSize = 12,
        TextAlignment = TextAlignment.Center,
        TextWrapping = TextWrapping.NoWrap,
        Constraint = Size.Infinity
      });

      var textOrigin = new Point(
        targetRect.X + (targetRect.Width - text.Bounds.Width) / 2,
        targetRect.Y + (targetRect.Height - text.Bounds.Height) / 2);

      parContext.DrawText(Brushes.White, textOrigin, text);
    }
  }

  protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs parChange)
  {
    base.OnPropertyChanged(parChange);

    if (parChange.Property == SnapshotProperty)
    {
      InvalidateVisual();
    }
  }
}
