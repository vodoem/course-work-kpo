using System;
using CosmicCollector.ConsoleApp.Controllers;
using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Snapshots;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Loop;

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
  public void HandleKey_MoveLeft_EnqueuesCommand()
  {
    var queue = new CommandQueue();
    var controller = CreateController(queue);

    controller.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.A, false, false, false));

    var commands = queue.DrainAll();
    Xunit.Assert.Single(commands);
    Xunit.Assert.IsType<MoveLeftCommand>(commands[0]);
  }

  /// <summary>
  /// Проверяет, что движение отключено на паузе.
  /// </summary>
  [Xunit.Fact]
  public void HandleKey_MoveRight_IgnoredWhenPaused()
  {
    var queue = new CommandQueue();
    var controller = CreateController(queue);
    controller.UpdatePauseState(true);

    controller.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.D, false, false, false));

    Xunit.Assert.Empty(queue.DrainAll());
  }

  /// <summary>
  /// Проверяет, что движение отключено во время отсчёта.
  /// </summary>
  [Xunit.Fact]
  public void HandleKey_MoveLeft_IgnoredDuringCountdown()
  {
    var queue = new CommandQueue();
    var controller = CreateController(queue);
    controller.UpdateCountdown(2);

    controller.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.A, false, false, false));

    Xunit.Assert.Empty(queue.DrainAll());
  }

  /// <summary>
  /// Проверяет, что P/Space отправляет команду паузы.
  /// </summary>
  [Xunit.Fact]
  public void HandleKey_TogglePause_EnqueuesCommand()
  {
    var queue = new CommandQueue();
    var controller = CreateController(queue);

    controller.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.P, false, false, false));

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
    controller.UpdateCountdown(3);

    controller.RenderSnapshot(CreateSnapshot());

    Xunit.Assert.Equal(3, view.LastCountdownValue);
  }

  private static GameScreenController CreateController(CommandQueue parQueue)
  {
    return CreateController(parQueue, new TestGameScreenView());
  }

  private static GameScreenController CreateController(CommandQueue parQueue, TestGameScreenView parView)
  {
    return new GameScreenController(
      parView,
      new EventBus(),
      new TestSnapshotProvider(),
      new TestLoopRunner(),
      parQueue,
      new TestInputReader(),
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

  private sealed class TestInputReader : IConsoleInputReader
  {
    public ConsoleKeyInfo ReadKey()
    {
      return new ConsoleKeyInfo('\0', ConsoleKey.NoName, false, false, false);
    }
  }
}
