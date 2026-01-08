using CosmicCollector.Core.Services;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет конфигурацию спавна по умолчанию.
/// </summary>
public sealed class SpawnConfigTests
{
  /// <summary>
  /// Проверяет значения из ТЗ в конфигурации по умолчанию.
  /// </summary>
  [Xunit.Fact]
  public void SpawnConfig_DefaultUsesSpecValues()
  {
    var config = SpawnConfig.Default;

    Xunit.Assert.Equal(100, config.CrystalBaseSpeed);
    Xunit.Assert.Equal(150, config.AsteroidBaseSpeed);
    Xunit.Assert.Equal(100, config.BonusBaseSpeed);
    Xunit.Assert.Equal(100, config.BlackHoleBaseSpeed);
    Xunit.Assert.Equal(220, config.BlackHoleRadius);
    Xunit.Assert.Equal(40, config.BlackHoleCoreRadius);
  }
}
