using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.ConsoleApp.Views;
using CosmicCollector.Core.Snapshots;
using CosmicCollector.Persistence.Records;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CosmicCollector.ConsoleApp.Controllers;

/// <summary>
/// Контроллер экрана завершения игры с сохранением рекорда.
/// </summary>
public sealed class GameEndController
{
  private const int MaxNameLength = 16;
  /// <summary>
  /// Максимальное число рекордов, учитываемых для попадания в таблицу.
  /// </summary>
  private const int MaxRecords = 10;
  private readonly IGameEndView _view;
  private readonly IConsoleInputReader _inputReader;
  private readonly IRecordsRepository _recordsRepository;

  /// <summary>
  /// Создаёт контроллер экрана завершения.
  /// </summary>
  /// <param name="parView">Представление экрана завершения.</param>
  /// <param name="parInputReader">Читатель ввода.</param>
  /// <param name="parRecordsRepository">Репозиторий рекордов.</param>
  public GameEndController(
    IGameEndView parView,
    IConsoleInputReader parInputReader,
    IRecordsRepository parRecordsRepository)
  {
    _view = parView;
    _inputReader = parInputReader;
    _recordsRepository = parRecordsRepository;
  }

  /// <summary>
  /// Запускает экран завершения и возвращает выбранное действие.
  /// </summary>
  /// <param name="parReason">Причина завершения.</param>
  /// <param name="parSnapshot">Финальный снимок.</param>
  /// <param name="parLevel">Номер уровня.</param>
  /// <returns>Действие после экрана завершения.</returns>
  public GameEndAction Run(GameEndReason parReason, GameSnapshot parSnapshot, int parLevel)
  {
    var records = _recordsRepository.LoadAll();
    bool isHighScore = IsHighScore(records, parSnapshot.parDrone.parScore, parLevel);
    string playerName = string.Empty;
    bool isSaved = false;
    IReadOnlyList<RecordEntry> topRecords = Array.Empty<RecordEntry>();

    Render(parReason, parSnapshot, parLevel, isHighScore, playerName, isSaved, topRecords);

    while (true)
    {
      ConsoleKeyInfo keyInfo = _inputReader.ReadKey();

      if (keyInfo.Key == ConsoleKey.Escape)
      {
        return GameEndAction.ReturnToMenu;
      }

      if (keyInfo.Key == ConsoleKey.R)
      {
        return GameEndAction.RestartGame;
      }

      if (keyInfo.Key == ConsoleKey.Enter)
      {
        if (isHighScore && !isSaved)
        {
          if (!IsValidName(playerName))
          {
            continue;
          }

          var record = new RecordEntry(
            playerName,
            parSnapshot.parDrone.parScore,
            parLevel,
            DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
          _recordsRepository.Add(record);
          isSaved = true;
          records = _recordsRepository.LoadAll();
          topRecords = GetTopRecords(records);
          Render(parReason, parSnapshot, parLevel, isHighScore, playerName, isSaved, topRecords);
          continue;
        }

        return GameEndAction.ReturnToMenu;
      }

      if (isHighScore && !isSaved)
      {
        if (keyInfo.Key == ConsoleKey.Backspace)
        {
          if (playerName.Length > 0)
          {
            playerName = playerName[..^1];
            Render(parReason, parSnapshot, parLevel, isHighScore, playerName, isSaved, topRecords);
          }

          continue;
        }

        char keyChar = keyInfo.KeyChar;
        if (IsAllowedChar(keyChar) && playerName.Length < MaxNameLength)
        {
          playerName += keyChar;
          Render(parReason, parSnapshot, parLevel, isHighScore, playerName, isSaved, topRecords);
        }
      }
    }
  }

  private void Render(
    GameEndReason parReason,
    GameSnapshot parSnapshot,
    int parLevel,
    bool parIsHighScore,
    string parPlayerName,
    bool parIsSaved,
    IReadOnlyList<RecordEntry> parTopRecords)
  {
    _view.Render(
      parReason,
      parSnapshot,
      parLevel,
      parIsHighScore,
      parPlayerName,
      parIsSaved,
      parTopRecords);
  }

  private static bool IsValidName(string parValue)
  {
    if (parValue.Length == 0 || parValue.Length > MaxNameLength)
    {
      return false;
    }

    return parValue.All(IsAllowedChar);
  }

  private static bool IsAllowedChar(char parChar)
  {
    return char.IsLetterOrDigit(parChar) || parChar == ' ' || parChar == '_' || parChar == '-';
  }

  private static bool IsHighScore(IReadOnlyList<RecordEntry> parRecords, int parScore, int parLevel)
  {
    if (parRecords.Count < MaxRecords)
    {
      return true;
    }

    RecordEntry? worst = GetTopRecords(parRecords).LastOrDefault();
    if (worst is null)
    {
      return true;
    }

    if (parScore > worst.parScore)
    {
      return true;
    }

    return parScore == worst.parScore && parLevel > worst.parLevel;
  }

  private static IReadOnlyList<RecordEntry> GetTopRecords(IReadOnlyList<RecordEntry> parRecords)
  {
    return parRecords
      .Select((record, index) => new { Record = record, Index = index })
      .OrderByDescending(item => item.Record.parScore)
      .ThenByDescending(item => item.Record.parLevel)
      .ThenBy(item => item.Index)
      .Take(MaxRecords)
      .Select(item => item.Record)
      .ToList();
  }
}
