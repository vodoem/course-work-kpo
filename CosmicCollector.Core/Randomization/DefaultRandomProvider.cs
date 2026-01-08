namespace CosmicCollector.Core.Randomization;

/// <summary>
/// Провайдер случайных чисел на основе <see cref="System.Random"/>.
/// </summary>
public sealed class DefaultRandomProvider : IRandomProvider
{
  private readonly Random _random;

  /// <summary>
  /// Инициализирует провайдер.
  /// </summary>
  public DefaultRandomProvider()
  {
    _random = new Random();
  }

  /// <summary>
  /// Инициализирует провайдер с заданным seed.
  /// </summary>
  /// <param name="parSeed">Seed генератора.</param>
  public DefaultRandomProvider(int parSeed)
  {
    _random = new Random(parSeed);
  }

  /// <inheritdoc />
  public int NextInt(int parMinInclusive, int parMaxInclusive)
  {
    return _random.Next(parMinInclusive, parMaxInclusive + 1);
  }
}
