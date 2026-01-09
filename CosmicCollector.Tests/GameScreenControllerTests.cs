using System;
using CosmicCollector.ConsoleApp.Controllers;
using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Snapshots;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Loop;
using CosmicCollector.Persistence.Records;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет контроллер игрового экрана.
/// </summary>
public sealed class GameScreenControllerTests
{
  /// <summary>
  /// Проверяет, что A/Left отправляет команду движения влево.
  /// </summary>
  [Xunit.Fact]
  public void ApplyInputState_MoveLeft_EnqueuesCommand()
  {
    var queue = new CommandQueue();
    var controller = CreateController(queue);
    controller.StartForTests();

    controller.ApplyInputState(true, false, false);

    var commands = queue.DrainAll();
    Xunit.Assert.Single(commands);
    var command = Xunit.Assert.IsType<SetMoveDirectionCommand>(commands[0]);
    Xunit.Assert.Equal(-1, command.DirectionX);
  }

  /// <summary>
  /// Проверяет, что движение отключено на паузе.
  /// </summary>
  [Xunit.Fact]
  public void ApplyInputState_MoveRight_IgnoredWhenPaused()
  {
    var queue = new CommandQueue();
    var controller = CreateController(queue);
    controller.StartForTests();
    controller.UpdatePauseState(true);

    controller.ApplyInputState(false, true, false);

    Xunit.Assert.Empty(queue.DrainAll());
  }

  /// <summary>
  /// Проверяет, что движение отключено во время отсчёта.
  /// </summary>
  [Xunit.Fact]
  public void ApplyInputState_MoveLeft_IgnoredDuringCountdown()
  {
    var queue = new CommandQueue();
    var controller = CreateController(queue);
    controller.StartForTests();
    controller.UpdateCountdown(2);

    controller.ApplyInputState(true, false, false);

    Xunit.Assert.Empty(queue.DrainAll());
  }

  /// <summary>
  /// Проверяет, что P/Space отправляет команду паузы.
  /// </summary>
  [Xunit.Fact]
  public void ApplyInputState_TogglePause_EnqueuesCommand()
  {
    var queue = new CommandQueue();
    var controller = CreateController(queue);
    controller.StartForTests();

    controller.ApplyInputState(false, false, true);

    var commands = queue.DrainAll();
    Xunit.Assert.Single(commands);
    Xunit.Assert.IsType<TogglePauseCommand>(commands[0]);
  }

  /// <summary>
  /// Проверяет, что пауза обрабатывается даже в состоянии паузы.
  /// </summary>
  [Xunit.Fact]
  public void ApplyInputState_TogglePause_WhenPaused_StillEnqueuesCommand()
  {
    var queue = new CommandQueue();
    var controller = CreateController(queue);
    controller.StartForTests();
    controller.UpdatePauseState(true);

    controller.ApplyInputState(false, false, true);

    var commands = queue.DrainAll();
    Xunit.Assert.Single(commands);
    Xunit.Assert.IsType<TogglePauseCommand>(commands[0]);
  }

  /// <summary>
  /// Проверяет, что отсчёт передаётся в представление.
  /// </summary>
  [Xunit.Fact]
  public void RenderSnapshot_PassesCountdownToView()
  {
    var queue = new CommandQueue();
    var view = new TestGameScreenView();
    var controller = CreateController(queue, view);
    controller.StartForTests();
    controller.UpdateCountdown(3);

    controller.RenderSnapshot(CreateSnapshot());

    Xunit.Assert.Equal(3, view.LastCountdownValue);
  }

  /// <summary>
  /// Проверяет, что после завершения игра не рендерится и ввод отключён.
  /// </summary>
  [Xunit.Fact]
  public void HandleGameEnd_StopsRenderingAndInput()
  {
    var queue = new CommandQueue();
    var view = new TestGameScreenView();
    var controller = CreateController(queue, view);
    controller.StartForTests();
    var snapshot = CreateSnapshot();

    controller.RenderSnapshot(snapshot);
    controller.HandleGameEndForTests(GameEndReason.GameOver, snapshot);
    controller.RenderSnapshot(snapshot);
    controller.ApplyInputState(true, false, false);

    Xunit.Assert.Equal(1, view.RenderCalls);
    Xunit.Assert.Empty(queue.DrainAll());
    Xunit.Assert.False(controller.IsInputEnabled);
  }

  /// <summary>
  /// Проверяет, что публикация GameOver переводит контроллер к выходу.
  /// </summary>
  [Xunit.Fact]
  public void PublishGameOver_SetsExitFlagAndFinalSnapshot()
  {
    var queue = new CommandQueue();
    var controller = CreateController(queue);
    controller.InitializeForTests();

    var bus = new EventBus();
    var controllerWithBus = CreateController(queue, new TestGameScreenView(), bus);
    controllerWithBus.InitializeForTests();
    bus.Publish(new GameOver("Test"));

    Xunit.Assert.True(controllerWithBus.ShouldExitGameScreen);
    Xunit.Assert.NotNull(controllerWithBus.FinalSnapshot);
    Xunit.Assert.Equal(GameEndReason.GameOver, controllerWithBus.EndReason);
  }

  /// <summary>
  /// Проверяет выход в меню через подтверждение паузы.
  /// </summary>
  [Xunit.Fact]
  public void PauseMenu_ConfirmExit_SetsExitFlag()
  {
    var queue = new CommandQueue();
    var controller = CreateController(queue);
    controller.StartForTests();
    controller.OpenPauseMenuForTests();

    controller.HandlePauseMenuInputForTests(new PauseMenuInput(false, true, false, false, false, false), false);
    controller.HandlePauseMenuInputForTests(new PauseMenuInput(false, false, true, false, false, false), false);
    controller.HandlePauseMenuInputForTests(new PauseMenuInput(false, false, false, false, true, false), false);

    Xunit.Assert.True(controller.ShouldExitGameScreen);
    Xunit.Assert.True(controller.ExitToMenuRequested);
    Xunit.Assert.False(controller.IsInputEnabled);
  }

