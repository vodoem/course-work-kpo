using Avalonia;

namespace CosmicCollector.Avalonia;

/// <summary>
/// Точка входа Avalonia UI.
/// </summary>
internal static class Program
{
  /// <summary>
  /// Главный метод приложения.
  /// </summary>
  /// <param name="parArgs">Аргументы командной строки.</param>
  public static void Main(string[] parArgs)
  {
    BuildAvaloniaApp()
      .StartWithClassicDesktopLifetime(parArgs);
  }

  /// <summary>
  /// Настраивает Avalonia AppBuilder.
  /// </summary>
  /// <returns>Сконфигурированный builder.</returns>
  public static AppBuilder BuildAvaloniaApp()
  {
    return AppBuilder.Configure<App>()
      .UsePlatformDetect()
      .LogToTrace();
  }
}
