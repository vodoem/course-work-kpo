using Avalonia.Media;

namespace CosmicCollector.Avalonia.Rendering;

/// <summary>
/// Возвращает параметры fallback-отрисовки спрайтов.
/// </summary>
public static class SpriteFallbackProvider
{
  /// <summary>
  /// Возвращает метку для fallback-спрайта.
  /// </summary>
  /// <param name="parSpriteKey">Ключ спрайта.</param>
  /// <returns>Текст метки.</returns>
  public static string GetLabel(string parSpriteKey)
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

  /// <summary>
  /// Возвращает цвет fallback-спрайта.
  /// </summary>
  /// <param name="parSpriteKey">Ключ спрайта.</param>
  /// <returns>Цвет.</returns>
  public static Color GetColor(string parSpriteKey)
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
