using CosmicCollector.Core.Geometry;

namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Вспомогательные методы для рисования спрайтов в консоли.
/// </summary>
public sealed class ConsoleSpritePainter
{
  private readonly double _pixelsPerCell;

  /// <summary>
  /// Инициализирует помощник рисования.
  /// </summary>
  /// <param name="parPixelsPerCell">Количество пикселей мира на клетку.</param>
  public ConsoleSpritePainter(double parPixelsPerCell)
  {
    _pixelsPerCell = Math.Max(1.0, parPixelsPerCell);
  }

  /// <summary>
  /// Преобразует позицию и габариты Aabb в прямоугольник клеток внутри поля.
  /// </summary>
  /// <param name="parMapper">Маппер координат мира.</param>
  /// <param name="parPosition">Позиция объекта.</param>
  /// <param name="parBounds">Габариты объекта.</param>
  /// <param name="parOffsetY">Смещение поля по Y.</param>
  /// <param name="parFieldWidth">Ширина поля.</param>
  /// <param name="parFieldHeight">Высота поля.</param>
  /// <returns>Кортеж (Left, Top, Width, Height) в клетках.</returns>
  public (int Left, int Top, int Width, int Height) MapAabbToCellRect(
    WorldToScreenMapper parMapper,
    Vector2 parPosition,
    Aabb parBounds,
    int parOffsetY,
    int parFieldWidth,
    int parFieldHeight)
  {
    int centerX = 1 + parMapper.MapX(parPosition.X);
    int centerY = parOffsetY + 1 + parMapper.MapY(parPosition.Y);
    int widthCells = Math.Max(2, (int)Math.Round(parBounds.Width / _pixelsPerCell));
    int heightCells = Math.Max(2, (int)Math.Round(parBounds.Height / _pixelsPerCell));

    int left = centerX - (widthCells / 2);
    int top = centerY - (heightCells / 2);
    int right = left + widthCells - 1;
    int bottom = top + heightCells - 1;

    int minX = 1;
    int maxX = parFieldWidth - 2;
    int minY = parOffsetY + 1;
    int maxY = parOffsetY + parFieldHeight - 2;

    left = Math.Max(left, minX);
    top = Math.Max(top, minY);
    right = Math.Min(right, maxX);
    bottom = Math.Min(bottom, maxY);

    int width = right - left + 1;
    int height = bottom - top + 1;

    if (width <= 0 || height <= 0)
    {
      return (0, 0, 0, 0);
    }

    return (left, top, width, height);
  }

  /// <summary>
  /// Рисует заполненный прямоугольник.
  /// </summary>
  /// <param name="parBuffer">Буфер символов.</param>
  /// <param name="parColors">Буфер цветов.</param>
  /// <param name="parRect">Прямоугольник.</param>
  /// <param name="parFillChar">Символ заливки.</param>
  /// <param name="parColor">Цвет.</param>
  public void DrawFilledRect(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    (int Left, int Top, int Width, int Height) parRect,
    char parFillChar,
    ConsoleColor parColor)
  {
    if (parRect.Width <= 0 || parRect.Height <= 0)
    {
      return;
    }

    int right = parRect.Left + parRect.Width - 1;
    int bottom = parRect.Top + parRect.Height - 1;

    for (int y = parRect.Top; y <= bottom; y++)
    {
      for (int x = parRect.Left; x <= right; x++)
      {
        parBuffer[x, y] = parFillChar;
        parColors[x, y] = parColor;
      }
    }
  }

  /// <summary>
  /// Рисует глиф в центре прямоугольника.
  /// </summary>
  /// <param name="parBuffer">Буфер символов.</param>
  /// <param name="parColors">Буфер цветов.</param>
  /// <param name="parRect">Прямоугольник.</param>
  /// <param name="parGlyph">Глиф.</param>
  /// <param name="parColor">Цвет.</param>
  public void DrawCenteredGlyph(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    (int Left, int Top, int Width, int Height) parRect,
    char parGlyph,
    ConsoleColor parColor)
  {
    if (parRect.Width < 2 || parRect.Height < 2)
    {
      return;
    }

    int centerX = parRect.Left + (parRect.Width / 2);
    int centerY = parRect.Top + (parRect.Height / 2);
    parBuffer[centerX, centerY] = parGlyph;
    parColors[centerX, centerY] = parColor;
  }
}
