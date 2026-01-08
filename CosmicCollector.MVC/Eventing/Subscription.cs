using CosmicCollector.Core.Events;

namespace CosmicCollector.MVC.Eventing;

/// <summary>
/// Представляет подписку на событие с возможностью отписки.
/// </summary>
/// <typeparam name="TEvent">Тип события.</typeparam>
public sealed class Subscription<TEvent> : IDisposable where TEvent : IGameEvent
{
  private readonly EventBus _eventBus;
  private readonly Action<TEvent> _handler;
  private bool _isDisposed;

  /// <summary>
  /// Инициализирует новую подписку.
  /// </summary>
  /// <param name="parEventBus">Шина событий.</param>
  /// <param name="parHandler">Обработчик события.</param>
  public Subscription(EventBus parEventBus, Action<TEvent> parHandler)
  {
    _eventBus = parEventBus;
    _handler = parHandler;
  }

  /// <summary>
  /// Освобождает ресурсы и снимает подписку.
  /// </summary>
  public void Dispose()
  {
    if (_isDisposed)
    {
      return;
    }

    _isDisposed = true;
    _eventBus.Unsubscribe(_handler);
  }
}
