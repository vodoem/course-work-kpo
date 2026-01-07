namespace CosmicCollector.MVC.Loop;

/// <summary>
/// Определяет запуск и остановку игрового цикла.
/// </summary>
public interface IGameLoopRunner : IDisposable
{
  /// <summary>
  /// Запускает игровой цикл.
  /// </summary>
  void Start();

  /// <summary>
  /// Останавливает игровой цикл.
  /// </summary>
  void Stop();
}
