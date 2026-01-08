using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Snapshots;

namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Представление игрового экрана (консоль).
/// </summary>
public sealed class GameScreenView : IGameScreenView
{
  private const char BorderChar = '#';
  private const char DroneChar = 'D';
  private const char AsteroidChar = 'A';
  private const char BlackHoleChar = 'O';
  private const char BonusAcceleratorChar = 'U';
  private const char BonusTimeStabilizerChar = 'T';
  private const char BonusMagnetChar = 'M';
  private const char CrystalBlueChar = 'B';
  private const char CrystalGreenChar = 'G';
  private const char CrystalRedChar = 'R';
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
    _renderer.Clear();

    double secondsElapsed = parSnapshot.parTickNo / 60.0;
    string bonusesSummary = BuildBonusesSummary(parSnapshot);

    _renderer.WriteLine($"Очки: {parSnapshot.parDrone.parScore} | Энергия: {parSnapshot.parDrone.parEnergy} | " +
      $"Уровень: {parLevel} | Таймер: {secondsElapsed:0.0}с");
    _renderer.WriteLine($"Бонусы: {bonusesSummary}");
    _renderer.WriteLine(string.Empty);

    int fieldWidth = Math.Max(10, _renderer.BufferWidth - 2);
    int fieldHeight = Math.Max(6, _renderer.BufferHeight - 6);
    char[,] buffer = CreateBuffer(fieldWidth, fieldHeight);

    DrawBorder(buffer, fieldWidth, fieldHeight);
    DrawDrone(buffer, parSnapshot.parDrone.parPosition, fieldWidth, fieldHeight);
    DrawCrystals(buffer, parSnapshot.parCrystals, fieldWidth, fieldHeight);
    DrawAsteroids(buffer, parSnapshot.parAsteroids, fieldWidth, fieldHeight);
    DrawBonuses(buffer, parSnapshot.parBonuses, fieldWidth, fieldHeight);
    DrawBlackHoles(buffer, parSnapshot.parBlackHoles, fieldWidth, fieldHeight);

    for (int y = 0; y < fieldHeight; y++)
    {
      var lineChars = new char[fieldWidth];
      for (int x = 0; x < fieldWidth; x++)
      {
        lineChars[x] = buffer[x, y];
      }

      _renderer.WriteLine(new string(lineChars));
    }
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

  private void DrawBorder(char[,] parBuffer, int parWidth, int parHeight)
  {
    for (int x = 0; x < parWidth; x++)
    {
      parBuffer[x, 0] = BorderChar;
      parBuffer[x, parHeight - 1] = BorderChar;
    }

    for (int y = 0; y < parHeight; y++)
    {
      parBuffer[0, y] = BorderChar;
      parBuffer[parWidth - 1, y] = BorderChar;
    }
  }

  private void DrawDrone(char[,] parBuffer, Vector2 parPosition, int parWidth, int parHeight)
  {
    if (TryMapToGrid(parPosition, parWidth, parHeight, out var coords))
    {
      parBuffer[coords.X, coords.Y] = DroneChar;
    }
  }

  private void DrawCrystals(
    char[,] parBuffer,
    IReadOnlyList<CrystalSnapshot> parCrystals,
    int parWidth,
    int parHeight)
  {
    foreach (var crystal in parCrystals)
    {
      if (!TryMapToGrid(crystal.parPosition, parWidth, parHeight, out var coords))
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
    int parWidth,
    int parHeight)
  {
    foreach (var asteroid in parAsteroids)
    {
      if (TryMapToGrid(asteroid.parPosition, parWidth, parHeight, out var coords))
      {
        parBuffer[coords.X, coords.Y] = AsteroidChar;
      }
    }
  }

  private void DrawBonuses(
    char[,] parBuffer,
    IReadOnlyList<BonusSnapshot> parBonuses,
    int parWidth,
    int parHeight)
  {
    foreach (var bonus in parBonuses)
    {
      if (!TryMapToGrid(bonus.parPosition, parWidth, parHeight, out var coords))
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
    int parWidth,
    int parHeight)
  {
    foreach (var blackHole in parBlackHoles)
    {
      if (TryMapToGrid(blackHole.parPosition, parWidth, parHeight, out var coords))
      {
        parBuffer[coords.X, coords.Y] = BlackHoleChar;
      }
    }
  }

  private bool TryMapToGrid(Vector2 parPosition, int parWidth, int parHeight, out (int X, int Y) parCoords)
  {
    double worldWidth = _worldBounds.Right - _worldBounds.Left;
    double worldHeight = _worldBounds.Bottom - _worldBounds.Top;

    if (worldWidth <= 0 || worldHeight <= 0)
    {
      parCoords = (0, 0);
      return false;
    }

    double normalizedX = (parPosition.X - _worldBounds.Left) / worldWidth;
    double normalizedY = (parPosition.Y - _worldBounds.Top) / worldHeight;
    int gridX = 1 + (int)Math.Round(normalizedX * (parWidth - 3));
    int gridY = 1 + (int)Math.Round(normalizedY * (parHeight - 3));

    if (gridX < 1 || gridX > parWidth - 2 || gridY < 1 || gridY > parHeight - 2)
    {
      parCoords = (0, 0);
      return false;
    }

    parCoords = (gridX, gridY);
    return true;
  }

  private static string BuildBonusesSummary(GameSnapshot parSnapshot)
  {
    int accelerator = parSnapshot.parBonuses.Count(bonus => bonus.parType == BonusType.Accelerator);
    int timeStabilizer = parSnapshot.parBonuses.Count(bonus => bonus.parType == BonusType.TimeStabilizer);
    int magnet = parSnapshot.parBonuses.Count(bonus => bonus.parType == BonusType.Magnet);

    return $"ускоритель={accelerator}, стабилизатор={timeStabilizer}, магнит={magnet}";
  }
}
