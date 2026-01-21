using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Snapshots;
using System;
using System.Text;

namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Представление игрового экрана (консоль).
/// </summary>
public sealed class GameScreenView : IGameScreenView
{
  private const char BorderChar = '#';
  private const char DroneFillChar = '░';
  private const char DroneGlyphChar = '▲';
  private const char CrystalFillChar = '░';
  private const char CrystalBlueGlyphChar = '♦';
  private const char CrystalGreenGlyphChar = '♣';
  private const char CrystalRedGlyphChar = '♥';
  private const char AsteroidFillChar = '▒';
  private const char AsteroidGlyphChar = '●';
  private const char BonusFillChar = '░';
  private const char BonusAcceleratorGlyphChar = 'A';
  private const char BonusTimeStabilizerGlyphChar = 'T';
  private const char BonusMagnetGlyphChar = 'M';
  private const char BlackHoleCoreFillChar = '▓';
  private const char BlackHoleCoreGlyphChar = '●';
  private const char BlackHoleFieldChar = '·';
  private const int HudHeight = RenderConfig.HudHeight;
  private const double PixelsPerCell = RenderConfig.PixelsPerCell;
  private readonly IConsoleRenderer _renderer;
  private readonly WorldBounds _worldBounds;
  private readonly ConsoleSpritePainter _spritePainter;

  /// <summary>
  /// Создаёт представление игрового экрана.
  /// </summary>
  /// <param name="parRenderer">Рендерер консоли.</param>
  /// <param name="parWorldBounds">Границы игрового мира.</param>
  public GameScreenView(IConsoleRenderer parRenderer, WorldBounds parWorldBounds)
  {
    _renderer = parRenderer;
    _worldBounds = parWorldBounds;
    _spritePainter = new ConsoleSpritePainter(PixelsPerCell);
  }

  /// <inheritdoc />
  public void Render(GameSnapshot parSnapshot, int parLevel, bool parIsPaused, int parCountdownValue)
  {
    int width = Math.Max(20, _renderer.WindowWidth);
    int height = Math.Max(10, _renderer.WindowHeight);
    _renderer.SetBufferSize(width, height);
    char[,] buffer = CreateBuffer(width, height);
    ConsoleColor[,] colors = CreateColorBuffer(width, height, ConsoleColor.Gray);

    DrawHud(buffer, colors, width, parSnapshot, parLevel);

    int fieldOffsetY = HudHeight;
    int fieldWidth = width;
    int fieldHeight = Math.Max(6, height - fieldOffsetY);
    int innerWidth = Math.Max(1, fieldWidth - 2);
    int innerHeight = Math.Max(1, fieldHeight - 2);
    var mapper = new WorldToScreenMapper(_worldBounds, innerWidth, innerHeight, PixelsPerCell);

    DrawBorder(buffer, colors, fieldOffsetY, fieldWidth, fieldHeight);
    DrawBlackHoleFields(buffer, colors, mapper, parSnapshot.parBlackHoles, fieldOffsetY, fieldWidth, fieldHeight);
    DrawCrystals(buffer, colors, mapper, parSnapshot.parCrystals, fieldOffsetY, fieldWidth, fieldHeight);
    DrawBonuses(buffer, colors, mapper, parSnapshot.parBonuses, fieldOffsetY, fieldWidth, fieldHeight);
    DrawAsteroids(buffer, colors, mapper, parSnapshot.parAsteroids, fieldOffsetY, fieldWidth, fieldHeight);
    DrawBlackHoles(buffer, colors, mapper, parSnapshot.parBlackHoles, fieldOffsetY, fieldWidth, fieldHeight);
    DrawDrone(buffer, colors, mapper, parSnapshot.parDrone, fieldOffsetY, fieldWidth, fieldHeight);
    DrawOverlay(buffer, colors, fieldOffsetY, fieldWidth, fieldHeight, parIsPaused, parCountdownValue);

    RenderBuffer(buffer, colors, width, height);
  }

  /// <summary>
  /// Выполняет CreateBuffer.
  /// </summary>
  private char[,] CreateBuffer(int parWidth, int parHeight)
  {
    var buffer = new char[parWidth, parHeight];

    for (int x = 0; x < parWidth; x++)
    {
      for (int y = 0; y < parHeight; y++)
      {
        buffer[x, y] = ' ';
      }
    }

    return buffer;
  }

  /// <summary>
  /// Выполняет CreateColorBuffer.
  /// </summary>
  private ConsoleColor[,] CreateColorBuffer(int parWidth, int parHeight, ConsoleColor parDefault)
  {
    var colors = new ConsoleColor[parWidth, parHeight];

    for (int x = 0; x < parWidth; x++)
    {
      for (int y = 0; y < parHeight; y++)
      {
        colors[x, y] = parDefault;
      }
    }

    return colors;
  }

