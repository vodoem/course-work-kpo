namespace CosmicCollector.Core.Events;

/// <summary>
/// Сигнализирует о закрытии диалога подтверждения сохранения.
/// </summary>
public sealed record ConfirmSaveBeforeMenuClosed() : IGameEvent;
