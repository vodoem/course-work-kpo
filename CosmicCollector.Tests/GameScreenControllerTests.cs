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
  public void ApplyInputState_MoveLeft_EnqueuesCommand()
  {
    var queue = new CommandQueue();
    var controller = CreateController(queue);

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
      new TestKeyStateProvider(),
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

  private sealed class TestKeyStateProvider : IKeyStateProvider
  {
    public bool IsKeyDown(ConsoleKey parKey)
    {
      return false;
    }
  }
}
