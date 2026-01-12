using System.Collections.Generic;

namespace CosmicCollector.Avalonia.Rendering;

/// <summary>
/// Представляет неизменяемый снимок рендера.
/// </summary>
public sealed class FrameSnapshot
{
  /// <summary>
  /// Инициализирует новый экземпляр <see cref="FrameSnapshot"/>.
  /// </summary>
  /// <param name="parTimestampTicks">Метка времени в тиках Stopwatch.</param>
  /// <param name="parItems">Список элементов рендера.</param>
  public FrameSnapshot(long parTimestampTicks, IReadOnlyList<RenderItem> parItems)
  {
    TimestampTicks = parTimestampTicks;
    Items = parItems;
  }

  /// <summary>
  /// Метка времени в тиках Stopwatch.
  /// </summary>
  public long TimestampTicks { get; }

  /// <summary>
  /// Элементы рендера.
  /// </summary>
  public IReadOnlyList<RenderItem> Items { get; }
}