  /// <summary>
  /// Проверяет отсутствие двойного TogglePause при одном нажатии.
  /// </summary>
  [Xunit.Fact]
  public void PauseMenu_DoesNotResume_OnInitialPausePress()
  {
    var queue = new CommandQueue();
    var controller = CreateController(queue);
    controller.StartForTests();
    controller.OpenPauseMenuForTests();

    controller.HandlePauseMenuInputForTests(new PauseMenuInput(false, false, false, false, false, false), true);

    Xunit.Assert.Empty(queue.DrainAll());
  }

  /// <summary>
  /// Проверяет, что повторное нажатие паузы после отпускания продолжает игру.
  /// </summary>
  [Xunit.Fact]
  public void PauseMenu_Resume_OnPauseKeyAfterRelease()
  {
    var queue = new CommandQueue();
    var controller = CreateController(queue);
    controller.StartForTests();
    controller.OpenPauseMenuForTests();

    controller.HandlePauseMenuInputForTests(new PauseMenuInput(false, false, false, false, false, false), true);
    controller.HandlePauseMenuInputForTests(new PauseMenuInput(false, false, false, false, false, false), false);
    controller.HandlePauseMenuInputForTests(new PauseMenuInput(false, false, false, false, false, false), true);

    var commands = queue.DrainAll();
    Xunit.Assert.Single(commands);
    Xunit.Assert.IsType<TogglePauseCommand>(commands[0]);
  }

  private static GameScreenController CreateController(CommandQueue parQueue)
  {
    return CreateController(parQueue, new TestGameScreenView());
  }

  private static GameScreenController CreateController(CommandQueue parQueue, TestGameScreenView parView)
  {
    return CreateController(parQueue, parView, new EventBus());
  }

  private static GameScreenController CreateController(
    CommandQueue parQueue,
    TestGameScreenView parView,
    EventBus parEventBus)
  {
    return new GameScreenController(
      parView,
      parEventBus,
      new TestSnapshotProvider(),
      new TestLoopRunner(),
      parQueue,
      new TestKeyStateProvider(),
      new TestConsoleRenderer(),
      new TestInputReader(),
      new TestRecordsRepository(),
      1);
  }

  private static GameSnapshot CreateSnapshot()
  {
    var drone = new DroneSnapshot(
      Guid.NewGuid(),
      new Vector2(0, 0),
      new Vector2(0, 0),
      new Aabb(2, 2),
      100,
      0,
      false,
      0);

    return new GameSnapshot(
      false,
      0,
      1,
      new LevelGoalsSnapshot(1, 1, 1),
      new LevelProgressSnapshot(0, 0, 0),
      false,
      0,
      drone,
      Array.Empty<CrystalSnapshot>(),
      Array.Empty<AsteroidSnapshot>(),
      Array.Empty<BonusSnapshot>(),
      Array.Empty<BlackHoleSnapshot>());
  }

  private sealed class TestGameScreenView : IGameScreenView
  {
    public int RenderCalls { get; private set; }
    public int LastCountdownValue { get; private set; }

    public void Render(GameSnapshot parSnapshot, int parLevel, bool parIsPaused, int parCountdownValue)
    {
      RenderCalls++;
      LastCountdownValue = parCountdownValue;
    }
  }

  private sealed class TestSnapshotProvider : IGameSnapshotProvider
  {
    public GameSnapshot GetSnapshot()
    {
      return CreateSnapshot();
    }
  }

  private sealed class TestLoopRunner : IGameLoopRunner
  {
    public void Start()
    {
    }

    public void Stop()
    {
    }

    public void Dispose()
    {
    }
  }

  private sealed class TestKeyStateProvider : IKeyStateProvider
  {
    public bool IsKeyDown(ConsoleKey parKey)
    {
      return false;
    }
  }

  private sealed class TestInputReader : IConsoleInputReader
  {
    public ConsoleKeyInfo ReadKey()
    {
      return new ConsoleKeyInfo('\0', ConsoleKey.NoName, false, false, false);
    }

    public void ClearBuffer()
    {
    }
  }

  private sealed class TestConsoleRenderer : IConsoleRenderer
  {
    public void Clear()
    {
    }

    public void WriteLine(string parText)
    {
    }

    public ConsoleKeyInfo ReadKey()
    {
      return new ConsoleKeyInfo('\0', ConsoleKey.NoName, false, false, false);
    }

    public void SetCursorPosition(int parLeft, int parTop)
    {
    }

    public void Write(string parText)
    {
    }

    public void SetForegroundColor(ConsoleColor parColor)
    {
    }

    public void ResetColor()
    {
    }

    public void SetBufferSize(int parWidth, int parHeight)
    {
    }

    public int BufferWidth => 80;

    public int BufferHeight => 25;

    public int WindowWidth => 80;

    public int WindowHeight => 25;
  }

  private sealed class TestRecordsRepository : IRecordsRepository
  {
    public IReadOnlyList<RecordEntry> LoadAll()
    {
      return Array.Empty<RecordEntry>();
    }

    public void SaveAll(IReadOnlyList<RecordEntry> parRecords)
    {
    }

    public void Add(RecordEntry parRecord)
    {
    }
  }
}
