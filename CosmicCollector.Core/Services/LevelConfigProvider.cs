using System;

namespace CosmicCollector.Core.Services;

/// <summary>
/// Поставщик конфигурации уровней.
/// </summary>
public sealed class LevelConfigProvider
{
  private static readonly LevelConfig[] Configs =
  {
    new LevelConfig(2, 1, 1, 20, 60),
    new LevelConfig(3, 2, 2, 35, 55),
    new LevelConfig(4, 3, 2, 50, 50),
    new LevelConfig(5, 3, 3, 70, 45),
    new LevelConfig(6, 4, 3, 90, 40)
  };

  /// <summary>
  /// Возвращает конфигурацию для уровня.
  /// </summary>
  /// <param name="parLevel">Номер уровня.</param>
  /// <returns>Конфигурация уровня.</returns>
  public LevelConfig GetConfig(int parLevel)
  {
    if (Configs.Length == 0)
    {
      throw new InvalidOperationException("Отсутствует конфигурация уровней.");
    }

    var level = parLevel < 1 ? 1 : parLevel;
    var index = Math.Min(level - 1, Configs.Length - 1);
    return Configs[index];
  }
}
