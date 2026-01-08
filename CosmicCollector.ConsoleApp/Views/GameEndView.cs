using CosmicCollector.ConsoleApp.Infrastructure;
using CosmicCollector.Core.Snapshots;

namespace CosmicCollector.ConsoleApp.Views;

/// <summary>
/// Представление экрана завершения игры.
/// </summary>
public sealed class GameEndView : IGameEndView
{
  private readonly IConsoleRenderer _renderer;

  /// <summary>
  /// Создаёт представление экрана завершения.
  /// </summary>
  /// <param name="parRenderer">Рендерер консоли.</param>
  public GameEndView(IConsoleRenderer parRenderer)
  {
    _renderer = parRenderer;
  }

  /// <inheritdoc />
  public void Render(GameEndReason parReason, GameSnapshot parSnapshot, int parLevel)
  {
    _renderer.Clear();
    _renderer.SetCursorPosition(0, 0);
    _renderer.SetForegroundColor(ConsoleColor.White);

    string title = parReason == GameEndReason.LevelCompleted
      ? "УРОВЕНЬ ПРОЙДЕН"
      : "ИГРА ОКОНЧЕНА";

    _renderer.WriteLine(title);
    _renderer.WriteLine(string.Empty);
    _renderer.WriteLine($"Уровень: {parLevel}");
    _renderer.WriteLine($"Очки: {parSnapshot.parDrone.parScore}");
    _renderer.WriteLine($"Энергия: {parSnapshot.parDrone.parEnergy}");

    string timeText = parSnapshot.parHasLevelTimer
      ? $"{Math.Max(0, Math.Floor(parSnapshot.parLevelTimeRemainingSec)):0}с"
      : "—";
    _renderer.WriteLine($"Время: {timeText}");
    _renderer.WriteLine(string.Empty);
    _renderer.WriteLine("Enter/Esc — в меню, R — новая игра");
  }
}
