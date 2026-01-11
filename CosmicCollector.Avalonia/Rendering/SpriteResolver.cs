using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace CosmicCollector.Avalonia.Rendering;

/// <summary>
/// Resolves sprite assets with a fallback when files are missing.
/// </summary>
public sealed class SpriteResolver
{
  private readonly IAssetLoader _assetLoader;
  private readonly Uri _baseUri;

  /// <summary>
  /// Initializes a new instance of the <see cref="SpriteResolver"/> class.
  /// </summary>
  public SpriteResolver()
    : this(AvaloniaLocator.Current.GetService<IAssetLoader>() ??
           throw new InvalidOperationException("Asset loader is not available."))
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="SpriteResolver"/> class.
  /// </summary>
  /// <param name="parAssetLoader">Asset loader instance.</param>
  public SpriteResolver(IAssetLoader parAssetLoader)
  {
    _assetLoader = parAssetLoader;
    _baseUri = new Uri("avares://CosmicCollector.Avalonia/Assets/Sprites/");
  }

  /// <summary>
  /// Attempts to load a sprite bitmap by name.
  /// </summary>
  /// <param name="parSpriteName">Sprite name without extension.</param>
  /// <param name="outBitmap">Loaded bitmap when available.</param>
  /// <returns>True if the bitmap was loaded.</returns>
  public bool TryResolve(string parSpriteName, out IBitmap? outBitmap)
  {
    outBitmap = null;

    if (string.IsNullOrWhiteSpace(parSpriteName))
    {
      return false;
    }

    var spriteUri = new Uri(_baseUri, $"{parSpriteName}.png");

    try
    {
      using var stream = _assetLoader.Open(spriteUri);
      outBitmap = new Bitmap(stream);
      return true;
    }
    catch
    {
      return false;
    }
  }

  /// <summary>
  /// Resolves a control for the sprite with a fallback if missing.
  /// </summary>
  /// <param name="parSpriteName">Sprite name without extension.</param>
  /// <param name="parWidth">Desired width.</param>
  /// <param name="parHeight">Desired height.</param>
  /// <returns>Image control or fallback element.</returns>
  public IControl Resolve(string parSpriteName, double parWidth, double parHeight)
  {
    if (TryResolve(parSpriteName, out var bitmap) && bitmap is not null)
    {
      return new Image
      {
        Source = bitmap,
        Width = parWidth,
        Height = parHeight,
        Stretch = Stretch.Uniform
      };
    }

    return BuildFallback(parSpriteName, parWidth, parHeight);
  }

  private static IControl BuildFallback(string parSpriteName, double parWidth, double parHeight)
  {
    var label = GetFallbackLabel(parSpriteName);
    var brush = new SolidColorBrush(GetFallbackColor(parSpriteName));

    return new Border
    {
      Width = parWidth,
      Height = parHeight,
      Background = brush,
      BorderBrush = Brushes.White,
      BorderThickness = new Thickness(1),
      Child = new TextBlock
      {
        Text = label,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Foreground = Brushes.White,
        FontWeight = FontWeight.Bold
      }
    };
  }

  private static string GetFallbackLabel(string parSpriteName)
  {
    return parSpriteName switch
    {
      "drone" => "D",
      "crystal_blue" => "B",
      "crystal_green" => "G",
      "crystal_red" => "R",
      "asteroid" => "A",
      "bonus_magnet" => "M",
      "bonus_accelerator" => "X",
      "bonus_time" => "T",
      "blackhole" => "BH",
      _ => "?"
    };
  }

  private static Color GetFallbackColor(string parSpriteName)
  {
    return parSpriteName switch
    {
      "drone" => Colors.SlateGray,
      "crystal_blue" => Colors.DodgerBlue,
      "crystal_green" => Colors.MediumSeaGreen,
      "crystal_red" => Colors.IndianRed,
      "asteroid" => Colors.DimGray,
      "bonus_magnet" => Colors.Goldenrod,
      "bonus_accelerator" => Colors.OrangeRed,
      "bonus_time" => Colors.MediumPurple,
      "blackhole" => Colors.Black,
      _ => Colors.DarkSlateBlue
    };
  }
}
