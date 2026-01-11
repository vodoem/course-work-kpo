using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// Base class for view models.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
  /// <inheritdoc />
  public event PropertyChangedEventHandler? PropertyChanged;

  /// <summary>
  /// Raises the <see cref="PropertyChanged"/> event.
  /// </summary>
  /// <param name="parPropertyName">Changed property name.</param>
  protected void OnPropertyChanged([CallerMemberName] string? parPropertyName = null)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(parPropertyName));
  }
}
