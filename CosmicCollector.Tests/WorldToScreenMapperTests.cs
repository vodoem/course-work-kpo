using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.Core.Geometry;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет преобразование координат мира в координаты экрана.
/// </summary>
public sealed class WorldToScreenMapperTests
{
  /// <summary>
  /// Проверяет, что верхняя граница мира соответствует верхней строке поля.
  /// </summary>
  [Xunit.Fact]
  public void MapY_WorldTop_IsFirstRow()
  {
    var bounds = new WorldBounds(0, 0, 800, 600);
    var mapper = new WorldToScreenMapper(bounds, 40, 20, RenderConfig.PixelsPerCell);

    var screenY = mapper.MapY(bounds.Top);

    Xunit.Assert.Equal(0, screenY);
  }

  /// <summary>
  /// Проверяет, что нижняя граница мира соответствует нижней строке поля.
  /// </summary>
  [Xunit.Fact]
  public void MapY_WorldBottom_IsLastRow()
  {
    var bounds = new WorldBounds(0, 0, 800, 600);
    var mapper = new WorldToScreenMapper(bounds, 40, 20, RenderConfig.PixelsPerCell);

    var screenY = mapper.MapY(bounds.Bottom);

    Xunit.Assert.Equal(19, screenY);
  }

  /// <summary>
  /// Проверяет, что увеличение worldY приводит к увеличению screenY.
  /// </summary>
  [Xunit.Fact]
  public void MapY_IncreasesWithWorldY()
  {
    var bounds = new WorldBounds(0, 0, 800, 600);
    var mapper = new WorldToScreenMapper(bounds, 40, 20, RenderConfig.PixelsPerCell);

    var screenLow = mapper.MapY(100);
    var screenHigh = mapper.MapY(300);

    Xunit.Assert.True(screenHigh > screenLow);
  }
}
