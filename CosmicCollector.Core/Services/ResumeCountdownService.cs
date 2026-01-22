using CosmicCollector.Core.Events;
using CosmicCollector.Core.Model;

namespace CosmicCollector.Core.Services;

/// <summary>
/// Управляет обратным отсчётом для снятия паузы.
/// </summary>
public sealed class ResumeCountdownService
{
  /// <summary>
  /// Обрабатывает обратный отсчёт на шаге игрового цикла.
  /// </summary>
  /// <param name="parGameState">Состояние игры.</param>
  /// <param name="parDt">Длительность шага.</param>
  /// <param name="parEventPublisher">Публикатор событий.</param>
  /// <returns>Признак завершения отсчёта.</returns>
  public bool Process(
    GameState parGameState,
    double parDt,
    IEventPublisher parEventPublisher)
  {
    if (!parGameState.IsResumeCountdownActive)
    {
      return false;
    }

    if (parGameState.IsResumeCountdownJustStarted)
    {
      var startValue = parGameState.ResumeCountdownValue;
      parEventPublisher.Publish(new CountdownTick(startValue));
      parGameState.IsResumeCountdownJustStarted = false;
    }

    parGameState.ResumeCountdownAccumulatedSec += parDt;

    while (parGameState.ResumeCountdownAccumulatedSec >= 1.0 && parGameState.IsResumeCountdownActive)
    {
      parGameState.ResumeCountdownAccumulatedSec -= 1.0;
      var value = parGameState.ResumeCountdownValue - 1;
      parGameState.ResumeCountdownValue = value;

      if (value <= 0)
      {
        parGameState.StopResumeCountdown();
        parGameState.SetPaused(false);
        parEventPublisher.Publish(new PauseToggled(false));
        return true;
      }

      parEventPublisher.Publish(new CountdownTick(value));
    }

    return false;
  }
}
