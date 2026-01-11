namespace CosmicCollector.Avalonia.Rendering;

/// <summary>
/// Элемент для отрисовки на игровом поле.
/// </summary>
public sealed class RenderItem
{
  /// <summary>
  /// Инициализирует новый экземпляр <see cref="RenderItem"/>.
  /// </summary>
  /// <param name="parX">Координата X (левый край).</param>
  /// <param name="parY">Координата Y (верхний край).</param>
  /// <param name="parWidth">Ширина.</param>
  /// <param name="parHeight">Высота.</param>
  /// <param name="parSpriteKey">Ключ спрайта.</param>
  public RenderItem(double parX, double parY, double parWidth, double parHeight, string parSpriteKey)
  {
    X = parX;
    Y = parY;
    Width = parWidth;
    Height = parHeight;
    SpriteKey = parSpriteKey;
  }

  /// <summary>
  /// Координата X (левый край).
  /// </summary>
  public double X { get; }

  /// <summary>
  /// Координата Y (верхний край).
  /// </summary>
  public double Y { get; }

  /// <summary>
  /// Ширина.
  /// </summary>
  public double Width { get; }

  /// <summary>
  /// Высота.
  /// </summary>
  public double Height { get; }

  /// <summary>
  /// Ключ спрайта.
  /// </summary>
  public string SpriteKey { get; }
}
