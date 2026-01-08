using System.Collections.Generic;

namespace CosmicCollector.Core.Snapshots;

/// <summary>
/// Неизменяемый снимок состояния игры для представлений.
/// </summary>
/// <param name="parIsPaused">Признак паузы.</param>
/// <param name="parTickNo">Номер тика.</param>
/// <param name="parHasLevelTimer">Признак наличия таймера уровня.</param>
/// <param name="parLevelTimeRemainingSec">Оставшееся время уровня в секундах.</param>
/// <param name="parDrone">Снимок дрона.</param>
/// <param name="parCrystals">Снимки кристаллов.</param>
/// <param name="parAsteroids">Снимки астероидов.</param>
/// <param name="parBonuses">Снимки бонусов.</param>
/// <param name="parBlackHoles">Снимки чёрных дыр.</param>
public sealed record GameSnapshot(
  bool parIsPaused,
  long parTickNo,
  bool parHasLevelTimer,
  double parLevelTimeRemainingSec,
  DroneSnapshot parDrone,
  IReadOnlyList<CrystalSnapshot> parCrystals,
  IReadOnlyList<AsteroidSnapshot> parAsteroids,
  IReadOnlyList<BonusSnapshot> parBonuses,
  IReadOnlyList<BlackHoleSnapshot> parBlackHoles);
