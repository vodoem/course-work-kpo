using CosmicCollector.ConsoleApp.Controllers;
using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Snapshots;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет контроллер экрана завершения игры.
/// </summary>
public sealed class GameEndControllerTests
{
  /// <summary>
  /// Проверяет возврат в меню по Enter.
  /// </summary>
  [Xunit.Fact]
  public void Run_Enter_ReturnsToMenu()
  {
    var input = new TestInputReader(new[] { ConsoleKey.Enter });
    var view = new TestGameEndView();
    var controller = new GameEndController(view, input);

    var action = controller.Run(GameEndReason.GameOver, CreateSnapshot(12, 50, true, 30), 2);

    Xunit.Assert.Equal(GameEndAction.ReturnToMenu, action);
    Xunit.Assert.Equal(GameEndReason.GameOver, view.LastReason);
    Xunit.Assert.Equal(12, view.LastScore);
    Xunit.Assert.Equal(50, view.LastEnergy);
    Xunit.Assert.Equal(2, view.LastLevel);
    Xunit.Assert.True(view.LastHasTimer);
  }

  /// <summary>
  /// Проверяет перезапуск по клавише R.
  /// </summary>
  [Xunit.Fact]
  public void Run_R_ReturnsRestart()
  {
    var input = new TestInputReader(new[] { ConsoleKey.R });
    var view = new TestGameEndView();
    var controller = new GameEndController(view, input);

    var action = controller.Run(GameEndReason.LevelCompleted, CreateSnapshot(5, 20, false, 0), 1);

    Xunit.Assert.Equal(GameEndAction.RestartGame, action);
    Xunit.Assert.Equal(GameEndReason.LevelCompleted, view.LastReason);
    Xunit.Assert.False(view.LastHasTimer);
  }

  private static GameSnapshot CreateSnapshot(int parScore, int parEnergy, bool parHasTimer, double parTime)
  {
    var drone = new DroneSnapshot(
      Guid.NewGuid(),
      new Vector2(0, 0),
      new Vector2(0, 0),
      new Aabb(2, 2),
      parEnergy,
      parScore,
      false,
      0);

    return new GameSnapshot(
      false,
      0,
      parHasTimer,
      parTime,
      drone,
      Array.Empty<CrystalSnapshot>(),
      Array.Empty<AsteroidSnapshot>(),
      Array.Empty<BonusSnapshot>(),
      Array.Empty<BlackHoleSnapshot>());
  }

  private sealed class TestInputReader : IConsoleInputReader
  {
    private readonly Queue<ConsoleKey> _keys;

    public TestInputReader(IEnumerable<ConsoleKey> parKeys)
    {
      _keys = new Queue<ConsoleKey>(parKeys);
    }

    public ConsoleKeyInfo ReadKey()
    {
      if (_keys.Count == 0)
      {
        return new ConsoleKeyInfo('\0', ConsoleKey.NoName, false, false, false);
      }

      ConsoleKey key = _keys.Dequeue();
      return new ConsoleKeyInfo('\0', key, false, false, false);
    }
  }

  private sealed class TestGameEndView : IGameEndView
  {
    public GameEndReason? LastReason { get; private set; }
    public int LastScore { get; private set; }
    public int LastEnergy { get; private set; }
    public int LastLevel { get; private set; }
    public bool LastHasTimer { get; private set; }

    public void Render(GameEndReason parReason, GameSnapshot parSnapshot, int parLevel)
    {
      LastReason = parReason;
      LastScore = parSnapshot.parDrone.parScore;
      LastEnergy = parSnapshot.parDrone.parEnergy;
      LastLevel = parLevel;
      LastHasTimer = parSnapshot.parHasLevelTimer;
    }
  }
}
