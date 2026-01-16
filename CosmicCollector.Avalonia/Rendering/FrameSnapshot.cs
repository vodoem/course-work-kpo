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
  /// <param name="parWorldBounds">Границы мира.</param>
  public FrameSnapshot(
    long parTimestampTicks,
    IReadOnlyList<RenderItem> parItems,
    CosmicCollector.Core.Geometry.WorldBounds parWorldBounds)
  {
    TimestampTicks = parTimestampTicks;
    Items = parItems;
    WorldBounds = parWorldBounds;
  }

  /// <summary>
  /// Метка времени в тиках Stopwatch.
  /// </summary>
  public long TimestampTicks { get; }

  /// <summary>
  /// Элементы рендера.
  /// </summary>
  public IReadOnlyList<RenderItem> Items { get; }

  /// <summary>
  /// Границы мира.
  /// </summary>
  public CosmicCollector.Core.Geometry.WorldBounds WorldBounds { get; }
}
