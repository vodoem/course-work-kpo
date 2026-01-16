using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace CosmicCollector.Avalonia.Rendering;

/// <summary>
/// Кэширует спрайты из ресурсов Avalonia.
/// </summary>
public sealed class SpriteAtlas
{
  private readonly Uri _baseUri;
  private readonly Dictionary<string, Bitmap> _cache = new();

  /// <summary>
  /// Инициализирует новый экземпляр <see cref="SpriteAtlas"/>.
  /// </summary>
  public SpriteAtlas()
  {
    _baseUri = new Uri("avares://CosmicCollector.Avalonia/Assets/Sprites/");
  }

  /// <summary>
  /// Пытается получить bitmap спрайта.
  /// </summary>
  /// <param name="parSpriteKey">Ключ спрайта без расширения.</param>
  /// <param name="outBitmap">Найденный bitmap.</param>
  /// <returns>true, если bitmap найден.</returns>
  public bool TryGetBitmap(string parSpriteKey, out Bitmap? outBitmap)
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
}
