using System.Collections.Concurrent;

namespace CosmicCollector.MVC.Commands;

/// <summary>
/// Потокобезопасная очередь игровых команд.
/// </summary>
public sealed class CommandQueue
{
  private readonly ConcurrentQueue<IGameCommand> _commands = new();

  /// <summary>
  /// Добавляет команду в очередь.
  /// </summary>
  /// <param name="parCommand">Команда для добавления.</param>
  public void Enqueue(IGameCommand parCommand)
  {
    _commands.Enqueue(parCommand);
  }

  /// <summary>
  /// Снимает все накопленные команды в порядке FIFO.
  /// </summary>
  /// <returns>Список команд для обработки.</returns>
  public IReadOnlyList<IGameCommand> DrainAll()
  {
    var drained = new List<IGameCommand>();

    while (_commands.TryDequeue(out var command))
    {
      drained.Add(command);
    }

    return drained;
  }
}