  /// <summary>
  /// Выполняет DrawBorder.
  /// </summary>
  private void DrawBorder(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    int parOffsetY,
    int parWidth,
    int parHeight)
  {
    for (int x = 0; x < parWidth; x++)
    {
      parBuffer[x, parOffsetY] = BorderChar;
      parBuffer[x, parOffsetY + parHeight - 1] = BorderChar;
      parColors[x, parOffsetY] = ConsoleColor.DarkGray;
      parColors[x, parOffsetY + parHeight - 1] = ConsoleColor.DarkGray;
    }

    for (int y = 0; y < parHeight; y++)
    {
      parBuffer[0, parOffsetY + y] = BorderChar;
      parBuffer[parWidth - 1, parOffsetY + y] = BorderChar;
      parColors[0, parOffsetY + y] = ConsoleColor.DarkGray;
      parColors[parWidth - 1, parOffsetY + y] = ConsoleColor.DarkGray;
    }
  }

  /// <summary>
  /// Выполняет DrawDrone.
  /// </summary>
  private void DrawDrone(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    WorldToScreenMapper parMapper,
    DroneSnapshot parDrone,
    int parOffsetY,
    int parWidth,
    int parHeight)
  {
    var rect = _spritePainter.MapAabbToCellRect(
      parMapper,
      parDrone.parPosition,
      parDrone.parBounds,
      parOffsetY,
      parWidth,
      parHeight);
    _spritePainter.DrawFilledRect(parBuffer, parColors, rect, DroneFillChar, ConsoleColor.White);
    _spritePainter.DrawCenteredGlyph(parBuffer, parColors, rect, DroneGlyphChar, ConsoleColor.White);
  }

  /// <summary>
  /// Выполняет DrawCrystals.
  /// </summary>
  private void DrawCrystals(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    WorldToScreenMapper parMapper,
    IReadOnlyList<CrystalSnapshot> parCrystals,
    int parOffsetY,
    int parWidth,
    int parHeight)
  {
    foreach (var crystal in parCrystals)
    {
      var rect = _spritePainter.MapAabbToCellRect(
        parMapper,
        crystal.parPosition,
        crystal.parBounds,
        parOffsetY,
        parWidth,
        parHeight);
      var glyphColor = crystal.parType switch
      {
        CrystalType.Blue => ConsoleColor.Cyan,
        CrystalType.Green => ConsoleColor.Green,
        CrystalType.Red => ConsoleColor.Red,
        _ => ConsoleColor.Cyan
      };
      var glyphChar = crystal.parType switch
      {
        CrystalType.Blue => CrystalBlueGlyphChar,
        CrystalType.Green => CrystalGreenGlyphChar,
        CrystalType.Red => CrystalRedGlyphChar,
        _ => CrystalBlueGlyphChar
      };
      _spritePainter.DrawFilledRect(parBuffer, parColors, rect, CrystalFillChar, glyphColor);
      _spritePainter.DrawCenteredGlyph(parBuffer, parColors, rect, glyphChar, glyphColor);
    }
  }

  /// <summary>
  /// Выполняет DrawAsteroids.
  /// </summary>
  private void DrawAsteroids(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    WorldToScreenMapper parMapper,
    IReadOnlyList<AsteroidSnapshot> parAsteroids,
    int parOffsetY,
    int parWidth,
    int parHeight)
  {
    foreach (var asteroid in parAsteroids)
    {
      var rect = _spritePainter.MapAabbToCellRect(
        parMapper,
        asteroid.parPosition,
        asteroid.parBounds,
        parOffsetY,
        parWidth,
        parHeight);
      _spritePainter.DrawFilledRect(parBuffer, parColors, rect, AsteroidFillChar, ConsoleColor.DarkGray);
      _spritePainter.DrawCenteredGlyph(parBuffer, parColors, rect, AsteroidGlyphChar, ConsoleColor.DarkGray);
    }
  }

  /// <summary>
  /// Выполняет DrawBonuses.
  /// </summary>
  private void DrawBonuses(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    WorldToScreenMapper parMapper,
    IReadOnlyList<BonusSnapshot> parBonuses,
    int parOffsetY,
    int parWidth,
    int parHeight)
  {
    foreach (var bonus in parBonuses)
    {
      var rect = _spritePainter.MapAabbToCellRect(
        parMapper,
        bonus.parPosition,
        bonus.parBounds,
        parOffsetY,
        parWidth,
        parHeight);
      var glyphColor = bonus.parType switch
      {
        BonusType.Accelerator => ConsoleColor.Yellow,
        BonusType.TimeStabilizer => ConsoleColor.Magenta,
        BonusType.Magnet => ConsoleColor.Cyan,
        _ => ConsoleColor.Yellow
      };
      var glyphChar = bonus.parType switch
      {
        BonusType.Accelerator => BonusAcceleratorGlyphChar,
        BonusType.TimeStabilizer => BonusTimeStabilizerGlyphChar,
        BonusType.Magnet => BonusMagnetGlyphChar,
        _ => BonusAcceleratorGlyphChar
      };
      _spritePainter.DrawFilledRect(parBuffer, parColors, rect, BonusFillChar, glyphColor);
      _spritePainter.DrawCenteredGlyph(parBuffer, parColors, rect, glyphChar, glyphColor);
    }
  }

