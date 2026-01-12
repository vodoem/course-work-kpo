using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering;
using CosmicCollector.Avalonia.Rendering;

namespace CosmicCollector.Avalonia.Views.Controls;

/// <summary>
/// Контрол отрисовки игрового поля через снимки.
/// </summary>
public sealed class GameFieldControl : Control, IRenderLoopTask
{
  /// <summary>
  /// Свойство хранилища снимков.
  /// </summary>
  public static readonly StyledProperty<FrameSnapshotStore?> SnapshotStoreProperty =
    AvaloniaProperty.Register<GameFieldControl, FrameSnapshotStore?>(nameof(SnapshotStore));

  private readonly SpriteAtlas _spriteAtlas = new();
  private readonly Typeface _fallbackTypeface = new("Inter");
  private IRenderLoop? _renderLoop;
  private bool _isAttached;
  private long _lastRenderTicks;

  /// <summary>
  /// Хранилище снимков рендера.
  /// </summary>
  public FrameSnapshotStore? SnapshotStore
  {
    get => GetValue(SnapshotStoreProperty);
    set => SetValue(SnapshotStoreProperty, value);
  }

  /// <inheritdoc />
  public bool NeedsUpdate => true;

  /// <inheritdoc />
  public void Update(TimeSpan parTime)
  {
    if (!_isAttached)
    {
      return;
    }

    _lastRenderTicks = System.Diagnostics.Stopwatch.GetTimestamp();
    InvalidateVisual();
  }

  /// <inheritdoc />
  public override void Render(DrawingContext parContext)
  {
    base.Render(parContext);

    var snapshot = SnapshotStore?.GetLatest();

    if (snapshot is null)
    {
      return;
    }

    foreach (var item in snapshot.Items)
    {
      var targetRect = new Rect(item.X, item.Y, item.Width, item.Height);

      if (_spriteAtlas.TryGetBitmap(item.SpriteKey, out var bitmap) && bitmap is not null)
      {
        parContext.DrawImage(bitmap, 1, new Rect(bitmap.Size), targetRect);
        continue;
      }

      var color = SpriteFallbackProvider.GetColor(item.SpriteKey);
      var label = SpriteFallbackProvider.GetLabel(item.SpriteKey);
      var brush = new SolidColorBrush(color);
      parContext.DrawRectangle(brush, new Pen(Brushes.White, 1), targetRect);

      var text = new FormattedText(
        label,
        _fallbackTypeface,
        12,
        TextAlignment.Center,
        TextWrapping.NoWrap,
        Size.Infinity);

      var textOrigin = new Point(
        targetRect.X + (targetRect.Width - text.Bounds.Width) / 2,
        targetRect.Y + (targetRect.Height - text.Bounds.Height) / 2);

      parContext.DrawText(Brushes.White, textOrigin, text);
    }
  }

  protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs parE)
  {
    base.OnAttachedToVisualTree(parE);
    _isAttached = true;
    _renderLoop = AvaloniaLocator.Current.GetService<IRenderLoop>();
    _renderLoop?.Add(this);
  }

  protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs parE)
  {
    base.OnDetachedFromVisualTree(parE);
    _isAttached = false;
    _renderLoop?.Remove(this);
    _renderLoop = null;
  }
}
