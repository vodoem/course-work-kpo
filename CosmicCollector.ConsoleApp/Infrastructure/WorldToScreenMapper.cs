using CosmicCollector.Core.Geometry;

namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Преобразует мировые координаты в экранные клетки.
/// Единственное место, где определяется ориентация оси Y.
/// </summary>
public sealed class WorldToScreenMapper
{
  private readonly WorldBounds _worldBounds;
  private readonly int _innerWidth;
  private readonly int _innerHeight;
  private readonly double _pixelsPerCell;

  /// <summary>
  /// Инициализирует маппер координат.
  /// </summary>
  /// <param name="parWorldBounds">Границы мира.</param>
  /// <param name="parInnerWidth">Ширина поля в клетках без рамки.</param>
  /// <param name="parInnerHeight">Высота поля в клетках без рамки.</param>
  /// <param name="parPixelsPerCell">Количество пикселей мира на клетку.</param>
  public WorldToScreenMapper(
    WorldBounds parWorldBounds,
    int parInnerWidth,
    int parInnerHeight,
    double parPixelsPerCell)
  {
    _worldBounds = parWorldBounds;
    _innerWidth = Math.Max(1, parInnerWidth);
    _innerHeight = Math.Max(1, parInnerHeight);
    _pixelsPerCell = Math.Max(1.0, parPixelsPerCell);
  }

  /// <summary>
  /// Преобразует координату X мира в клетку поля.
  /// </summary>
  /// <param name="parWorldX">Мировая координата X.</param>
  /// <returns>Клетка по X в диапазоне [0..innerWidth-1].</returns>
  public int MapX(double parWorldX)
  {
    double worldWidth = _worldBounds.Right - _worldBounds.Left;
    if (worldWidth <= 0)
    {
      return 0;
    }

    double maxCells = Math.Max(1.0, worldWidth / _pixelsPerCell);
    double cellX = (parWorldX - _worldBounds.Left) / _pixelsPerCell;
    double normalized = cellX / maxCells;
    normalized = Math.Clamp(normalized, 0, 1);
    return (int)Math.Round(normalized * (_innerWidth - 1));
  }

  /// <summary>
  /// Преобразует координату Y мира в клетку поля.
  /// </summary>
  /// <param name="parWorldY">Мировая координата Y.</param>
  /// <returns>Клетка по Y в диапазоне [0..innerHeight-1].</returns>
  public int MapY(double parWorldY)
  {
    double worldHeight = _worldBounds.Bottom - _worldBounds.Top;
    if (worldHeight <= 0)
    {
      return 0;
    }

    double maxCells = Math.Max(1.0, worldHeight / _pixelsPerCell);
    double cellY = (parWorldY - _worldBounds.Top) / _pixelsPerCell;
    double normalized = cellY / maxCells;
    normalized = Math.Clamp(normalized, 0, 1);
    return (int)Math.Round(normalized * (_innerHeight - 1));
  }
}
