using CosmicCollector.ConsoleApp.Controllers;
using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Snapshots;
using CosmicCollector.Persistence.Records;

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
    var input = new TestInputReader(new[]
    {
      CreateKey(ConsoleKey.Enter)
    });
    var view = new TestGameEndView();
    var controller = new GameEndController(view, input, CreateFullRepository());

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
    var input = new TestInputReader(new[]
    {
      CreateKey(ConsoleKey.R)
    });
    var view = new TestGameEndView();
    var controller = new GameEndController(view, input, CreateFullRepository());

    var action = controller.Run(GameEndReason.LevelCompleted, CreateSnapshot(5, 20, false, 0), 1);

    Xunit.Assert.Equal(GameEndAction.RestartGame, action);
    Xunit.Assert.Equal(GameEndReason.LevelCompleted, view.LastReason);
    Xunit.Assert.False(view.LastHasTimer);
  }

  /// <summary>
  /// Проверяет сохранение рекорда и возврат в меню.
  /// </summary>
  [Xunit.Fact]
  public void Run_HighScore_SavesRecord_AndReturnsToMenu()
  {
    var input = new TestInputReader(new[]
    {
      CreateCharKey('A'),
      CreateCharKey('1'),
      CreateKey(ConsoleKey.Enter),
      CreateKey(ConsoleKey.Enter)
    });
    var view = new TestGameEndView();
    var repository = new TestRecordsRepository(Array.Empty<RecordEntry>());
    var controller = new GameEndController(view, input, repository);

    var action = controller.Run(GameEndReason.GameOver, CreateSnapshot(12, 50, true, 30), 2);

    Xunit.Assert.Equal(GameEndAction.ReturnToMenu, action);
    Xunit.Assert.True(view.LastIsSaved);
    Xunit.Assert.Single(repository.AddedRecords);
    Xunit.Assert.Equal("A1", repository.AddedRecords[0].parPlayerName);
  }

  /// <summary>
  /// Проверяет, что недопустимые символы в имени игнорируются.
  /// </summary>
  [Xunit.Fact]
  public void Run_HighScore_IgnoresInvalidCharacters()
  {
    var input = new TestInputReader(new[]
    {
      CreateCharKey('@'),
      CreateCharKey('B'),
      CreateKey(ConsoleKey.Enter),
      CreateKey(ConsoleKey.Enter)
    });
    var view = new TestGameEndView();
    var repository = new TestRecordsRepository(Array.Empty<RecordEntry>());
    var controller = new GameEndController(view, input, repository);

    controller.Run(GameEndReason.GameOver, CreateSnapshot(12, 50, true, 30), 2);

    Xunit.Assert.Single(repository.AddedRecords);
    Xunit.Assert.Equal("B", repository.AddedRecords[0].parPlayerName);
  }

  /// <summary>
  /// Проверяет, что пустое имя не сохраняется.
  /// </summary>
  [Xunit.Fact]
  public void Run_HighScore_RejectsEmptyName()
  {
    var input = new TestInputReader(new[]
    {
      CreateKey(ConsoleKey.Enter),
      CreateCharKey('Z'),
      CreateKey(ConsoleKey.Enter),
      CreateKey(ConsoleKey.Enter)
    });
    var view = new TestGameEndView();
    var repository = new TestRecordsRepository(Array.Empty<RecordEntry>());
    var controller = new GameEndController(view, input, repository);

    controller.Run(GameEndReason.GameOver, CreateSnapshot(12, 50, true, 30), 2);

    Xunit.Assert.Single(repository.AddedRecords);
    Xunit.Assert.Equal("Z", repository.AddedRecords[0].parPlayerName);
  }

  /// <summary>
  /// Проверяет, что имя ограничено 16 символами.
  /// </summary>
  [Xunit.Fact]
  public void Run_HighScore_LimitsNameLength()
  {
    var inputKeys = new List<ConsoleKeyInfo>();
    for (int i = 0; i < 20; i++)
    {
      inputKeys.Add(CreateCharKey('A'));
    }
    inputKeys.Add(CreateKey(ConsoleKey.Enter));
    inputKeys.Add(CreateKey(ConsoleKey.Enter));

    var input = new TestInputReader(inputKeys);
    var view = new TestGameEndView();
    var repository = new TestRecordsRepository(Array.Empty<RecordEntry>());
    var controller = new GameEndController(view, input, repository);

    controller.Run(GameEndReason.GameOver, CreateSnapshot(12, 50, true, 30), 2);

    Xunit.Assert.Single(repository.AddedRecords);
    Xunit.Assert.Equal(16, repository.AddedRecords[0].parPlayerName.Length);
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
      1,
      new LevelGoalsSnapshot(1, 1, 1),
      new LevelProgressSnapshot(0, 0, 0),
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
    private readonly Queue<ConsoleKeyInfo> _keys;

    public TestInputReader(IEnumerable<ConsoleKeyInfo> parKeys)
    {
      _keys = new Queue<ConsoleKeyInfo>(parKeys);
    }

    public ConsoleKeyInfo ReadKey()
    {
      if (_keys.Count == 0)
      {
        return new ConsoleKeyInfo('\0', ConsoleKey.NoName, false, false, false);
      }

      return _keys.Dequeue();
    }

    public void ClearBuffer()
    {
    }
  }

  private sealed class TestGameEndView : IGameEndView
  {
    public GameEndReason? LastReason { get; private set; }
    public int LastScore { get; private set; }
    public int LastEnergy { get; private set; }
    public int LastLevel { get; private set; }
    public bool LastHasTimer { get; private set; }
    public bool LastIsHighScore { get; private set; }
    public string LastPlayerName { get; private set; } = string.Empty;
    public bool LastIsSaved { get; private set; }
    public int TopRecordsCount { get; private set; }

    public void Render(
      GameEndReason parReason,
      GameSnapshot parSnapshot,
      int parLevel,
      bool parIsHighScore,
      string parPlayerName,
      bool parIsSaved,
      IReadOnlyList<RecordEntry> parTopRecords)
    {
      LastReason = parReason;
      LastScore = parSnapshot.parDrone.parScore;
      LastEnergy = parSnapshot.parDrone.parEnergy;
      LastLevel = parLevel;
      LastHasTimer = parSnapshot.parHasLevelTimer;
      LastIsHighScore = parIsHighScore;
      LastPlayerName = parPlayerName;
      LastIsSaved = parIsSaved;
      TopRecordsCount = parTopRecords.Count;
    }
  }

  private sealed class TestRecordsRepository : IRecordsRepository
  {
    private readonly IReadOnlyList<RecordEntry> _records;
    public List<RecordEntry> AddedRecords { get; } = new();

    public TestRecordsRepository(IReadOnlyList<RecordEntry> parRecords)
    {
      _records = parRecords;
    }

    public IReadOnlyList<RecordEntry> LoadAll()
    {
      return _records;
    }

    public void SaveAll(IReadOnlyList<RecordEntry> parRecords)
    {
    }

    public void Add(RecordEntry parRecord)
    {
      AddedRecords.Add(parRecord);
    }
  }

  private static IRecordsRepository CreateFullRepository()
  {
    var records = new List<RecordEntry>();
    for (int i = 0; i < 10; i++)
    {
      records.Add(new RecordEntry($"Player{i}", 1000 - i, 1, "2024-01-01T00:00:00Z"));
    }

    return new TestRecordsRepository(records);
  }

  private static ConsoleKeyInfo CreateKey(ConsoleKey parKey)
  {
    return new ConsoleKeyInfo('\0', parKey, false, false, false);
  }

  private static ConsoleKeyInfo CreateCharKey(char parChar)
  {
    var key = char.ToUpperInvariant(parChar) switch
    {
      >= 'A' and <= 'Z' => (ConsoleKey)((int)ConsoleKey.A + (char.ToUpperInvariant(parChar) - 'A')),
      >= '0' and <= '9' => (ConsoleKey)((int)ConsoleKey.D0 + (parChar - '0')),
      _ => ConsoleKey.NoName
    };

    return new ConsoleKeyInfo(parChar, key, false, false, false);
  }
}
