namespace CosmicCollector.Core.Events;

/// <summary>
/// Событие переключения паузы.
/// </summary>
/// <param name="parIsPaused">Признак паузы.</param>
public sealed record PauseToggled(bool parIsPaused) : IGameEvent;
