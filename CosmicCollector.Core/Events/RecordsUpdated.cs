using CosmicCollector.Core.Snapshots;

namespace CosmicCollector.Core.Events;

/// <summary>
/// Событие обновления таблицы рекордов.
/// </summary>
/// <param name="parRecords">Список актуальных рекордов.</param>
public sealed record RecordsUpdated(IReadOnlyList<RecordSnapshot> parRecords) : IGameEvent;
