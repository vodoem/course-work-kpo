using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CosmicCollector.Avalonia.ViewModels;

/// <summary>
/// Базовый класс ViewModel.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
  /// <inheritdoc />
  public event PropertyChangedEventHandler? PropertyChanged;

  /// <summary>
  /// Уведомляет об изменении свойства.
  /// </summary>
  /// <param name="parPropertyName">Имя свойства.</param>
  protected void OnPropertyChanged([CallerMemberName] string? parPropertyName = null)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(parPropertyName));
  }
}
