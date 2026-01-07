namespace CosmicCollector.Core.Randomization;

/// <summary>
/// Предоставляет доступ к случайным значениям.
/// </summary>
public interface IRandomProvider
{
  /// <summary>
  /// Возвращает случайное целое число в диапазоне.
  /// </summary>
  /// <param name="parMinInclusive">Минимум (включительно).</param>
  /// <param name="parMaxInclusive">Максимум (включительно).</param>
  /// <returns>Случайное число.</returns>
  int NextInt(int parMinInclusive, int parMaxInclusive);
}
