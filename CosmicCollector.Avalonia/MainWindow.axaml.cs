using System;
using Avalonia.Controls;
using CosmicCollector.Avalonia.Navigation;
using CosmicCollector.Avalonia.ViewModels;
using CosmicCollector.Avalonia.Views;

namespace CosmicCollector.Avalonia;

/// <summary>
/// Главное окно Avalonia UI.
/// </summary>
public sealed partial class MainWindow : Window
{
  private readonly NavigationStore _navigationStore;

  /// <summary>
  /// Инициализирует новый экземпляр <see cref="MainWindow"/>.
  /// </summary>
  /// <param name="parNavigationStore">Хранилище навигации.</param>
  /// <exception cref="ArgumentNullException">Если хранилище не передано.</exception>
  public MainWindow(NavigationStore parNavigationStore)
  {
    _navigationStore = parNavigationStore ?? throw new ArgumentNullException(nameof(parNavigationStore));
    InitializeComponent();
    _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
    Closed += OnClosed;
    UpdateContent();
  }

  private void OnClosed(object? parSender, EventArgs parArgs)
  {
    _navigationStore.CurrentViewModelChanged -= OnCurrentViewModelChanged;
    Closed -= OnClosed;
  }

  private void OnCurrentViewModelChanged()
  {
    UpdateContent();
  }

  private void UpdateContent()
  {
    if (ContentHost is null)
    {
      return;
    }

    ContentHost.Content = CreateViewForViewModel(_navigationStore.CurrentViewModel);
  }

  private static Control? CreateViewForViewModel(ViewModelBase? parViewModel)
  {
    if (parViewModel is null)
    {
      return null;
    }

    Control view = parViewModel switch
    {
      MainMenuViewModel => new MainMenuView(),
      RulesViewModel => new RulesView(),
      RecordsViewModel => new RecordsView(),
      GameViewModel => new GameView(),
      GameOverViewModel => new GameOverView(),
      _ => new ContentControl()
    };

    view.DataContext = parViewModel;
    return view;
  }
}
