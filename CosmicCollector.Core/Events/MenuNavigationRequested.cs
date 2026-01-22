namespace CosmicCollector.Core.Events;

/// <summary>
/// Сигнализирует о переходе в главное меню.
/// </summary>
/// <param name="parWasSaved">Признак сохранения перед выходом.</param>
public sealed record MenuNavigationRequested(bool parWasSaved) : IGameEvent;
