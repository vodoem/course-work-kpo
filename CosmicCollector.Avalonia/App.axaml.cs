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
  /// <summary>
  /// Загружает ресурсы XAML приложения.
  /// </summary>
  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }

  /// <summary>
  /// Собирает граф навигации и инициализирует главные ViewModel.
  /// </summary>
  /// <remarks>
  /// Выполняется один раз при старте и создаёт общие сервисы (runtime, хранилище навигации).
  /// </remarks>
  public override void OnFrameworkInitializationCompleted()
  {
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      var navigationStore = new NavigationStore();
      var mainWindow = new MainWindow(navigationStore);
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
        return new GameOverViewModel(
          CreateNavigationService(CreateGameViewModel),
          CreateNavigationService(CreateMainMenuViewModel),
          recordsRepository);
      }

      navigationStore.CurrentViewModel = CreateMainMenuViewModel();
      desktop.MainWindow = mainWindow;
    }

    base.OnFrameworkInitializationCompleted();
  }
}
