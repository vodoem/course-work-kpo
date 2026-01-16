namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Фабрика игровых сессий для консольного UI.
/// </summary>
public interface IGameSessionFactory
{
  /// <summary>
  /// Создаёт новый игровой сеанс.
  /// </summary>
  /// <returns>Игровой сеанс.</returns>
  GameSession Create();
}
