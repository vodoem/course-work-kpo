using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.Core.Snapshots;
using CosmicCollector.Persistence.Records;

namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Контракт представления экрана завершения игры.
/// </summary>
public interface IGameEndView
{
  /// <summary>
  /// Отрисовывает экран завершения игры.
  /// </summary>
  /// <param name="parReason">Причина завершения.</param>
  /// <param name="parSnapshot">Финальный снимок.</param>
  /// <param name="parLevel">Номер уровня.</param>
  /// <param name="parIsHighScore">Признак попадания в рекорды.</param>
  /// <param name="parPlayerName">Текущее имя игрока.</param>
  /// <param name="parIsSaved">Признак сохранения.</param>
  /// <param name="parTopRecords">Топ рекордов для отображения.</param>
  void Render(
    GameEndReason parReason,
    GameSnapshot parSnapshot,
    int parLevel,
    bool parIsHighScore,
    string parPlayerName,
    bool parIsSaved,
    IReadOnlyList<RecordEntry> parTopRecords);
}
