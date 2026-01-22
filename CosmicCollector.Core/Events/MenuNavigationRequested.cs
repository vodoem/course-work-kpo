namespace CosmicCollector.Core.Events;

/// <summary>
/// Сигнализирует о переходе в главное меню.
/// </summary>
public sealed record MenuNavigationRequested() : IGameEvent;
