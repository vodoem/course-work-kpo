using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Snapshots;
using System.Text;

namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Представление игрового экрана (консоль).
/// </summary>
public sealed class GameScreenView : IGameScreenView
{
  private const char BorderChar = '#';
  private const char DroneChar = '▲';
  private const char AsteroidChar = '*';
  private const char BlackHoleChar = 'O';
  private const char BonusAcceleratorChar = 'A';
  private const char BonusTimeStabilizerChar = 'T';
  private const char BonusMagnetChar = 'M';
  private const char CrystalChar = '◆';
  private const int HudHeight = 4;
  private const double PixelsPerCell = 12.0;
  private readonly IConsoleRenderer _renderer;
  private readonly WorldBounds _worldBounds;

  /// <summary>
  /// Создаёт представление игрового экрана.
  /// </summary>
  /// <param name="parRenderer">Рендерер консоли.</param>
  /// <param name="parWorldBounds">Границы игрового мира.</param>
  public GameScreenView(IConsoleRenderer parRenderer, WorldBounds parWorldBounds)
  {
    _renderer = parRenderer;
    _worldBounds = parWorldBounds;
  }

  /// <inheritdoc />
  public void Render(GameSnapshot parSnapshot, int parLevel)
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

    DrawBorder(buffer, colors, fieldOffsetY, fieldWidth, fieldHeight);
    DrawDrone(buffer, colors, parSnapshot.parDrone.parPosition, fieldOffsetY, fieldWidth, fieldHeight);
    DrawCrystals(buffer, colors, parSnapshot.parCrystals, fieldOffsetY, fieldWidth, fieldHeight);
    DrawAsteroids(buffer, colors, parSnapshot.parAsteroids, fieldOffsetY, fieldWidth, fieldHeight);
    DrawBonuses(buffer, colors, parSnapshot.parBonuses, fieldOffsetY, fieldWidth, fieldHeight);
    DrawBlackHoles(buffer, colors, parSnapshot.parBlackHoles, fieldOffsetY, fieldWidth, fieldHeight);

