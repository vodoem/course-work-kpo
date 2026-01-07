namespace CosmicCollector.Core.Geometry;

/// <summary>
/// Представляет границы игрового мира.
/// </summary>
public readonly struct WorldBounds
{
  /// <summary>
  /// Инициализирует границы мира.
  /// </summary>
  /// <param name="parLeft">Левая граница.</param>
  /// <param name="parTop">Верхняя граница.</param>
  /// <param name="parRight">Правая граница.</param>
  /// <param name="parBottom">Нижняя граница.</param>
  public WorldBounds(double parLeft, double parTop, double parRight, double parBottom)
  {
    Left = parLeft;
    Top = parTop;
    Right = parRight;
    Bottom = parBottom;
  }

  /// <summary>
  /// Левая граница.
  /// </summary>
  public double Left { get; }

  /// <summary>
  /// Верхняя граница.
  /// </summary>
  public double Top { get; }

  /// <summary>
  /// Правая граница.
  /// </summary>
  public double Right { get; }

  /// <summary>
  /// Нижняя граница.
  /// </summary>
  public double Bottom { get; }
}
