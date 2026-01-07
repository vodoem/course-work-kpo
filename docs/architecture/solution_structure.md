# Архитектура решения (Solution) и проекты

## Цели разбиения
- Общая логика (MVC, модели, контроллеры, сервисы) — переиспользуется в обоих UI.
- UI-проекты — только отображение, ввод и привязки.

## Рекомендуемые проекты (net8.0)
1) `CosmicCollector.Core` (class library)
   - Model (состояние игры)
   - Доменные объекты (Drone, Crystal, Asteroid, BlackHole, Bonus...)
   - Сервисы расчёта (обновление мира, коллизии, цели уровня)
   - Доменные события (Observer)

2) `CosmicCollector.MVC` (class library)
   - Контракты View (интерфейсы) и Controller
   - Базовые классы MVC
   - EventBus (или простая шина событий)

3) `CosmicCollector.Persistence` (class library)
   - Загрузка/сохранение records.json (RecordsRepository)

4) `CosmicCollector.ConsoleApp` (console, net8.0)
   - Реализации View для консоли
   - Обработка ввода (клавиатура)
   - Без логики мира (вызовы контроллера, отрисовка состояния)

5) `CosmicCollector.AvaloniaApp` (Avalonia, net8.0)
   - Реализации View для Avalonia
   - Отрисовка (canvas/custom drawing)
   - Обработка ввода (KeyDown/PointerMoved)
   - Без логики мира

6) `CosmicCollector.Tests` (xUnit/NUnit)
   - ≥20 unit-тестов для выбранного класса

## Правило зависимостей
- UI -> (MVC + Core + Persistence)
- MVC -> Core
- Core -> ничего UI-специфичного
- Persistence -> Core (модели рекордов) или DTO

Запрещены зависимости:
- Core/MVC/Persistence -> UI-проекты.
