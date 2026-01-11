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

  /// <inheritdoc />
  public event EventHandler? CanExecuteChanged;

  /// <inheritdoc />
  public bool CanExecute(object? parParameter)
  {
    return _canExecute?.Invoke() ?? true;
  }

  /// <inheritdoc />
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
