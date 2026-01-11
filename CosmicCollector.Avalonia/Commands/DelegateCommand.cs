using System;
using System.Windows.Input;

namespace CosmicCollector.Avalonia.Commands;

/// <summary>
/// Basic command wrapper for UI actions.
/// </summary>
public sealed class DelegateCommand : ICommand
{
  private readonly Action _execute;
  private readonly Func<bool>? _canExecute;

  /// <summary>
  /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
  /// </summary>
  /// <param name="parExecute">Action to execute.</param>
  /// <param name="parCanExecute">Predicate to allow execution.</param>
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
  /// Triggers a re-evaluation of <see cref="CanExecute"/>.
  /// </summary>
  public void RaiseCanExecuteChanged()
  {
    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
  }
}
