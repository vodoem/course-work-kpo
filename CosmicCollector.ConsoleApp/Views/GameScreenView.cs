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
  private const char CrystalChar = '♦';
  private const int HudHeight = RenderConfig.HudHeight;
  private const double PixelsPerCell = RenderConfig.PixelsPerCell;
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
    int innerWidth = Math.Max(1, fieldWidth - 2);
    int innerHeight = Math.Max(1, fieldHeight - 2);
    var mapper = new WorldToScreenMapper(_worldBounds, innerWidth, innerHeight, PixelsPerCell);

    DrawBorder(buffer, colors, fieldOffsetY, fieldWidth, fieldHeight);
    DrawDrone(buffer, colors, mapper, parSnapshot.parDrone.parPosition, fieldOffsetY, fieldWidth, fieldHeight);
    DrawCrystals(buffer, colors, mapper, parSnapshot.parCrystals, fieldOffsetY, fieldWidth, fieldHeight);
    DrawAsteroids(buffer, colors, mapper, parSnapshot.parAsteroids, fieldOffsetY, fieldWidth, fieldHeight);
    DrawBonuses(buffer, colors, mapper, parSnapshot.parBonuses, fieldOffsetY, fieldWidth, fieldHeight);
    DrawBlackHoles(buffer, colors, mapper, parSnapshot.parBlackHoles, fieldOffsetY, fieldWidth, fieldHeight);

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
    WorldToScreenMapper parMapper,
    Vector2 parPosition,
    int parOffsetY,
    int parWidth,
    int parHeight)
  {
    if (TryMapToGrid(parMapper, parPosition, parOffsetY, parWidth, parHeight, out var coords))
    {
      parBuffer[coords.X, coords.Y] = DroneChar;
      parColors[coords.X, coords.Y] = ConsoleColor.White;
    }
  }

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
      if (!TryMapToGrid(parMapper, crystal.parPosition, parOffsetY, parWidth, parHeight, out var coords))
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
    WorldToScreenMapper parMapper,
    IReadOnlyList<AsteroidSnapshot> parAsteroids,
    int parOffsetY,
    int parWidth,
    int parHeight)
  {
    foreach (var asteroid in parAsteroids)
    {
      if (TryMapToGrid(parMapper, asteroid.parPosition, parOffsetY, parWidth, parHeight, out var coords))
      {
        parBuffer[coords.X, coords.Y] = AsteroidChar;
        parColors[coords.X, coords.Y] = ConsoleColor.DarkGray;
      }
    }
  }

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
      if (!TryMapToGrid(parMapper, bonus.parPosition, parOffsetY, parWidth, parHeight, out var coords))
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
    WorldToScreenMapper parMapper,
    IReadOnlyList<BlackHoleSnapshot> parBlackHoles,
    int parOffsetY,
    int parWidth,
    int parHeight)
  {
    foreach (var blackHole in parBlackHoles)
    {
      if (TryMapToGrid(parMapper, blackHole.parPosition, parOffsetY, parWidth, parHeight, out var coords))
      {
        parBuffer[coords.X, coords.Y] = BlackHoleChar;
        parColors[coords.X, coords.Y] = ConsoleColor.Magenta;
      }
    }
  }

  private bool TryMapToGrid(
    WorldToScreenMapper parMapper,
    Vector2 parPosition,
    int parOffsetY,
    int parWidth,
    int parHeight,
    out (int X, int Y) parCoords)
  {
    int gridX = 1 + parMapper.MapX(parPosition.X);
    int gridY = parOffsetY + 1 + parMapper.MapY(parPosition.Y);

    if (gridX < 1 || gridX > parWidth - 2 || gridY < parOffsetY + 1 || gridY > parOffsetY + parHeight - 2)
    {
      parCoords = (0, 0);
      return false;
    }

    parCoords = (gridX, gridY);
    return true;
  }

  private void DrawHud(
    char[,] parBuffer,
    ConsoleColor[,] parColors,
    int parWidth,
    GameSnapshot parSnapshot,
    int parLevel)
  {
    string goals = "Цели: B=— G=— R=— | Цель энергии: —";
    string timerHeader = "Таймер";
    string progress = $"Уровень: {parLevel} | Энергия: {parSnapshot.parDrone.parEnergy} | Очки: {parSnapshot.parDrone.parScore}";
    WriteHudLine(parBuffer, parColors, 0, parWidth, goals, timerHeader, progress);

    string collected = "Собрано: B=— G=— R=—";
    string timerLine = CenterText($"{parSnapshot.parTickNo / 60.0:0.0}с", parWidth / 3);
    string progressLine = "Прогресс: B=— G=— R=—";
    WriteHudLine(parBuffer, parColors, 1, parWidth, collected, timerLine, progressLine);
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
