namespace CosmicCollector.Avalonia.Rendering;

/// <summary>
/// Элемент для отрисовки на игровом поле.
/// </summary>
public sealed class RenderItem
{
  /// <summary>
  /// Инициализирует новый экземпляр <see cref="RenderItem"/>.
  /// </summary>
  /// <param name="parX">Координата X (левый край) в мировых единицах.</param>
  /// <param name="parY">Координата Y (верхний край) в мировых единицах.</param>
  /// <param name="parWidth">Ширина в мировых единицах.</param>
  /// <param name="parHeight">Высота в мировых единицах.</param>
  /// <param name="parSpriteKey">Ключ спрайта.</param>
  /// <param name="parLayer">Слой отрисовки.</param>
  /// <param name="parOrder">Порядок внутри слоя.</param>
  public RenderItem(
    double parX,
    double parY,
    double parWidth,
    double parHeight,
    string parSpriteKey,
    int parLayer,
    int parOrder)
  {
    X = parX;
    Y = parY;
    Width = parWidth;
    Height = parHeight;
    SpriteKey = parSpriteKey;
    Layer = parLayer;
    Order = parOrder;
  }

  /// <summary>
  /// Координата X (левый край) в мировых единицах.
  /// </summary>
  public double X { get; }

  /// <summary>
  /// Координата Y (верхний край) в мировых единицах.
  /// </summary>
  public double Y { get; }

  /// <summary>
  /// Ширина в мировых единицах.
  /// </summary>
  public double Width { get; }

  /// <summary>
  /// Высота в мировых единицах.
  /// </summary>
  public double Height { get; }

  /// <summary>
  /// Ключ спрайта.
  /// </summary>
  public string SpriteKey { get; }

  /// <summary>
  /// Слой отрисовки.
  /// </summary>
  public int Layer { get; }

  /// <summary>
  /// Порядок внутри слоя.
  /// </summary>
  public int Order { get; }
}
