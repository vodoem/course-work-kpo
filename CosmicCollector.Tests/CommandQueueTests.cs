using CosmicCollector.MVC.Commands;

namespace CosmicCollector.Tests;

/// <summary>
/// Проверяет работу очереди команд.
/// </summary>
public sealed class CommandQueueTests
{
  /// <summary>
  /// Проверяет порядок FIFO при извлечении команд.
  /// </summary>
  [Xunit.Fact]
  public void DrainAll_ReturnsCommandsInFifoOrder()
  {
    var queue = new CommandQueue();
    var first = new MoveLeftCommand();
    var second = new MoveRightCommand();
    var third = new TogglePauseCommand();

    queue.Enqueue(first);
    queue.Enqueue(second);
    queue.Enqueue(third);

    var drained = queue.DrainAll();

    Xunit.Assert.Equal(new IGameCommand[] { first, second, third }, drained);
  }

  /// <summary>
  /// Проверяет, что многопоточная постановка команд не падает.
  /// </summary>
  [Xunit.Fact]
  public async Task Enqueue_FromMultipleTasks_DoesNotThrow()
  {
    var queue = new CommandQueue();
    var tasks = new List<Task>();

    for (var i = 0; i < 10; i++)
    {
      tasks.Add(Task.Run(() => queue.Enqueue(new MoveLeftCommand())));
    }

    await Task.WhenAll(tasks);

    var drained = queue.DrainAll();

    Xunit.Assert.Equal(10, drained.Count);
  }
}
