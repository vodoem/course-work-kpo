using System;
using CosmicCollector.Core.Events;

namespace CosmicCollector.MVC.Eventing;

/// <summary>
/// Наблюдаемый поток доменных событий указанного типа.
/// </summary>
/// <typeparam name="TEvent">Тип события.</typeparam>
public sealed class EventObservable<TEvent> : IObservable<TEvent> where TEvent : IGameEvent
{
  private readonly EventBus _eventBus;

  /// <summary>
  /// Инициализирует новый экземпляр <see cref="EventObservable{TEvent}"/>.
  /// </summary>
  /// <param name="parEventBus">Шина событий.</param>
  public EventObservable(EventBus parEventBus)
  {
    _eventBus = parEventBus;
  }

  /// <inheritdoc />
  public IDisposable Subscribe(IObserver<TEvent> parObserver)
  {
    if (parObserver is null)
    {
      throw new ArgumentNullException(nameof(parObserver));
    }

    return _eventBus.Subscribe<TEvent>(parObserver.OnNext);
  }
}
