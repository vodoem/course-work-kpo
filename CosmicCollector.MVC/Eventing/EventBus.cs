using CosmicCollector.Core.Events;

namespace CosmicCollector.MVC.Eventing;

/// <summary>
/// Потокобезопасная шина доменных событий.
/// </summary>
public sealed class EventBus : IEventBus
{
  private readonly object _lockObject = new();
  private readonly Dictionary<Type, List<Delegate>> _handlers = new();

  /// <inheritdoc />
  public void Publish<TEvent>(TEvent parEvent) where TEvent : IGameEvent
  {
    PublishInternal(typeof(TEvent), parEvent);
  }

  /// <inheritdoc />
  public void Publish(IGameEvent parEvent)
  {
    PublishInternal(parEvent.GetType(), parEvent);
  }

  /// <inheritdoc />
  public IDisposable Subscribe<TEvent>(Action<TEvent> parHandler) where TEvent : IGameEvent
  {
    lock (_lockObject)
    {
      if (!_handlers.TryGetValue(typeof(TEvent), out var handlers))
      {
        handlers = new List<Delegate>();
        _handlers[typeof(TEvent)] = handlers;
      }

      handlers.Add(parHandler);
    }

    return new Subscription<TEvent>(this, parHandler);
  }

  internal void Unsubscribe<TEvent>(Action<TEvent> parHandler) where TEvent : IGameEvent
  {
    lock (_lockObject)
    {
      if (!_handlers.TryGetValue(typeof(TEvent), out var handlers))
      {
        return;
      }

      handlers.Remove(parHandler);

      if (handlers.Count == 0)
      {
        _handlers.Remove(typeof(TEvent));
      }
    }
  }

  private void PublishInternal(Type parEventType, IGameEvent parEvent)
  {
    List<Delegate>? handlersCopy;

    lock (_lockObject)
    {
      if (!_handlers.TryGetValue(parEventType, out var handlers))
      {
        return;
      }

      handlersCopy = new List<Delegate>(handlers);
    }

    foreach (var handler in handlersCopy)
    {
      handler.DynamicInvoke(parEvent);
    }
  }
}
