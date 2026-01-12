using System.Threading;

namespace CosmicCollector.Avalonia.Rendering;

/// <summary>
/// Хранит последний опубликованный снимок для рендера.
/// </summary>
public sealed class FrameSnapshotStore
{
  private FrameSnapshot? _latest;

  /// <summary>
  /// Обновляет текущий снимок.
  /// </summary>
  /// <param name="parSnapshot">Новый снимок.</param>
  public void Update(FrameSnapshot parSnapshot)
  {
    Interlocked.Exchange(ref _latest, parSnapshot);
  }

  /// <summary>
  /// Возвращает последний снимок.
  /// </summary>
  /// <returns>Снимок или null, если ещё не задан.</returns>
  public FrameSnapshot? GetLatest()
  {
    return Volatile.Read(ref _latest);
  }
}
