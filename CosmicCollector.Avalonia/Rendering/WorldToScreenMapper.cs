using Avalonia;
using CosmicCollector.Core.Geometry;

namespace CosmicCollector.Avalonia.Rendering;

/// <summary>
/// Преобразует мировые координаты в экранные.
/// Единственное место, где определяется масштабирование и ориентация оси Y.
/// </summary>
public sealed class WorldToScreenMapper
{
  private readonly WorldBounds _worldBounds;
  private readonly double _scale;
  private readonly double _offsetX;
  private readonly double _offsetY;

  /// <summary>
  /// Инициализирует маппер координат.
  /// </summary>
  /// <param name="parWorldBounds">Границы мира.</param>
  /// <param name="parViewportWidth">Ширина области отрисовки.</param>
  /// <param name="parViewportHeight">Высота области отрисовки.</param>
  public WorldToScreenMapper(
    WorldBounds parWorldBounds,
    double parViewportWidth,
    double parViewportHeight)
  {
    _worldBounds = parWorldBounds;
    var worldWidth = _worldBounds.Right - _worldBounds.Left;
    var worldHeight = _worldBounds.Bottom - _worldBounds.Top;

    if (worldWidth <= 0 || worldHeight <= 0 || parViewportWidth <= 0 || parViewportHeight <= 0)
    {
      IsValid = false;
      _scale = 0;
      _offsetX = 0;
      _offsetY = 0;
      return;
    }

    _scale = Math.Min(parViewportWidth / worldWidth, parViewportHeight / worldHeight);
    _offsetX = (parViewportWidth - (worldWidth * _scale)) / 2.0;
    _offsetY = (parViewportHeight - (worldHeight * _scale)) / 2.0;
    IsValid = true;
  }

  /// <summary>
  /// Признак корректности маппера.
  /// </summary>
  public bool IsValid { get; }

  /// <summary>
  /// Преобразует прямоугольник из мировых координат в экранные.
  /// </summary>
  /// <param name="parLeft">Левая граница в мире.</param>
  /// <param name="parTop">Верхняя граница в мире.</param>
  /// <param name="parWidth">Ширина в мире.</param>
  /// <param name="parHeight">Высота в мире.</param>
  /// <returns>Прямоугольник в экранных координатах.</returns>
  public Rect MapRect(double parLeft, double parTop, double parWidth, double parHeight)
  {
    var left = (parLeft - _worldBounds.Left) * _scale + _offsetX;
    var top = (parTop - _worldBounds.Top) * _scale + _offsetY;
    var width = parWidth * _scale;
    var height = parHeight * _scale;
    return new Rect(left, top, width, height);
  }
}
