namespace CosmicCollector.Core.Events;

/// <summary>
/// Сигнализирует о необходимости подтвердить сохранение перед выходом в меню.
/// </summary>
public sealed record ConfirmSaveBeforeMenuRequested() : IGameEvent;