  /// <summary>
  /// Выполняет DrawBlackHoleFields.
  /// </summary>
  private void DrawBlackHoleFields(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    WorldToScreenMapper parMapper,
    IReadOnlyList<BlackHoleSnapshot> parBlackHoles,
    int parOffsetY,
    int parWidth,
    int parHeight)
  {
    foreach (var blackHole in parBlackHoles)
    {
      DrawBlackHoleField(parBuffer, parColors, parMapper, blackHole, parOffsetY, parWidth, parHeight);
    }
  }

  /// <summary>
  /// Выполняет DrawBlackHoleField.
  /// </summary>
  private void DrawBlackHoleField(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    WorldToScreenMapper parMapper,
    BlackHoleSnapshot parBlackHole,
    int parOffsetY,
    int parWidth,
    int parHeight)
  {
    int centerX = 1 + parMapper.MapX(parBlackHole.parPosition.X);
    int centerY = parOffsetY + 1 + parMapper.MapY(parBlackHole.parPosition.Y);
    int radiusCells = (int)Math.Round(parBlackHole.parRadius / PixelsPerCell);

    if (radiusCells < 1)
    {
      return;
    }

    int minX = Math.Max(1, centerX - radiusCells);
    int maxX = Math.Min(parWidth - 2, centerX + radiusCells);
    int minY = Math.Max(parOffsetY + 1, centerY - radiusCells);
    int maxY = Math.Min(parOffsetY + parHeight - 2, centerY + radiusCells);
    int radiusSquared = radiusCells * radiusCells;

    for (int y = minY; y <= maxY; y++)
    {
      int dy = y - centerY;
      for (int x = minX; x <= maxX; x++)
      {
        int dx = x - centerX;
        int distanceSquared = (dx * dx) + (dy * dy);
        int delta = Math.Abs(distanceSquared - radiusSquared);

        if (delta <= radiusCells && ((x + y) % 2 == 0))
        {
          parBuffer[x, y] = BlackHoleFieldChar;
          parColors[x, y] = ConsoleColor.DarkMagenta;
        }
      }
    }
  }

  /// <summary>
  /// Выполняет DrawBlackHoles.
  /// </summary>
  private void DrawBlackHoles(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    WorldToScreenMapper parMapper,
    IReadOnlyList<BlackHoleSnapshot> parBlackHoles,
    int parOffsetY,
    int parWidth,
    int parHeight)
  {
    foreach (var blackHole in parBlackHoles)
    {
      var rect = _spritePainter.MapAabbToCellRect(
        parMapper,
        blackHole.parPosition,
        blackHole.parBounds,
        parOffsetY,
        parWidth,
        parHeight);
      _spritePainter.DrawFilledRect(parBuffer, parColors, rect, BlackHoleCoreFillChar, ConsoleColor.DarkMagenta);
      _spritePainter.DrawCenteredGlyph(parBuffer, parColors, rect, BlackHoleCoreGlyphChar, ConsoleColor.DarkMagenta);
    }
  }

  /// <summary>
  /// Выполняет DrawHud.
  /// </summary>
  private void DrawHud(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    int parWidth,
    GameSnapshot parSnapshot,
    int parLevel)
  {
    string goals = $"Цель очков: {parSnapshot.parRequiredScore}";
    string progress = $"Уровень: {parSnapshot.parCurrentLevel} | Энергия: {parSnapshot.parDrone.parEnergy} | Очки: {parSnapshot.parDrone.parScore}";
    string timerLine = GetTimerText(parSnapshot, parWidth / 3);
    WriteHudLine(parBuffer, parColors, 0, parWidth, goals, timerLine, progress);

    string goalsLine = $"Цели: B={parSnapshot.parLevelGoals.parRequiredBlue} " +
                       $"G={parSnapshot.parLevelGoals.parRequiredGreen} " +
                       $"R={parSnapshot.parLevelGoals.parRequiredRed}";
    string progressLine = $"Прогресс: B={parSnapshot.parLevelProgress.parCollectedBlue}/{parSnapshot.parLevelGoals.parRequiredBlue} " +
                          $"G={parSnapshot.parLevelProgress.parCollectedGreen}/{parSnapshot.parLevelGoals.parRequiredGreen} " +
                          $"R={parSnapshot.parLevelProgress.parCollectedRed}/{parSnapshot.parLevelGoals.parRequiredRed}";
    WriteHudLine(parBuffer, parColors, 1, parWidth, goalsLine, string.Empty, progressLine);
  }

