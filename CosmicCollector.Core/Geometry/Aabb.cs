namespace CosmicCollector.Core.Geometry;

/// <summary>
/// Представляет осевое ограничивающее прямоугольное поле.
/// </summary>
public readonly struct Aabb
{
  /// <summary>
  /// Инициализирует габариты.
  /// </summary>
  /// <param name="parWidth">Ширина.</param>
  /// <param name="parHeight">Высота.</param>
  public Aabb(double parWidth, double parHeight)
  {
    Width = parWidth;
    Height = parHeight;
  }

  /// <summary>
  /// Ширина.
  /// </summary>
  public double Width { get; }

  /// <summary>
  /// Высота.
  /// </summary>
  public double Height { get; }
}
