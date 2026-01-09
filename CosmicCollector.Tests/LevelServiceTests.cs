using CosmicCollector.Core.Model;
using CosmicCollector.Core.Services;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет инициализацию уровней.
/// </summary>
public sealed class LevelServiceTests
{
  /// <summary>
  /// Проверяет, что при инициализации уровня задаются цели и таймер.
  /// </summary>
  [Xunit.Fact]
  public void InitLevel_SetsTimerAndGoals()
  {
    var state = new GameState();
    var service = new LevelService(new LevelConfigProvider());

    service.InitLevel(state);

    Xunit.Assert.True(state.LevelTimeRemainingSec > 0);
    Xunit.Assert.True(state.LevelGoals.RequiredBlue > 0);
    Xunit.Assert.True(state.LevelGoals.RequiredGreen > 0);
    Xunit.Assert.True(state.LevelGoals.RequiredRed > 0);
  }

  /// <summary>
  /// Проверяет, что снимок содержит цели, прогресс и уровень.
  /// </summary>
  [Xunit.Fact]
  public void Snapshot_ContainsGoalsAndProgress()
  {
    var state = new GameState();
    var service = new LevelService(new LevelConfigProvider());

    service.InitLevel(state);

    var snapshot = state.GetSnapshot();

    Xunit.Assert.Equal(1, snapshot.parCurrentLevel);
    Xunit.Assert.Equal(state.LevelGoals.RequiredBlue, snapshot.parLevelGoals.parRequiredBlue);
    Xunit.Assert.Equal(state.LevelGoals.RequiredGreen, snapshot.parLevelGoals.parRequiredGreen);
    Xunit.Assert.Equal(state.LevelGoals.RequiredRed, snapshot.parLevelGoals.parRequiredRed);
    Xunit.Assert.Equal(0, snapshot.parLevelProgress.parCollectedBlue);
    Xunit.Assert.Equal(0, snapshot.parLevelProgress.parCollectedGreen);
    Xunit.Assert.Equal(0, snapshot.parLevelProgress.parCollectedRed);
  }
}
