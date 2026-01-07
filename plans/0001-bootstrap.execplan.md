# ExecPlan: Bootstrap solution + skeleton

## Goal
Создать solution (net8.0) с правильным разбиением на проекты и контрактами MVC, чтобы дальше на это наращивать игровую логику и UI.

## Non-goals
- Реальная отрисовка игры
- Полный геймплей

## Source of truth
- docs/spec/requirements.md
- docs/architecture/solution_structure.md
- docs/spec/csharp_style_summary.md

## Constraints
- Без таймеров
- UI-логика отделена от Core/MVC
- Стиль C# соблюдён

## Design
- Проекты: Core/MVC/Persistence/Console/Avalonia/Tests
- Базовые интерфейсы: IGameView, IMenuView, IGameController, IMenuController
- EventBus: простая публикация/подписка

## Milestones
- M1: создать solution и проекты, настроить refs
- M2: добавить пустые интерфейсы и базовые модели
- M3: минимальный запуск ConsoleApp (показывает меню текстом)
- M4: минимальный запуск AvaloniaApp (пустое окно)

## Validation
- dotnet build
- dotnet test (пока может быть 0 тестов)
