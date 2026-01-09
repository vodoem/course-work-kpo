using CosmicCollector.Core.Model;

namespace CosmicCollector.Core.Services;

/// <summary>
/// Сервис инициализации уровня.
/// </summary>
public sealed class LevelService
{
  private readonly LevelConfigProvider _configProvider;

  /// <summary>
  /// Инициализирует сервис уровней.
  /// </summary>
  /// <param name="parConfigProvider">Поставщик конфигураций уровней.</param>
  public LevelService(LevelConfigProvider parConfigProvider)
  {
    _configProvider = parConfigProvider;
  }

  /// <summary>
  /// Инициализирует уровень, если он ещё не настроен.
  /// </summary>
  /// <param name="parGameState">Состояние игры.</param>
  public void InitLevel(GameState parGameState)
  {
    if (parGameState.IsLevelInitialized || parGameState.HasLevelTimer)
    {
      return;
    }

    var level = parGameState.CurrentLevel;

    if (level <= 0)
    {
      level = 1;
    }

    var config = _configProvider.GetConfig(level);
    var goals = new LevelGoals(config.parRequiredBlue, config.parRequiredGreen, config.parRequiredRed);
    parGameState.InitializeLevel(level, goals, config.parRequiredScore, config.parLevelTimeSec);
  }

  /// <summary>
  /// Переходит к следующему уровню и сбрасывает прогресс.
  /// </summary>
  /// <param name="parGameState">Состояние игры.</param>
  public void AdvanceLevel(GameState parGameState)
  {
    var nextLevel = parGameState.CurrentLevel + 1;
    var config = _configProvider.GetConfig(nextLevel);
    var goals = new LevelGoals(config.parRequiredBlue, config.parRequiredGreen, config.parRequiredRed);
    parGameState.InitializeLevel(nextLevel, goals, config.parRequiredScore, config.parLevelTimeSec);
  }
}
