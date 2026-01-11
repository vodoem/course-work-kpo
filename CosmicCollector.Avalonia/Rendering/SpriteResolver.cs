using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace CosmicCollector.Avalonia.Rendering;

/// <summary>
/// Резолвер спрайтов с fallback-отрисовкой при отсутствии PNG.
/// </summary>
public sealed class SpriteResolver
{
  private readonly Uri _baseUri;
  private readonly Dictionary<string, Bitmap> _cache = new();

  /// <summary>
  /// Инициализирует новый экземпляр <see cref="SpriteResolver"/>.
  /// </summary>
  public SpriteResolver()
  {
    _baseUri = new Uri("avares://CosmicCollector.Avalonia/Assets/Sprites/");
  }

  /// <summary>
  /// Возвращает контрол спрайта или fallback.
  /// </summary>
  /// <param name="parSpriteKey">Ключ спрайта без расширения.</param>
  /// <returns>Контрол спрайта.</returns>
  public Control Resolve(string parSpriteKey)
  {
    if (TryLoadBitmap(parSpriteKey, out var bitmap) && bitmap is not null)
    {
      return new Image
      {
        Source = bitmap,
        Stretch = Stretch.Uniform
      };
    }

    return BuildFallback(parSpriteKey);
  }

  private bool TryLoadBitmap(string parSpriteKey, out Bitmap? outBitmap)
  {
    outBitmap = null;

    if (string.IsNullOrWhiteSpace(parSpriteKey))
    {
      return false;
    }

    if (_cache.TryGetValue(parSpriteKey, out var cached))
    {
      outBitmap = cached;
      return true;
    }

    var spriteUri = new Uri(_baseUri, $"{parSpriteKey}.png");

    try
    {
      using var stream = AssetLoader.Open(spriteUri);
      var bitmap = new Bitmap(stream);
      _cache[parSpriteKey] = bitmap;
      outBitmap = bitmap;
      return true;
    }
    catch
    {
      return false;
    }
  }

  private static Control BuildFallback(string parSpriteKey)
  {
    var label = GetFallbackLabel(parSpriteKey);
    var brush = new SolidColorBrush(GetFallbackColor(parSpriteKey));

    return new Border
    {
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

  private static string GetFallbackLabel(string parSpriteKey)
  {
    return parSpriteKey switch
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

  private static Color GetFallbackColor(string parSpriteKey)
  {
    return parSpriteKey switch
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