    RenderBuffer(buffer, colors, width, height);
  }

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

  private void DrawDrone(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    Vector2 parPosition,
    int parOffsetY,
    int parWidth,
    int parHeight)
  {
    if (TryMapToGrid(parPosition, parOffsetY, parWidth, parHeight, out var coords))
    {
      parBuffer[coords.X, coords.Y] = DroneChar;
      parColors[coords.X, coords.Y] = ConsoleColor.White;
    }
  }

  private void DrawCrystals(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    IReadOnlyList<CrystalSnapshot> parCrystals,
    int parOffsetY,
    int parWidth,
    int parHeight)
  {
    foreach (var crystal in parCrystals)
    {
      if (!TryMapToGrid(crystal.parPosition, parOffsetY, parWidth, parHeight, out var coords))
      {
        continue;
      }

      parBuffer[coords.X, coords.Y] = CrystalChar;
      parColors[coords.X, coords.Y] = crystal.parType switch
      {
        CrystalType.Blue => ConsoleColor.Cyan,
        CrystalType.Green => ConsoleColor.Green,
        CrystalType.Red => ConsoleColor.Red,
        _ => ConsoleColor.Cyan
      };
    }
  }

  private void DrawAsteroids(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    IReadOnlyList<AsteroidSnapshot> parAsteroids,
    int parOffsetY,
    int parWidth,
    int parHeight)
  {
    foreach (var asteroid in parAsteroids)
    {
      if (TryMapToGrid(asteroid.parPosition, parOffsetY, parWidth, parHeight, out var coords))
      {
        parBuffer[coords.X, coords.Y] = AsteroidChar;
        parColors[coords.X, coords.Y] = ConsoleColor.DarkGray;
      }
    }
  }

  private void DrawBonuses(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    IReadOnlyList<BonusSnapshot> parBonuses,
    int parOffsetY,
    int parWidth,
    int parHeight)
  {
    foreach (var bonus in parBonuses)
    {
      if (!TryMapToGrid(bonus.parPosition, parOffsetY, parWidth, parHeight, out var coords))
      {
        continue;
      }

      parBuffer[coords.X, coords.Y] = bonus.parType switch
      {
        BonusType.Accelerator => BonusAcceleratorChar,
        BonusType.TimeStabilizer => BonusTimeStabilizerChar,
        BonusType.Magnet => BonusMagnetChar,
        _ => BonusAcceleratorChar
      };
      parColors[coords.X, coords.Y] = bonus.parType switch
      {
        BonusType.Accelerator => ConsoleColor.Yellow,
        BonusType.TimeStabilizer => ConsoleColor.White,
        BonusType.Magnet => ConsoleColor.DarkGreen,
        _ => ConsoleColor.Yellow
      };
    }
  }

  private void DrawBlackHoles(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    IReadOnlyList<BlackHoleSnapshot> parBlackHoles,
    int parOffsetY,
    int parWidth,
    int parHeight)
  {
    foreach (var blackHole in parBlackHoles)
    {
      if (TryMapToGrid(blackHole.parPosition, parOffsetY, parWidth, parHeight, out var coords))
      {
        parBuffer[coords.X, coords.Y] = BlackHoleChar;
        parColors[coords.X, coords.Y] = ConsoleColor.Magenta;
      }
    }
  }

  private bool TryMapToGrid(
    Vector2 parPosition,
    int parOffsetY,
    int parWidth,
    int parHeight,
    out (int X, int Y) parCoords)
  {
    double worldWidth = _worldBounds.Right - _worldBounds.Left;
    double worldHeight = _worldBounds.Bottom - _worldBounds.Top;

    if (worldWidth <= 0 || worldHeight <= 0)
    {
      parCoords = (0, 0);
      return false;
    }

    int gridX = MapWorldToCellX(parPosition.X, parWidth);
    int gridY = MapWorldToCellY(parPosition.Y, parOffsetY, parHeight);

    if (gridX < 1 || gridX > parWidth - 2 || gridY < parOffsetY + 1 || gridY > parOffsetY + parHeight - 2)
    {
      parCoords = (0, 0);
      return false;
    }

    parCoords = (gridX, gridY);
    return true;
  }

  private int MapWorldToCellX(double parWorldX, int parWidth)
  {
    double worldWidth = _worldBounds.Right - _worldBounds.Left;
    if (worldWidth <= 0)
    {
      return 1;
    }

    int innerWidth = Math.Max(1, parWidth - 2);
    double normalizedX = (parWorldX - _worldBounds.Left) / worldWidth;
    normalizedX = Math.Clamp(normalizedX, 0, 1);
    int cellX = (int)Math.Round((normalizedX * worldWidth) / PixelsPerCell);
    int clamped = Math.Clamp(cellX, 0, innerWidth - 1);
    return 1 + clamped;
  }

  private int MapWorldToCellY(double parWorldY, int parOffsetY, int parHeight)
  {
    int innerHeight = Math.Max(1, parHeight - 2);
    double worldHeight = _worldBounds.Bottom - _worldBounds.Top;
    if (worldHeight <= 0)
    {
      return parOffsetY + 1;
    }

    double distanceFromBottom = _worldBounds.Bottom - parWorldY;
    double normalizedY = distanceFromBottom / worldHeight;
    normalizedY = Math.Clamp(normalizedY, 0, 1);
    int cellY = (int)Math.Round((normalizedY * worldHeight) / PixelsPerCell);
    int clamped = Math.Clamp(cellY, 0, innerHeight - 1);
    return parOffsetY + 1 + clamped;
  }

  private void DrawHud(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    int parWidth,
    GameSnapshot parSnapshot,
    int parLevel)
  {
    string goals = "Цели: кристаллы всех типов | Энергия: —";
    string timerHeader = "ТАЙМЕР";
    string timerValue = $"{parSnapshot.parTickNo / 60.0:0.0}с";
    string progress = $"Уровень: {parLevel} | Энергия: {parSnapshot.parDrone.parEnergy} | Очки: {parSnapshot.parDrone.parScore}";
    WriteHudLine(parBuffer, parColors, 0, parWidth, goals, timerHeader, progress);

    string collected = "Собрано: B=— G=— R=— | Энергия: —";
    string timerLine = CenterText(timerValue, parWidth / 3);
    string progressLine = $"На поле: B={parSnapshot.parCrystals.Count(c => c.parType == CrystalType.Blue)} " +
      $"G={parSnapshot.parCrystals.Count(c => c.parType == CrystalType.Green)} " +
      $"R={parSnapshot.parCrystals.Count(c => c.parType == CrystalType.Red)}";
    WriteHudLine(parBuffer, parColors, 1, parWidth, collected, timerLine, progressLine);

    string bonusesSummary = $"Бонусы: A={parSnapshot.parBonuses.Count(b => b.parType == BonusType.Accelerator)} " +
      $"T={parSnapshot.parBonuses.Count(b => b.parType == BonusType.TimeStabilizer)} " +
      $"M={parSnapshot.parBonuses.Count(b => b.parType == BonusType.Magnet)}";
    WriteText(parBuffer, parColors, 2, parWidth, bonusesSummary, 0, parWidth, ConsoleColor.Yellow);

    string legend = $"Легенда: {DroneChar} дрон {CrystalChar} кристалл {AsteroidChar} астероид {BlackHoleChar} дыра " +
      $"{BonusAcceleratorChar} ускор {BonusTimeStabilizerChar} стаб {BonusMagnetChar} магнит";
    WriteText(parBuffer, parColors, 3, parWidth, legend, 0, parWidth, ConsoleColor.Gray);
  }

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
}
