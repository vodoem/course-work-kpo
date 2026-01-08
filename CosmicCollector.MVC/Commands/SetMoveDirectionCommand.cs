namespace CosmicCollector.MVC.Commands;

/// <summary>
/// Команда установки направления движения по X.
/// </summary>
public sealed class SetMoveDirectionCommand : IGameCommand
{
  /// <summary>
  /// Инициализирует команду направления движения.
  /// </summary>
  /// <param name="parDirectionX">Направление: -1, 0 или 1.</param>
  public SetMoveDirectionCommand(int parDirectionX)
  {
    DirectionX = parDirectionX;
  }

  /// <summary>
  /// Направление движения по X: -1, 0 или 1.
  /// </summary>
  public int DirectionX { get; }
}
