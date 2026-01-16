using Avalonia;
using CosmicCollector.Core.Geometry;

namespace CosmicCollector.Avalonia.Rendering;

/// <summary>
/// Преобразует мировые координаты в экранные.
/// Единственное место, где определяется масштабирование и ориентация оси Y.
/// Масштабирование по X/Y может быть независимым, чтобы поле заполняло доступную область.
/// </summary>
public sealed class WorldToScreenMapper
{
  private readonly WorldBounds _worldBounds;
  private readonly double _scaleX;
  private readonly double _scaleY;

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
      _scaleX = 0;
      _scaleY = 0;
      return;
    }

    _scaleX = parViewportWidth / worldWidth;
    _scaleY = parViewportHeight / worldHeight;
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
    var left = (parLeft - _worldBounds.Left) * _scaleX;
    var top = (parTop - _worldBounds.Top) * _scaleY;
    var width = parWidth * _scaleX;
    var height = parHeight * _scaleY;
    return new Rect(left, top, width, height);
  }
}
