namespace CosmicCollector.Core.Events;

/// <summary>
/// Событие запроса выхода в меню.
/// </summary>
/// <param name="parShouldSave">Нужно ли сохранить перед выходом.</param>
public sealed record MenuExitRequested(bool parShouldSave) : IGameEvent;
