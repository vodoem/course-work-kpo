namespace CosmicCollector.Core.Events;

/// <summary>
/// Событие сбора кристалла.
/// </summary>
/// <param name="parCrystalType">Тип кристалла.</param>
/// <param name="parPoints">Начисленные очки.</param>
/// <param name="parEnergyDelta">Изменение энергии.</param>
public sealed record CrystalCollected(string parCrystalType, int parPoints, int parEnergyDelta) : IGameEvent;