  /// <summary>
  /// Выполняет WriteHudLine.
  /// </summary>
  private void WriteHudLine(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    int parRow,
    int parWidth,
    string parLeft,
    string parCenter,
    string parRight)
  {
    int third = parWidth / 3;
    WriteText(parBuffer, parColors, parRow, parWidth, parLeft, 0, third, ConsoleColor.White);
    WriteText(parBuffer, parColors, parRow, parWidth, parCenter, third, third, ConsoleColor.Cyan);
    WriteText(parBuffer, parColors, parRow, parWidth, parRight, third * 2, parWidth - (third * 2), ConsoleColor.White);
  }

  /// <summary>
  /// Выполняет WriteText.
  /// </summary>
  private void WriteText(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    int parRow,
    int parWidth,
    string parText,
    int parStart,
    int parMaxLength,
    ConsoleColor parColor)
  {
    if (parRow < 0 || parRow >= parBuffer.GetLength(1))
    {
      return;
    }

    string text = parText.Length > parMaxLength ? parText[..parMaxLength] : parText;
    for (int i = 0; i < text.Length && (parStart + i) < parWidth; i++)
    {
      parBuffer[parStart + i, parRow] = text[i];
      parColors[parStart + i, parRow] = parColor;
    }
  }

  /// <summary>
  /// Выполняет RenderBuffer.
  /// </summary>
  private void RenderBuffer(char[,] parBuffer, ConsoleColor[,] parColors, int parWidth, int parHeight)
  {
    var builder = new StringBuilder(parWidth);
    _renderer.SetCursorPosition(0, 0);

    for (int y = 0; y < parHeight; y++)
    {
      _renderer.SetCursorPosition(0, y);
      builder.Clear();
      ConsoleColor? currentColor = null;

      for (int x = 0; x < parWidth; x++)
      {
        var color = parColors[x, y];
        if (currentColor != color)
        {
          FlushLine(builder, currentColor);
          currentColor = color;
        }

        builder.Append(parBuffer[x, y]);
      }

      FlushLine(builder, currentColor);
    }

    _renderer.ResetColor();
  }

  /// <summary>
  /// Выполняет FlushLine.
  /// </summary>
  private void FlushLine(StringBuilder parBuilder, ConsoleColor? parColor)
  {
    if (parBuilder.Length == 0)
    {
      return;
    }

    if (parColor.HasValue)
    {
      _renderer.SetForegroundColor(parColor.Value);
    }

    _renderer.Write(parBuilder.ToString());
    parBuilder.Clear();
  }

  /// <summary>
  /// Выполняет CenterText.
  /// </summary>
  private static string CenterText(string parText, int parWidth)
  {
    if (parWidth <= 0)
    {
      return string.Empty;
    }

    if (parText.Length >= parWidth)
    {
      return parText[..parWidth];
    }

    int left = (parWidth - parText.Length) / 2;
    return new string(' ', left) + parText;
  }

  /// <summary>
  /// Выполняет GetTimerText.
  /// </summary>
  private static string GetTimerText(GameSnapshot parSnapshot, int parWidth)
  {
    string text = "—";

    if (parSnapshot.parHasLevelTimer)
    {
      var seconds = Math.Max(0, Math.Floor(parSnapshot.parLevelTimeRemainingSec));
      text = $"{seconds:0}с";
    }

    return CenterText(text, parWidth);
  }

  /// <summary>
  /// Выполняет DrawOverlay.
  /// </summary>
  private void DrawOverlay(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    int parOffsetY,
    int parWidth,
    int parHeight,
    bool parIsPaused,
    int parCountdownValue)
  {
    if (parCountdownValue > 0)
    {
      string text = parCountdownValue.ToString();
      DrawOverlayText(parBuffer, parColors, parOffsetY, parWidth, parHeight, text, ConsoleColor.Yellow);
    }
  }

  /// <summary>
  /// Выполняет DrawOverlayText.
  /// </summary>
  private void DrawOverlayText(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    int parOffsetY,
    int parWidth,
    int parHeight,
    string parText,
    ConsoleColor parColor)
  {
    int innerWidth = Math.Max(1, parWidth - 2);
    int innerHeight = Math.Max(1, parHeight - 2);
    int centerX = 1 + (innerWidth - parText.Length) / 2;
    int centerY = parOffsetY + 1 + (innerHeight / 2);

    for (int i = 0; i < parText.Length; i++)
    {
      int x = centerX + i;
      if (x <= 0 || x >= parWidth - 1)
      {
        continue;
      }

      parBuffer[x, centerY] = parText[i];
      parColors[x, centerY] = parColor;
    }
  }
}
