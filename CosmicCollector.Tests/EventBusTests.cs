using CosmicCollector.Core.Events;
using CosmicCollector.MVC.Eventing;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет работу шины событий.
/// </summary>
public sealed class EventBusTests
{
  /// <summary>
  /// Проверяет доставку события подписчику.
  /// </summary>
  [Xunit.Fact]
  public void Publish_DeliversEventToSubscriber()
  {
    var bus = new EventBus();
    var received = false;

    bus.Subscribe<GameStarted>(_ => received = true);

    bus.Publish(new GameStarted(1));

    Xunit.Assert.True(received);
  }

  /// <summary>
  /// Проверяет, что отписка прекращает доставку событий.
  /// </summary>
  [Xunit.Fact]
  public void Dispose_UnsubscribesHandler()
  {
    var bus = new EventBus();
    var received = 0;

    using (bus.Subscribe<GameTick>(_ => received++))
    {
      bus.Publish(new GameTick(1.0 / 60.0, 1));
    }

    bus.Publish(new GameTick(1.0 / 60.0, 2));

    Xunit.Assert.Equal(1, received);
  }

  /// <summary>
  /// Проверяет, что разные типы событий не пересекаются.
  /// </summary>
  [Xunit.Fact]
  public void Publish_DoesNotDeliverDifferentEventType()
  {
    var bus = new EventBus();
    var startedReceived = 0;
    var tickReceived = 0;

    bus.Subscribe<GameStarted>(_ => startedReceived++);
    bus.Subscribe<GameTick>(_ => tickReceived++);

    bus.Publish(new GameStarted(2));

    Xunit.Assert.Equal(1, startedReceived);
    Xunit.Assert.Equal(0, tickReceived);
  }
}
