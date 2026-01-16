using System;

namespace CosmicCollector.ConsoleApp.Infrastructure;

/// <summary>
/// Абстракция проверки состояния клавиш.
/// </summary>
public interface IKeyStateProvider
{
  /// <summary>
  /// Проверяет, нажата ли клавиша.
  /// </summary>
  /// <param name="parKey">Клавиша.</param>
  /// <returns>Признак нажатой клавиши.</returns>
  bool IsKeyDown(ConsoleKey parKey);
}
