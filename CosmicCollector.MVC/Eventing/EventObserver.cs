using System;
using CosmicCollector.Core.Events;

namespace CosmicCollector.MVC.Eventing;

/// <summary>
/// Наблюдатель доменных событий с делегатами обработки.
/// </summary>
/// <typeparam name="TEvent">Тип события.</typeparam>
public sealed class EventObserver<TEvent> : IObserver<TEvent> where TEvent : IGameEvent
{
  private readonly Action<TEvent> _onNext;
  private readonly Action<Exception>? _onError;
  private readonly Action? _onCompleted;

  /// <summary>
  /// Инициализирует новый экземпляр <see cref="EventObserver{TEvent}"/>.
  /// </summary>
  /// <param name="parOnNext">Обработчик входящих событий.</param>
  /// <param name="parOnError">Обработчик ошибок.</param>
  /// <param name="parOnCompleted">Обработчик завершения.</param>
  public EventObserver(
    Action<TEvent> parOnNext,
    Action<Exception>? parOnError = null,
    Action? parOnCompleted = null)
  {
    _onNext = parOnNext ?? throw new ArgumentNullException(nameof(parOnNext));
    _onError = parOnError;
    _onCompleted = parOnCompleted;
  }

  /// <inheritdoc />
  public void OnCompleted()
  {
    _onCompleted?.Invoke();
  }

  /// <inheritdoc />
  public void OnError(Exception parError)
  {
    _onError?.Invoke(parError);
  }

  /// <inheritdoc />
  public void OnNext(TEvent parValue)
  {
    _onNext(parValue);
  }
}
