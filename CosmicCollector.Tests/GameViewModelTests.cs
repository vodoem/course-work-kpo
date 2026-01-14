using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia.Input;
using CosmicCollector.Avalonia.Infrastructure;
using CosmicCollector.Avalonia.Navigation;
using CosmicCollector.Avalonia.ViewModels;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;
using Xunit;

namespace CosmicCollector.Tests;

public sealed class GameViewModelTests
{
  [Fact]
  public void Activate_StartsRuntime_InitializesFieldBounds_UpdatesFromInitialSnapshot()
  {
    // Arrange
    var bounds = new WorldBounds(0, 0, 800, 600);
    var runtime = CreateInitializedRuntime(bounds);
    var viewModel = CreateViewModel(runtime);

    try
    {
      // Act
      viewModel.Activate();

      // Assert
      Assert.Equal(800, viewModel.FieldWidth);
      Assert.Equal(600, viewModel.FieldHeight);
      Assert.NotNull(viewModel.LatestSnapshot);
    }
    finally
    {
      viewModel.Deactivate();
    }
  }

  [Fact]
  public void Activate_Idempotent_DoesNotStartTwice()
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);

    try
    {
      // Act
      viewModel.Activate();
      var firstSnapshot = viewModel.LatestSnapshot;
      viewModel.Activate();

      // Assert
      Assert.Same(firstSnapshot, viewModel.LatestSnapshot);
    }
    finally
    {
      viewModel.Deactivate();
    }
  }

  [Fact]
  public void Deactivate_StopsRuntime_Unsubscribes()
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);
    viewModel.Activate();

    // Act
    viewModel.Deactivate();

    // Assert
    Assert.Throws<InvalidOperationException>(() => runtime.GameState);
    Assert.Empty(GetSubscriptions(viewModel));
  }

  [Fact]
  public void Deactivate_Idempotent_DoesNotStopTwice()
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);
    viewModel.Activate();

    // Act
    viewModel.Deactivate();
    viewModel.Deactivate();

    // Assert
    Assert.Throws<InvalidOperationException>(() => runtime.GameState);
    Assert.Empty(GetSubscriptions(viewModel));
  }

  [Theory]
  [InlineData(Key.A)]
  [InlineData(Key.Left)]
  public void HandleKeyDown_Left_EnqueuesSetMoveDirectionMinusOne(Key parKey)
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);
    viewModel.Activate();

    try
    {
      // Act
      viewModel.HandleKeyDown(parKey);
      var commands = runtime.CommandQueue.DrainAll();

      // Assert
      var command = Assert.Single(commands);
      var moveCommand = Assert.IsType<SetMoveDirectionCommand>(command);
      Assert.Equal(-1, moveCommand.DirectionX);
    }
    finally
    {
      viewModel.Deactivate();
    }
  }

  [Theory]
  [InlineData(Key.D)]
  [InlineData(Key.Right)]
  public void HandleKeyDown_Right_EnqueuesSetMoveDirectionPlusOne(Key parKey)
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);
    viewModel.Activate();

    try
    {
      // Act
      viewModel.HandleKeyDown(parKey);
      var commands = runtime.CommandQueue.DrainAll();

      // Assert
      var command = Assert.Single(commands);
      var moveCommand = Assert.IsType<SetMoveDirectionCommand>(command);
      Assert.Equal(1, moveCommand.DirectionX);
    }
    finally
    {
      viewModel.Deactivate();
    }
  }

  [Theory]
  [InlineData(Key.A)]
  [InlineData(Key.Left)]
  [InlineData(Key.D)]
  [InlineData(Key.Right)]
  public void HandleKeyUp_LeftOrRight_EnqueuesSetMoveDirectionZero(Key parKey)
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);
    viewModel.Activate();

    try
    {
      viewModel.HandleKeyDown(parKey);
      runtime.CommandQueue.DrainAll();

      // Act
      viewModel.HandleKeyUp(parKey);
      var commands = runtime.CommandQueue.DrainAll();

      // Assert
      var command = Assert.Single(commands);
      var moveCommand = Assert.IsType<SetMoveDirectionCommand>(command);
      Assert.Equal(0, moveCommand.DirectionX);
    }
    finally
    {
      viewModel.Deactivate();
    }
  }

  [Theory]
  [InlineData(Key.Escape)]
  [InlineData(Key.P)]
  public void HandleKeyDown_Escape_EnqueuesTogglePauseCommand(Key parKey)
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);
    viewModel.Activate();

    try
    {
      // Act
      viewModel.HandleKeyDown(parKey);
      var commands = runtime.CommandQueue.DrainAll();

      // Assert
      Assert.Single(commands, command => command is TogglePauseCommand);
    }
    finally
    {
      viewModel.Deactivate();
    }
  }

  private static GameViewModel CreateViewModel(GameRuntime parRuntime)
  {
    var mainMenuNavigation = new NavigationService(new NavigationStore(), () => new StubViewModel());
    var gameOverNavigation = new NavigationService(new NavigationStore(), () => new StubViewModel());
    return new GameViewModel(parRuntime, mainMenuNavigation, gameOverNavigation);
  }

  private static GameRuntime CreateInitializedRuntime(WorldBounds parBounds)
  {
    var runtime = new GameRuntime();
    var drone = new Drone(Guid.NewGuid(), Vector2.Zero, Vector2.Zero, new Aabb(32, 32), 100);
    var gameState = new GameState(drone, parBounds);
    var eventBus = new EventBus();
    var commandQueue = new CommandQueue();

    // Заполняем приватные поля, чтобы не запускать поток игрового цикла в тестах.
    SetPrivateField(runtime, "_gameState", gameState);
    SetPrivateField(runtime, "_eventBus", eventBus);
    SetPrivateField(runtime, "_commandQueue", commandQueue);
    SetPrivateField(runtime, "_isRunning", true);

    return runtime;
  }

  private static List<IDisposable> GetSubscriptions(GameViewModel parViewModel)
  {
    return GetPrivateField<List<IDisposable>>(parViewModel, "_subscriptions");
  }

  private static void SetPrivateField<T>(object parTarget, string parFieldName, T parValue)
  {
    var field = parTarget.GetType().GetField(parFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
    if (field is null)
    {
      throw new InvalidOperationException($"Поле '{parFieldName}' не найдено.");
    }

    field.SetValue(parTarget, parValue);
  }

  private static T GetPrivateField<T>(object parTarget, string parFieldName)
  {
    var field = parTarget.GetType().GetField(parFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
    if (field is null)
    {
      throw new InvalidOperationException($"Поле '{parFieldName}' не найдено.");
    }

    return (T)(field.GetValue(parTarget) ?? throw new InvalidOperationException($"Поле '{parFieldName}' пустое."));
  }

  private sealed class StubViewModel : ViewModelBase
  {
  }
}
