namespace CosmicCollector.Avalonia.Rendering;

/// <summary>
/// Описывает слои отрисовки объектов.
/// </summary>
public enum RenderLayer
{
  /// <summary>
  /// Нижний слой.
  /// </summary>
  Background = 0,

  /// <summary>
  /// Чёрные дыры.
  /// </summary>
  BlackHole = 10,

  /// <summary>
  /// Дрон.
  /// </summary>
  Drone = 20,

  /// <summary>
  /// Астероиды.
  /// </summary>
  Asteroid = 30,

  /// <summary>
  /// Бонусы.
  /// </summary>
  Bonus = 40,

  /// <summary>
  /// Кристаллы.
  /// </summary>
  Crystal = 50
}
