namespace CosmicCollector.Core.Geometry;

/// <summary>
/// Представляет двумерный вектор.
/// </summary>
public readonly struct Vector2
{
  /// <summary>
  /// Инициализирует вектор.
  /// </summary>
  /// <param name="parX">Координата X.</param>
  /// <param name="parY">Координата Y.</param>
  public Vector2(double parX, double parY)
  {
    X = parX;
    Y = parY;
  }

  /// <summary>
  /// Координата X.
  /// </summary>
  public double X { get; }

  /// <summary>
  /// Координата Y.
  /// </summary>
  public double Y { get; }

  /// <summary>
  /// Возвращает нулевой вектор.
  /// </summary>
  public static Vector2 Zero => new(0, 0);

  /// <summary>
  /// Складывает два вектора.
  /// </summary>
  /// <param name="parOther">Вектор для сложения.</param>
  /// <returns>Сумма векторов.</returns>
  public Vector2 Add(Vector2 parOther)
  {
    return new Vector2(X + parOther.X, Y + parOther.Y);
  }

  /// <summary>
  /// Умножает вектор на скаляр.
  /// </summary>
  /// <param name="parScalar">Скаляр.</param>
  /// <returns>Новый вектор.</returns>
  public Vector2 Multiply(double parScalar)
  {
    return new Vector2(X * parScalar, Y * parScalar);
  }

  /// <summary>
  /// Возвращает длину вектора.
  /// </summary>
  /// <returns>Длина вектора.</returns>
  public double Length()
  {
    return Math.Sqrt(X * X + Y * Y);
  }

  /// <summary>
  /// Возвращает нормализованный вектор.
  /// </summary>
  /// <returns>Нормализованный вектор.</returns>
  public Vector2 Normalize()
  {
    var length = Length();

    if (length <= 0)
    {
      return Zero;
    }

    return new Vector2(X / length, Y / length);
  }
}
