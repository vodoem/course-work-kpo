using Avalonia;

namespace CosmicCollector.Avalonia;

/// <summary>
/// Entry point for the Avalonia UI.
/// </summary>
internal static class Program
{
  /// <summary>
  /// Application entry point.
  /// </summary>
  /// <param name="parArgs">Command-line arguments.</param>
  public static void Main(string[] parArgs)
  {
    BuildAvaloniaApp()
      .StartWithClassicDesktopLifetime(parArgs);
  }

  /// <summary>
  /// Configures Avalonia application builder.
  /// </summary>
  /// <returns>Configured application builder.</returns>
  public static AppBuilder BuildAvaloniaApp()
  {
    return AppBuilder.Configure<App>()
      .UsePlatformDetect()
      .LogToTrace();
  }
}
