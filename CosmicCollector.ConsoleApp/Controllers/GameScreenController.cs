using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;
using CosmicCollector.Core.Events;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Loop;

namespace CosmicCollector.ConsoleApp.Controllers;

/// <summary>
/// Контроллер игрового экрана (только отображение).
/// </summary>
public sealed class GameScreenController
{
  private readonly IGameScreenView _view;
  private readonly IEventBus _eventBus;
  private readonly IGameSnapshotProvider _snapshotProvider;
  private readonly IGameLoopRunner _gameLoopRunner;
  private readonly AutoResetEvent _tickSignal = new(false);
  private IDisposable? _gameStartedSubscription;
  private IDisposable? _gameTickSubscription;
  private int _level;

  /// <summary>
  /// Создаёт контроллер игрового экрана.
  /// </summary>
  /// <param name="parView">Представление игрового экрана.</param>
  /// <param name="parEventBus">Шина событий.</param>
  /// <param name="parSnapshotProvider">Поставщик снимков.</param>
  /// <param name="parGameLoopRunner">Игровой цикл.</param>
  /// <param name="parLevel">Стартовый уровень.</param>
  public GameScreenController(
    IGameScreenView parView,
    IEventBus parEventBus,
    IGameSnapshotProvider parSnapshotProvider,
    IGameLoopRunner parGameLoopRunner,
    int parLevel)
  {
    _view = parView;
    _eventBus = parEventBus;
    _snapshotProvider = parSnapshotProvider;
    _gameLoopRunner = parGameLoopRunner;
    _level = parLevel;
  }

  /// <summary>
  /// Запускает игровой экран в режиме отображения.
  /// </summary>
  public void Run()
  {
    _gameStartedSubscription = _eventBus.Subscribe<GameStarted>(OnGameStarted);
    _gameTickSubscription = _eventBus.Subscribe<GameTick>(OnGameTick);

    _eventBus.Publish(new GameStarted(_level));
    _gameLoopRunner.Start();

    while (true)
    {
      _tickSignal.WaitOne();
    }
  }

  private void OnGameStarted(GameStarted parEvent)
  {
    _level = parEvent.parLevel;
  }

  private void OnGameTick(GameTick parEvent)
  {
    var snapshot = _snapshotProvider.GetSnapshot();
    _view.Render(snapshot, _level);
    _tickSignal.Set();
  }
}
