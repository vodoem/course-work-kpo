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

    Xunit.Assert.Equal(120, config.CrystalBaseSpeed);
    Xunit.Assert.Equal(150, config.AsteroidBaseSpeed);
    Xunit.Assert.Equal(120, config.BonusBaseSpeed);
    Xunit.Assert.Equal(120, config.BlackHoleBaseSpeed);
    Xunit.Assert.InRange(config.BlackHoleRadius, (220.0 / 1.5) - 0.001, (220.0 / 1.5) + 0.001);
    Xunit.Assert.Equal(40, config.BlackHoleCoreRadius);
  }
}
