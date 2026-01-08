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
  private const char BorderChar = '█';
  private const char DroneChar = '▲';
  private const char AsteroidChar = '☄';
  private const char BlackHoleChar = '●';
  private const char BonusAcceleratorChar = '⚡';
  private const char BonusTimeStabilizerChar = '⌛';
  private const char BonusMagnetChar = '⦿';
  private const char CrystalBlueChar = '♦';
  private const char CrystalGreenChar = '◊';
  private const char CrystalRedChar = '✦';
  private const double VisualScaleX = 1.0;
  private const double VisualScaleY = 0.5;
  private const int HudHeight = 5;
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
    int width = Math.Max(20, _renderer.BufferWidth);
    int height = Math.Max(10, _renderer.BufferHeight);
    char[,] buffer = CreateBuffer(width, height);

    DrawHud(buffer, width, parSnapshot, parLevel);

    int fieldOffsetY = HudHeight;
    int fieldWidth = width;
    int fieldHeight = Math.Max(6, height - fieldOffsetY);

    DrawBorder(buffer, fieldOffsetY, fieldWidth, fieldHeight);
    DrawDrone(buffer, parSnapshot.parDrone.parPosition, fieldOffsetY, fieldWidth, fieldHeight);
    DrawCrystals(buffer, parSnapshot.parCrystals, fieldOffsetY, fieldWidth, fieldHeight);
    DrawAsteroids(buffer, parSnapshot.parAsteroids, fieldOffsetY, fieldWidth, fieldHeight);
    DrawBonuses(buffer, parSnapshot.parBonuses, fieldOffsetY, fieldWidth, fieldHeight);
    DrawBlackHoles(buffer, parSnapshot.parBlackHoles, fieldOffsetY, fieldWidth, fieldHeight);

    RenderBuffer(buffer, width, height);
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

  private void DrawBorder(char[,] parBuffer, int parOffsetY, int parWidth, int parHeight)
  {
    for (int x = 0; x < parWidth; x++)
    {
      parBuffer[x, parOffsetY] = BorderChar;
      parBuffer[x, parOffsetY + parHeight - 1] = BorderChar;
    }

    for (int y = 0; y < parHeight; y++)
    {
      parBuffer[0, parOffsetY + y] = BorderChar;
      parBuffer[parWidth - 1, parOffsetY + y] = BorderChar;
    }
  }

  private void DrawDrone(char[,] parBuffer, Vector2 parPosition, int parOffsetY, int parWidth, int parHeight)
  {
    if (TryMapToGrid(parPosition, parOffsetY, parWidth, parHeight, out var coords))
    {
      parBuffer[coords.X, coords.Y] = DroneChar;
    }
  }

  private void DrawCrystals(
    char[,] parBuffer,
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

      parBuffer[coords.X, coords.Y] = crystal.parType switch
      {
        CrystalType.Blue => CrystalBlueChar,
        CrystalType.Green => CrystalGreenChar,
        CrystalType.Red => CrystalRedChar,
        _ => CrystalBlueChar
      };
    }
  }

  private void DrawAsteroids(
    char[,] parBuffer,
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
      }
    }
  }

  private void DrawBonuses(
    char[,] parBuffer,
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
    }
  }

  private void DrawBlackHoles(
    char[,] parBuffer,
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

    double scaledWidth = worldWidth * VisualScaleX;
    double scaledHeight = worldHeight * VisualScaleY;
    double normalizedX = (parPosition.X - _worldBounds.Left) / scaledWidth;
    double normalizedY = (parPosition.Y - _worldBounds.Top) / scaledHeight;
    normalizedX = Math.Clamp(normalizedX, 0, 1);
    normalizedY = Math.Clamp(normalizedY, 0, 1);
    int gridX = MapToCellX(normalizedX, parWidth);
    int gridY = MapToCellY(normalizedY, parOffsetY, parHeight);

    if (gridX < 1 || gridX > parWidth - 2 || gridY < parOffsetY + 1 || gridY > parOffsetY + parHeight - 2)
    {
      parCoords = (0, 0);
      return false;
    }

    parCoords = (gridX, gridY);
    return true;
  }

  private int MapToCellX(double parNormalizedX, int parWidth)
  {
    int innerWidth = Math.Max(1, parWidth - 2);
    return 1 + (int)Math.Round(parNormalizedX * (innerWidth - 1));
  }

  private int MapToCellY(double parNormalizedY, int parOffsetY, int parHeight)
  {
    int innerHeight = Math.Max(1, parHeight - 2);
    int mapped = (int)Math.Round(parNormalizedY * (innerHeight - 1));
    int inverted = (innerHeight - 1) - mapped;
    return parOffsetY + 1 + inverted;
  }

  private bool ValidateDronePlacement(Vector2 parPosition, int parWidth)
  {
    int fieldHeight = Math.Max(6, _renderer.BufferHeight - HudHeight);
    int fieldOffsetY = HudHeight;
    double worldWidth = _worldBounds.Right - _worldBounds.Left;
    double worldHeight = _worldBounds.Bottom - _worldBounds.Top;

    if (worldWidth <= 0 || worldHeight <= 0)
    {
      return false;
    }

    double scaledWidth = worldWidth * VisualScaleX;
    double scaledHeight = worldHeight * VisualScaleY;
    double normalizedX = (parPosition.X - _worldBounds.Left) / scaledWidth;
    double normalizedY = (parPosition.Y - _worldBounds.Top) / scaledHeight;
    normalizedX = Math.Clamp(normalizedX, 0, 1);
    normalizedY = Math.Clamp(normalizedY, 0, 1);
    int gridX = MapToCellX(normalizedX, parWidth);
    int gridY = MapToCellY(normalizedY, fieldOffsetY, fieldHeight);
    int expectedX = parWidth / 2;
    int expectedY = fieldOffsetY + fieldHeight - 2;

    return Math.Abs(gridX - expectedX) <= 1 && Math.Abs(gridY - expectedY) <= 1;
  }

  private void DrawHud(char[,] parBuffer, int parWidth, GameSnapshot parSnapshot, int parLevel)
  {
    string goals = "Цели: —";
    string timer = $"Таймер: {parSnapshot.parTickNo / 60.0:0.0}с";
    string progress = $"Уровень: {parLevel} | Энергия: {parSnapshot.parDrone.parEnergy} | Очки: {parSnapshot.parDrone.parScore}";
    WriteHudLine(parBuffer, 0, parWidth, goals, timer, progress);

    string crystals = $"Кристаллы (на поле): B={parSnapshot.parCrystals.Count(c => c.parType == CrystalType.Blue)} " +
      $"G={parSnapshot.parCrystals.Count(c => c.parType == CrystalType.Green)} " +
      $"R={parSnapshot.parCrystals.Count(c => c.parType == CrystalType.Red)}";
    WriteText(parBuffer, 1, parWidth, crystals);

    string bonusesSummary = BuildBonusesSummary(parSnapshot);
    WriteText(parBuffer, 2, parWidth, $"Бонусы (на поле): {bonusesSummary}");

    string legend = $"Легенда: {DroneChar}=дрон {CrystalBlueChar}/{CrystalGreenChar}/{CrystalRedChar}=кристаллы " +
      $"{AsteroidChar}=астероид {BlackHoleChar}=чёрная дыра {BonusAcceleratorChar}=ускоритель " +
      $"{BonusTimeStabilizerChar}=стабилизатор {BonusMagnetChar}=магнит";
    WriteText(parBuffer, 3, parWidth, legend);

    string droneCheck = ValidateDronePlacement(parSnapshot.parDrone.parPosition, parWidth) ?
      "Проверка: дрон внизу по центру" :
      "Проверка: дрон смещён";
    WriteText(parBuffer, 4, parWidth, droneCheck);
  }

  private void WriteHudLine(
    char[,] parBuffer,
    int parRow,
    int parWidth,
    string parLeft,
    string parCenter,
    string parRight)
  {
    int third = parWidth / 3;
    WriteText(parBuffer, parRow, parWidth, parLeft, 0, third);
    WriteText(parBuffer, parRow, parWidth, parCenter, third, third);
    WriteText(parBuffer, parRow, parWidth, parRight, third * 2, parWidth - (third * 2));
  }

  private void WriteText(char[,] parBuffer, int parRow, int parWidth, string parText)
  {
    WriteText(parBuffer, parRow, parWidth, parText, 0, parWidth);
  }

  private void WriteText(char[,] parBuffer, int parRow, int parWidth, string parText, int parStart, int parMaxLength)
  {
    if (parRow < 0 || parRow >= parBuffer.GetLength(1))
    {
      return;
    }

    string text = parText.Length > parMaxLength ? parText[..parMaxLength] : parText;
    for (int i = 0; i < text.Length && (parStart + i) < parWidth; i++)
    {
      parBuffer[parStart + i, parRow] = text[i];
    }
  }

  private void RenderBuffer(char[,] parBuffer, int parWidth, int parHeight)
  {
    var builder = new StringBuilder(parHeight * (parWidth + 1));

    for (int y = 0; y < parHeight; y++)
    {
      for (int x = 0; x < parWidth; x++)
      {
        builder.Append(parBuffer[x, y]);
      }

      if (y < parHeight - 1)
      {
        builder.Append('\n');
      }
    }

    _renderer.SetCursorPosition(0, 0);
    _renderer.Write(builder.ToString());
  }

  private static string BuildBonusesSummary(GameSnapshot parSnapshot)
  {
    int accelerator = parSnapshot.parBonuses.Count(bonus => bonus.parType == BonusType.Accelerator);
    int timeStabilizer = parSnapshot.parBonuses.Count(bonus => bonus.parType == BonusType.TimeStabilizer);
    int magnet = parSnapshot.parBonuses.Count(bonus => bonus.parType == BonusType.Magnet);

    return $"ускоритель={accelerator}, стабилизатор={timeStabilizer}, магнит={magnet}";
  }
}
