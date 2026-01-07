namespace CosmicCollector.Core.Events;

/// <summary>
/// Событие активации бонуса.
/// </summary>
/// <param name="parBonusType">Тип бонуса.</param>
/// <param name="parDurationSec">Длительность бонуса в секундах.</param>
public sealed record BonusActivated(string parBonusType, double parDurationSec) : IGameEvent;
