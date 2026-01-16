using System;
using CosmicCollector.Core.Events;

namespace CosmicCollector.MVC.Eventing;

/// <summary>
/// Публикует и подписывает на доменные события ядра.
/// </summary>
public interface IEventBus : IEventPublisher
{
  /// <summary>
  /// Публикует доменное событие для всех подписчиков.
  /// </summary>
  /// <typeparam name="TEvent">Тип публикуемого события.</typeparam>
  /// <param name="parEvent">Экземпляр события для публикации.</param>
  void Publish<TEvent>(TEvent parEvent) where TEvent : IGameEvent;

  /// <summary>
  /// Подписывает на события указанного типа.
  /// </summary>
  /// <typeparam name="TEvent">Тип события для подписки.</typeparam>
  /// <param name="parHandler">Обработчик, который будет вызван.</param>
  /// <returns>Объект для отмены подписки.</returns>
  IDisposable Subscribe<TEvent>(Action<TEvent> parHandler) where TEvent : IGameEvent;

  /// <summary>
  /// Подписывает наблюдателя на события указанного типа.
  /// </summary>
  /// <typeparam name="TEvent">Тип события для подписки.</typeparam>
  /// <param name="parObserver">Наблюдатель событий.</param>
  /// <returns>Объект для отмены подписки.</returns>
  IDisposable Subscribe<TEvent>(IObserver<TEvent> parObserver) where TEvent : IGameEvent;

  /// <summary>
  /// Возвращает наблюдаемый поток событий указанного типа.
  /// </summary>
  /// <typeparam name="TEvent">Тип события.</typeparam>
  /// <returns>Наблюдаемый поток событий.</returns>
  IObservable<TEvent> Observe<TEvent>() where TEvent : IGameEvent;
}
