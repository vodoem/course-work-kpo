using System.Globalization;
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

  static GameFieldControl()
  {
    SnapshotProperty.Changed.AddClassHandler<GameFieldControl>((control, args) =>
    {
      control.InvalidateVisual();
    });
  }

  /// <summary>
  /// Рисует элементы текущего снимка, проецируя координаты мира в экран.
  /// </summary>
  /// <remarks>
  /// Отрисовка выполняется только из snapshot и не содержит игровой логики.
  /// </remarks>
  public override void Render(DrawingContext parContext)
  {
    base.Render(parContext);

    var snapshot = Snapshot;

    if (snapshot is null)
    {
      return;
    }

    var mapper = new WorldToScreenMapper(
      snapshot.WorldBounds,
      Bounds.Width,
      Bounds.Height);

    if (!mapper.IsValid)
    {
      return;
    }

    foreach (var item in snapshot.Items)
    {
      var targetRect = mapper.MapRect(item.X, item.Y, item.Width, item.Height);

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

}
