# Prompt cookbook для Codex (готовые шаблоны)

## Универсальный шаблон
**Задача:** <одним абзацем, что нужно сделать>
**Контекст:** смотри `docs/spec/requirements.md` и `docs/architecture/*`.
**Ограничения:** без таймеров, net8.0, MVC, 2 UI, общая логика в библиотеках, стиль C#.
**Сделано когда:**
- `dotnet build` проходит
- `dotnet test` проходит
- чек-лист `docs/spec/acceptance_checklist.md` не нарушен
**Команды проверки:**
- `dotnet build`
- `dotnet test`
- (опционально) запуск Console/Avalonia проектов

Пожалуйста:
1) сначала предложи план из 5–12 пунктов;
2) затем внеси изменения минимальным диффом;
3) добавь/обнови тесты.

---

## 1) Bootstrap solution
«Создай Solution net8.0 с проектами по `docs/architecture/solution_structure.md`.
Настрой ссылки проектов, добавь пустые интерфейсы View/Controller, добавь сборку и базовый запуск.
Никаких таймеров.»

## 2) Реализовать EventBus и доменные события
«Добавь event bus в `CosmicCollector.MVC` и минимальный набор событий из `docs/architecture/events.md`.
Сделай так, чтобы Controller публиковал события, а View мог подписываться.
Покажи пример подписки в ConsoleView и AvaloniaView.»

## 3) Реализовать GameWorldUpdateService (только Core)
«Реализуй `GameWorldUpdateService` и модели объектов по `docs/spec/gameplay.md`.
Случайность вынеси в `IRandomProvider`.
Обновление мира детерминировано при фиксированном seed.»

## 4) Набить 20+ unit-тестов
«Напиши ≥20 unit-тестов для `GameWorldUpdateService` по `docs/testing/unit_tests_target.md`.
Используй фиксированный рандом/seed, чтобы тесты не флапали.»

## 5) Проверка запрета на таймеры
«Пробегись по решению и убедись, что нет `DispatcherTimer`, `System.Timers.Timer`, `System.Threading.Timer`.
Если есть — замени на game loop + Stopwatch + Sleep. Добавь короткую заметку в README_CODEX.md.»
