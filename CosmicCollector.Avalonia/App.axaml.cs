using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CosmicCollector.Avalonia.Infrastructure;
using CosmicCollector.Avalonia.Navigation;
using CosmicCollector.Avalonia.ViewModels;
using CosmicCollector.Persistence.Records;

namespace CosmicCollector.Avalonia;

/// <summary>
/// Класс приложения Avalonia.
/// </summary>
public sealed class App : Application
{
  /// <inheritdoc />
  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }

  /// <inheritdoc />
  public override void OnFrameworkInitializationCompleted()
  {
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      var navigationStore = new NavigationStore();
      var mainWindow = new MainWindow();
      var gameRuntime = new GameRuntime();
      var recordsRepository = new RecordsRepository(AppDataPaths.GetRecordsFilePath());

      NavigationService CreateNavigationService(Func<ViewModelBase> parViewModelFactory)
      {
        return new NavigationService(navigationStore, parViewModelFactory);
      }

      MainMenuViewModel CreateMainMenuViewModel()
      {
        return new MainMenuViewModel(
          () => mainWindow.Close(),
          CreateNavigationService(CreateGameViewModel),
          CreateNavigationService(CreateRulesViewModel),
          CreateNavigationService(CreateRecordsViewModel));
      }

      RulesViewModel CreateRulesViewModel()
      {
        return new RulesViewModel(CreateNavigationService(CreateMainMenuViewModel));
      }

      RecordsViewModel CreateRecordsViewModel()
      {
        return new RecordsViewModel(CreateNavigationService(CreateMainMenuViewModel));
      }

      GameViewModel CreateGameViewModel()
      {
        return new GameViewModel(
          gameRuntime,
          CreateNavigationService(CreateMainMenuViewModel),
          CreateNavigationService(CreateGameOverViewModel));
      }

      GameOverViewModel CreateGameOverViewModel()
      {
        var snapshot = gameRuntime.GetSnapshot();
        return new GameOverViewModel(
          snapshot,
          CreateNavigationService(CreateGameViewModel),
          CreateNavigationService(CreateMainMenuViewModel),
          recordsRepository);
      }

      navigationStore.CurrentViewModel = CreateMainMenuViewModel();
      mainWindow.DataContext = new MainWindowViewModel(navigationStore);
      desktop.MainWindow = mainWindow;
    }

    base.OnFrameworkInitializationCompleted();
  }
}
