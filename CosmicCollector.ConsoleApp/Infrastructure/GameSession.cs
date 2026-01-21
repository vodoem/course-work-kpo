using CosmicCollector.Core.Geometry;
using CosmicCollector.MVC.Commands;
using CosmicCollector.MVC.Eventing;
using CosmicCollector.MVC.Loop;

namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Контейнер зависимостей игрового сеанса для UI.
/// </summary>
public sealed class GameSession
{
  /// <summary>
  /// Инициализирует игровой сеанс.
  /// </summary>
  /// <param name="parEventBus">Шина событий.</param>
  /// <param name="parGameLoopRunner">Игровой цикл.</param>
  /// <param name="parSnapshotProvider">Поставщик снимков.</param>
  /// <param name="parCommandQueue">Очередь команд.</param>
  /// <param name="parWorldBounds">Границы мира.</param>
  /// <param name="parLevel">Номер уровня.</param>
  public GameSession(
    IEventBus parEventBus,
    IGameLoopRunner parGameLoopRunner,
    IGameSnapshotProvider parSnapshotProvider,
    CommandQueue parCommandQueue,
    WorldBounds parWorldBounds,
    int parLevel)
  {
    EventBus = parEventBus;
    GameLoopRunner = parGameLoopRunner;
    SnapshotProvider = parSnapshotProvider;
    CommandQueue = parCommandQueue;
    WorldBounds = parWorldBounds;
    Level = parLevel;
  }

  /// <summary>
  /// Шина событий.
  /// </summary>
  public IEventBus EventBus { get; }

  /// <summary>
  /// Игровой цикл.
  /// </summary>
  public IGameLoopRunner GameLoopRunner { get; }

  /// <summary>
  /// Поставщик снимков.
  /// </summary>
  public IGameSnapshotProvider SnapshotProvider { get; }

  /// <summary>
  /// Очередь команд.
  /// </summary>
  public CommandQueue CommandQueue { get; }

  /// <summary>
  /// Границы игрового мира.
  /// </summary>
  public WorldBounds WorldBounds { get; }

  /// <summary>
  /// Номер уровня.
  /// </summary>
  public int Level { get; }
}
