namespace CosmicCollector.Core.Services;

/// <summary>
/// Представляет значение с весом для взвешенного выбора.
/// </summary>
/// <typeparam name="T">Тип значения.</typeparam>
public sealed record WeightedOption<T>(T Value, int Weight);
