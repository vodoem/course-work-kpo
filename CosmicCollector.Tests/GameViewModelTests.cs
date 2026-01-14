using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia.Input;
using Avalonia.Threading;
using CosmicCollector.Avalonia.Infrastructure;
using CosmicCollector.Avalonia.Navigation;
using CosmicCollector.Avalonia.ViewModels;
using CosmicCollector.Core.Entities;
using CosmicCollector.Core.Events;
using CosmicCollector.Core.Geometry;
using CosmicCollector.Core.Model;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;
using Xunit;

namespace CosmicCollector.Tests;

public sealed class GameViewModelTests
{
  // Batch 3: клавиатура + очередь команд + активность/неактивность + HUD-инварианты.

  [Fact]
  public void HandleKeyDown_WhenNotActive_DoesNothing()
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);

    // Act
    viewModel.HandleKeyDown(Key.Left);
    viewModel.HandleKeyDown(Key.Escape);

    // Assert
    Assert.Empty(runtime.CommandQueue.DrainAll());
  }

  [Fact]
  public void HandleKeyUp_WhenNotActive_DoesNothing()
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);

    // Act
    viewModel.HandleKeyUp(Key.Left);

    // Assert
    Assert.Empty(runtime.CommandQueue.DrainAll());
  }

  [Fact]
  public void HandleKeyDown_LeftOrA_EnqueuesSetMoveDirectionMinusOne()
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);
    viewModel.Activate();

    try
    {
      // Act
      viewModel.HandleKeyDown(Key.A);
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

  [Fact]
  public void HandleKeyDown_RightOrD_EnqueuesSetMoveDirectionPlusOne()
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);
    viewModel.Activate();

    try
    {
      // Act
      viewModel.HandleKeyDown(Key.D);
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

  [Fact]
  public void HandleKeyUp_AfterMovementKey_EnqueuesSetMoveDirectionZero()
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);
    viewModel.Activate();

    try
    {
      viewModel.HandleKeyDown(Key.Left);
      runtime.CommandQueue.DrainAll();

      // Act
      viewModel.HandleKeyUp(Key.Left);
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

  [Fact]
  public void HandleKeyDown_Escape_EnqueuesTogglePauseCommand_Batch3()
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);
    viewModel.Activate();

    try
    {
      // Act
      viewModel.HandleKeyDown(Key.Escape);
      var commands = runtime.CommandQueue.DrainAll();

      // Assert
      Assert.Single(commands, command => command is TogglePauseCommand);
    }
    finally
    {
      viewModel.Deactivate();
    }
  }

  [Fact]
  public void HandleKeyDown_P_EnqueuesTogglePauseCommand()
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);
    viewModel.Activate();

    try
    {
      // Act
      viewModel.HandleKeyDown(Key.P);
      var commands = runtime.CommandQueue.DrainAll();

      // Assert
      Assert.Single(commands, command => command is TogglePauseCommand);
    }
    finally
    {
      viewModel.Deactivate();
    }
  }

  [Fact]
  public void Activate_CallsRuntimeStart_AndInitializesFieldBoundsFromWorldBounds()
  {
    // Arrange
    var runtime = new GameRuntime();
    var viewModel = CreateViewModel(runtime);

    try
    {
      // Act
      viewModel.Activate();

      // Assert
      Assert.NotNull(GetPrivateField<object>(runtime, "_gameLoopRunner"));
      Assert.Equal(800, viewModel.FieldWidth);
      Assert.Equal(600, viewModel.FieldHeight);
    }
    finally
    {
      viewModel.Deactivate();
    }
  }

  [Fact]
  public void Activate_SecondCall_IsIdempotent_DoesNotStartTwice()
  {
    // Arrange
    var runtime = new GameRuntime();
    var viewModel = CreateViewModel(runtime);

    try
    {
      viewModel.Activate();
      var firstRunner = GetPrivateField<object>(runtime, "_gameLoopRunner");
      var firstTickSubscriptions = GetEventBusHandlerCount<GameTick>(runtime.EventBus);

      // Act
      viewModel.Activate();
      var secondRunner = GetPrivateField<object>(runtime, "_gameLoopRunner");
      var secondTickSubscriptions = GetEventBusHandlerCount<GameTick>(runtime.EventBus);

      // Assert
      Assert.Same(firstRunner, secondRunner);
      Assert.Equal(firstTickSubscriptions, secondTickSubscriptions);
    }
    finally
    {
      viewModel.Deactivate();
    }
  }

  [Fact]
  public void Deactivate_WhenNotActive_DoesNothing()
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);

    // Act
    viewModel.Deactivate();

    // Assert
    Assert.NotNull(runtime.GameState);
    Assert.Empty(GetSubscriptions(viewModel));
  }

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
  public void HandleKeyDown_Escape_EnqueuesTogglePauseCommand_Batch2(Key parKey)
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

  [Fact]
  public void OnGameOver_DeactivatesAndNavigatesToGameOver()
  {
    // Arrange
    var bounds = new WorldBounds(0, 0, 800, 600);
    var runtime = CreateInitializedRuntime(bounds);
    var navigation = CreateNavigationProbes();
    var viewModel = new GameViewModel(runtime, navigation.MainMenu.Service, navigation.GameOver.Service);
    viewModel.Activate();

    // Act
    runtime.EventBus.Publish(new GameOver("fail"));
    DrainUiThread();
    var stopped = WaitForRuntimeStop(runtime);

    // Assert
    Assert.Equal(1, navigation.GameOver.NavigateCount);
    Assert.True(stopped);
  }

  [Fact]
  public void OnLevelCompleted_UpdatesSnapshotButDoesNotNavigate()
  {
    // Arrange
    var bounds = new WorldBounds(0, 0, 800, 600);
    var runtime = CreateInitializedRuntime(bounds);
    SetGameStateScore(runtime.GameState, 120);
    var navigation = CreateNavigationProbes();
    var viewModel = new GameViewModel(runtime, navigation.MainMenu.Service, navigation.GameOver.Service);
    viewModel.Activate();

    // Act
    runtime.EventBus.Publish(new LevelCompleted("ok"));
    DrainUiThread();

    // Assert
    Assert.Equal(0, navigation.GameOver.NavigateCount);
    Assert.Equal(120, viewModel.Score);
  }

  [Fact]
  public void OnPauseToggled_ToPaused_SetsIsPausedTrue_AndShowsPauseOverlayWhenCountdownZero()
  {
    // Arrange
    var viewModel = CreateActivatedViewModel();

    try
    {
      // Act
      InvokePauseToggled(viewModel, true);
      DrainUiThread();

      // Assert
      Assert.True(viewModel.IsPaused);
      Assert.True(viewModel.IsPauseOverlayVisible);
      Assert.False(viewModel.IsCountdownVisible);
    }
    finally
    {
      viewModel.Deactivate();
    }
  }

  [Fact]
  public void OnCountdownTick_SetsCountdownValue_AndShowsCountdownOverlay()
  {
    // Arrange
    var viewModel = CreateActivatedViewModel();

    try
    {
      InvokePauseToggled(viewModel, true);
      DrainUiThread();

      // Act
      InvokeCountdownTick(viewModel, 3);
      DrainUiThread();

      // Assert
      Assert.Equal(3, viewModel.CountdownValue);
      Assert.True(viewModel.IsCountdownVisible);
      Assert.False(viewModel.IsPauseOverlayVisible);
    }
    finally
    {
      viewModel.Deactivate();
    }
  }

  [Fact]
  public void OnPauseToggled_ToUnpaused_ResetsCountdownValueToZero_AndHidesOverlays()
  {
    // Arrange
    var viewModel = CreateActivatedViewModel();

    try
    {
      InvokePauseToggled(viewModel, true);
      InvokeCountdownTick(viewModel, 2);
      DrainUiThread();

      // Act
      InvokePauseToggled(viewModel, false);
      DrainUiThread();

      // Assert
      Assert.False(viewModel.IsPaused);
      Assert.Equal(0, viewModel.CountdownValue);
      Assert.False(viewModel.IsPauseOverlayVisible);
      Assert.False(viewModel.IsCountdownVisible);
    }
    finally
    {
      viewModel.Deactivate();
    }
  }

  [Fact]
  public void CountdownFlow_3_2_1_ThenUnpause_ResultsInHiddenCountdown()
  {
    // Arrange
    var viewModel = CreateActivatedViewModel();

    try
    {
      // Act
      InvokePauseToggled(viewModel, true);
      InvokeCountdownTick(viewModel, 3);
      InvokeCountdownTick(viewModel, 2);
      InvokeCountdownTick(viewModel, 1);
      InvokePauseToggled(viewModel, false);
      DrainUiThread();

      // Assert
      Assert.False(viewModel.IsPaused);
      Assert.Equal(0, viewModel.CountdownValue);
      Assert.False(viewModel.IsCountdownVisible);
    }
    finally
    {
      viewModel.Deactivate();
    }
  }

  [Fact]
  public void ResumeCommand_WhenPausedAndNoCountdown_EnqueuesTogglePauseCommand()
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);
    viewModel.Activate();

    try
    {
      InvokePauseToggled(viewModel, true);
      DrainUiThread();

      // Act
      viewModel.ResumeCommand.Execute(null);
      var commands = runtime.CommandQueue.DrainAll();

      // Assert
      Assert.Single(commands, command => command is TogglePauseCommand);
    }
    finally
    {
      viewModel.Deactivate();
    }
  }

  [Fact]
  public void ResumeCommand_WhenCountdownActive_DoesNotEnqueueTogglePause()
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);
    viewModel.Activate();

    try
    {
      InvokePauseToggled(viewModel, true);
      InvokeCountdownTick(viewModel, 2);
      DrainUiThread();
      runtime.CommandQueue.DrainAll();

      // Act
      viewModel.ResumeCommand.Execute(null);
      var commands = runtime.CommandQueue.DrainAll();

      // Assert
      Assert.DoesNotContain(commands, command => command is TogglePauseCommand);
    }
    finally
    {
      viewModel.Deactivate();
    }
  }

  [Fact]
  public void ExitToMenuCommand_DeactivatesAndNavigatesToMainMenu()
  {
    // Arrange
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var navigation = CreateNavigationProbes();
    var viewModel = new GameViewModel(runtime, navigation.MainMenu.Service, navigation.GameOver.Service);
    viewModel.Activate();

    // Act
    viewModel.ExitToMenuCommand.Execute(null);
    var stopped = WaitForRuntimeStop(runtime);

    // Assert
    Assert.Equal(1, navigation.MainMenu.NavigateCount);
    Assert.True(stopped);
  }

  private static GameViewModel CreateViewModel(GameRuntime parRuntime)
  {
    var mainMenuNavigation = new NavigationService(new NavigationStore(), () => new StubViewModel());
    var gameOverNavigation = new NavigationService(new NavigationStore(), () => new StubViewModel());
    return new GameViewModel(parRuntime, mainMenuNavigation, gameOverNavigation);
  }

  private static GameViewModel CreateActivatedViewModel()
  {
    var runtime = CreateInitializedRuntime(new WorldBounds(0, 0, 800, 600));
    var viewModel = CreateViewModel(runtime);
    viewModel.Activate();
    return viewModel;
  }

  private static (NavigationProbe MainMenu, NavigationProbe GameOver) CreateNavigationProbes()
  {
    var mainMenu = new NavigationProbe();
    var gameOver = new NavigationProbe();
    return (mainMenu, gameOver);
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

  private static int GetEventBusHandlerCount<TEvent>(EventBus parEventBus) where TEvent : IGameEvent
  {
    var handlers = GetPrivateField<Dictionary<Type, List<Delegate>>>(parEventBus, "_handlers");
    return handlers.TryGetValue(typeof(TEvent), out var list) ? list.Count : 0;
  }

  private static void InvokePauseToggled(GameViewModel parViewModel, bool parIsPaused)
  {
    InvokePrivateMethod(parViewModel, "OnPauseToggled", new PauseToggled(parIsPaused));
  }

  private static void InvokeCountdownTick(GameViewModel parViewModel, int parValue)
  {
    InvokePrivateMethod(parViewModel, "OnCountdownTick", new CountdownTick(parValue));
  }

  private static void DrainUiThread()
  {
    // Avalonia.Dispatcher не даёт публичного способа синхронно выполнить Post, поэтому пробуем RunJobs через reflection.
    var dispatcher = Dispatcher.UIThread;
    if (!TryRunDispatcherJobs(dispatcher))
    {
      dispatcher.InvokeAsync(() => { }).GetAwaiter().GetResult();
      TryRunDispatcherJobs(dispatcher);
    }
  }

  private static bool TryRunDispatcherJobs(Dispatcher parDispatcher)
  {
    var dispatcherType = parDispatcher.GetType();
    var runJobsMethod = dispatcherType.GetMethod("RunJobs", new[] { typeof(DispatcherPriority) })
                        ?? dispatcherType.GetMethod("RunJobs", Type.EmptyTypes);
    if (runJobsMethod is null)
    {
      return false;
    }

    var parameters = runJobsMethod.GetParameters().Length == 0
      ? Array.Empty<object>()
      : new object[] { DispatcherPriority.Background };
    runJobsMethod.Invoke(parDispatcher, parameters);
    return true;
  }

  private static bool WaitForRuntimeStop(GameRuntime parRuntime)
  {
    for (var attempt = 0; attempt < 20; attempt++)
    {
      try
      {
        _ = parRuntime.GameState;
      }
      catch (InvalidOperationException)
      {
        return true;
      }

      // Ждём завершения Task.Run(_gameRuntime.Stop) из обработчика GameOver.
      Thread.Sleep(10);
    }

    return false;
  }

  private static void SetGameStateScore(GameState parGameState, int parScore)
  {
    SetPrivateField(parGameState, "_score", parScore);
  }

  private static void InvokePrivateMethod(object parTarget, string parMethodName, object parArgument)
  {
    var method = parTarget.GetType().GetMethod(parMethodName, BindingFlags.Instance | BindingFlags.NonPublic);
    if (method is null)
    {
      throw new InvalidOperationException($"Метод '{parMethodName}' не найден.");
    }

    method.Invoke(parTarget, new[] { parArgument });
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

  private sealed class NavigationProbe
  {
    private int _navigateCount;

    public NavigationProbe()
    {
      Store = new NavigationStore();
      Store.CurrentViewModelChanged += () => _navigateCount++;
      Service = new NavigationService(Store, () => new StubViewModel());
    }

    public NavigationService Service { get; }

    public NavigationStore Store { get; }

    public int NavigateCount => _navigateCount;
  }
}
