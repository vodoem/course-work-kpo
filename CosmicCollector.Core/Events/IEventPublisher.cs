namespace CosmicCollector.Core.Events;

/// <summary>
/// Публикует доменные события.
/// </summary>
public interface IEventPublisher
{
  /// <summary>
  /// Публикует доменное событие.
  /// </summary>
  /// <param name="parEvent">Событие для публикации.</param>
  void Publish(IGameEvent parEvent);
}
