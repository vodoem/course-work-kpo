using System;
using System.Windows.Input;

namespace CosmicCollector.Avalonia.Commands;

/// <summary>
/// Простая команда для действий UI.
/// </summary>
public sealed class DelegateCommand : ICommand
{
  private readonly Action _execute;
  private readonly Func<bool>? _canExecute;

  /// <summary>
  /// Инициализирует новый экземпляр <see cref="DelegateCommand"/>.
  /// </summary>
  /// <param name="parExecute">Действие команды.</param>
  /// <param name="parCanExecute">Проверка доступности команды.</param>
  public DelegateCommand(Action parExecute, Func<bool>? parCanExecute = null)
  {
    _execute = parExecute;
    _canExecute = parCanExecute;
  }

  /// <summary>
  /// Событие доступности команды.
  /// </summary>
  public event EventHandler? CanExecuteChanged;

  /// <summary>
  /// Возвращает доступность команды через делегат.
  /// </summary>
  /// <param name="parParameter">Параметр команды (не используется).</param>
  /// <returns>true, если команда доступна.</returns>
  public bool CanExecute(object? parParameter)
  {
    return _canExecute?.Invoke() ?? true;
  }

  /// <summary>
  /// Выполняет действие команды.
  /// </summary>
  /// <param name="parParameter">Параметр команды (не используется).</param>
  public void Execute(object? parParameter)
  {
    _execute();
  }

  /// <summary>
  /// Уведомляет об изменении доступности команды.
  /// </summary>
  public void RaiseCanExecuteChanged()
  {
    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
  }
}
