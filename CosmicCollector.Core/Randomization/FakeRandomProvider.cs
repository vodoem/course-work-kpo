namespace CosmicCollector.Core.Randomization;

/// <summary>
/// Фейковый провайдер случайных чисел для тестов.
/// </summary>
public sealed class FakeRandomProvider : IRandomProvider
{
  private readonly Queue<int> _values;

  /// <summary>
  /// Инициализирует провайдер последовательностью значений.
  /// </summary>
  /// <param name="parValues">Последовательность значений.</param>
  public FakeRandomProvider(IEnumerable<int> parValues)
  {
    _values = new Queue<int>(parValues);
  }

  /// <summary>
  /// Инициализирует провайдер одним фиксированным значением.
  /// </summary>
  /// <param name="parValue">Фиксированное значение.</param>
  public FakeRandomProvider(int parValue)
  {
    _values = new Queue<int>(new[] { parValue });
  }

  /// <inheritdoc />
  public int NextInt(int parMinInclusive, int parMaxInclusive)
  {
    if (_values.Count == 0)
    {
      return parMinInclusive;
    }

    var value = _values.Dequeue();

    if (value < parMinInclusive)
    {
      return parMinInclusive;
    }

    if (value > parMaxInclusive)
    {
      return parMaxInclusive;
    }

    return value;
  }
}
