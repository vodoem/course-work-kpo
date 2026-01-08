namespace CosmicCollector.Core.Snapshots;

/// <summary>
/// Неизменяемый снимок одной записи рекорда.
/// </summary>
/// <param name="parPlayerName">Имя игрока.</param>
/// <param name="parScore">Количество очков.</param>
public sealed record RecordSnapshot(string parPlayerName, int parScore);
