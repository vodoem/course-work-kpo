using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CosmicCollector.Core.Geometry;
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

  static GameFieldControl()
  {
    SnapshotProperty.Changed.AddClassHandler<GameFieldControl>((control, args) =>
    {
      control.InvalidateVisual();
    });
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

    var worldBounds = snapshot.WorldBounds;
    var worldWidth = worldBounds.Right - worldBounds.Left;
    var worldHeight = worldBounds.Bottom - worldBounds.Top;
    var viewportWidth = Bounds.Width;
    var viewportHeight = Bounds.Height;

    if (worldWidth <= 0 || worldHeight <= 0 || viewportWidth <= 0 || viewportHeight <= 0)
    {
      return;
    }

    var scale = Math.Min(viewportWidth / worldWidth, viewportHeight / worldHeight);
    var offsetX = (viewportWidth - (worldWidth * scale)) / 2.0;
    var offsetY = (viewportHeight - (worldHeight * scale)) / 2.0;

    foreach (var item in snapshot.Items)
    {
      var targetRect = BuildTargetRect(item, worldBounds, scale, offsetX, offsetY);

      if (_spriteAtlas.TryGetBitmap(item.SpriteKey, out var bitmap) && bitmap is not null)
      {
        parContext.DrawImage(bitmap, new Rect(bitmap.Size), targetRect);
        continue;
      }

      var color = SpriteFallbackProvider.GetColor(item.SpriteKey);
      var label = SpriteFallbackProvider.GetLabel(item.SpriteKey);
      var brush = new SolidColorBrush(color);
      parContext.DrawRectangle(brush, new Pen(Brushes.White, 1), targetRect);

      var text = new FormattedText(
        label,
        CultureInfo.InvariantCulture,
        FlowDirection.LeftToRight,
        _fallbackTypeface,
        12,
        Brushes.White);

      var textOrigin = new Point(
        targetRect.X + (targetRect.Width - text.Width) / 2,
        targetRect.Y + (targetRect.Height - text.Height) / 2);

      parContext.DrawText(text, textOrigin);
    }
  }

  private static Rect BuildTargetRect(
    RenderItem parItem,
    WorldBounds parWorldBounds,
    double parScale,
    double parOffsetX,
    double parOffsetY)
  {
    var left = (parItem.X - parWorldBounds.Left) * parScale + parOffsetX;
    var top = (parItem.Y - parWorldBounds.Top) * parScale + parOffsetY;
    var width = parItem.Width * parScale;
    var height = parItem.Height * parScale;
    return new Rect(left, top, width, height);
  }
}
