1\) Точка входа и сборка зависимостей

Где создаются ключевые объекты

Точка входа и DI‑сборка: CosmicCollector.ConsoleApp.Program.Main() создаёт ConsoleRenderer, ConsoleInputReader, FileRulesTextProvider, GameSessionFactory, RecordsRepository, а затем MainMenuView и MainMenuController и запускает controller.Run().



GameSession создаётся в GameSessionFactory.Create(): внутри создаются GameState, EventBus, CommandQueue, DefaultRandomProvider, LevelService(LevelConfigProvider), GameWorldUpdateService(SpawnConfig.Default), GameSnapshotProvider, GameLoopRunner и инициализация дрона + уровня, затем всё упаковывается в GameSession.



GameSession хранит EventBus, IGameLoopRunner, IGameSnapshotProvider, CommandQueue, WorldBounds, Level.



Где “истина” (GameState) и кто её обновляет

Источник истины — GameState (CosmicCollector.Core.Model.GameState), создаётся в GameSessionFactory и передаётся в GameLoopRunner и GameSnapshotProvider.



GameState обновляется в GameWorldUpdateService.Update(...), вызываемом в колбэке GameLoopRunner (см. parDt => updateService.Update(...)).



Параметры/конфиги (ключевые)

Шаг игрового цикла фиксирован: StepSeconds = 1.0 / 60.0 в GameLoopRunner.



SpawnConfig.Default содержит ключевые параметры (включённый спавн, лимиты активных объектов, интервалы спавна, базовые скорости, радиусы чёрных дыр, длительность бонусов, веса типов и пр.). См. определение SpawnConfig.Default.



LevelConfigProvider задаёт таблицу уровней (blue/green/red, requiredScore, levelTimeSec): уровни 1–5 с параметрами (2,1,1,20,60) ... (6,4,3,90,40).



GameWorldUpdateService содержит ключевые коэффициенты и длительности: SoftMargin, урон астероидов, ускорения, множители бонусов, длительность дезориентации, базовое время стабилизатора и т.д. — все в константах класса.



2\) Потоки и жизненный цикл

Потоки

Game loop thread: создаётся в GameLoopRunner.Start() (поток GameLoop, IsBackground = true).



Input thread: создаётся в GameScreenController.StartInputLoop() (поток ConsoleInput).



Старт/стоп цикла

Старт: GameScreenController.Run() публикует GameStarted, запускает GameLoopRunner.Start() и стартует input loop. Затем рендерит кадры по сигналу \_tickSignal из OnGameTick.



Стоп: после выхода из цикла Run() вызывается \_gameLoopRunner.Stop() и StopInputLoop() (Cancel + Join).



Выход/остановка игры

GameOver: обработчик OnGameOver → HandleGameEnd, который отключает ввод, выставляет флаг выхода, сигналит \_tickSignal и завершает цикл отрисовки. Затем Run() вызывает GameEndController.



Выход в меню из паузы: в HandlePauseMenuAction выставляются флаги \_exitToMenuRequestedFlag, \_isInputEnabledFlag, \_shouldExitGameScreenFlag, останавливается цикл рендера и подаётся \_tickSignal.



Риск утечек подписок

GameScreenController подписывается на события через EventBus.Subscribe, но явного Dispose() на подписках нет. Это потенциальный риск «залипания» обработчиков при повторных сессиях, если EventBus живёт дольше контроллера (в консоли создаётся новый GameSession каждый раз).



Механизм отписки есть (Subscription.Dispose() вызывает EventBus.Unsubscribe), но в GameScreenController он не используется сейчас.



3\) События (EventBus): полный список

Подписчик в консоли только один — GameScreenController (подписки на GameStarted/GameTick/PauseToggled/CountdownTick/GameOver/LevelCompleted).



События и их цепочка

GameStarted



Публикатор: GameScreenController.Run() публикует new GameStarted(\_level) перед стартом цикла.



Подписчик: GameScreenController.OnGameStarted обновляет \_level.



Файлы: CosmicCollector.Core.Events.GameStarted, CosmicCollector.ConsoleApp.Controllers.GameScreenController.



GameTick



Публикатор: GameLoopRunner.RunLoop() (и ManualGameLoopRunner.TickOnce() в тестовой ветке) публикуют new GameTick(StepSeconds, tickNo).



Подписчик: GameScreenController.OnGameTick берёт snapshot и будит рендер.



Файлы: CosmicCollector.MVC.Loop.GameLoopRunner, CosmicCollector.MVC.Loop.ManualGameLoopRunner, CosmicCollector.ConsoleApp.Controllers.GameScreenController.{line\_range\_start=100 line\_range\_end=146 path=CosmicCollector.ConsoleApp/Controllers/GameScreenController.csL157-L172】



PauseToggled



Публикатор 1: GameLoopRunner.ProcessCommands / ManualGameLoopRunner.ProcessCommands при TogglePauseCommand вызывает GameState.TogglePause() и публикует PauseToggled.



Публикатор 2: GameWorldUpdateService.ProcessResumeCountdown публикует PauseToggled(false) при завершении отсчёта.



Подписчик: GameScreenController.OnPauseToggled обновляет \_isPaused, открывает/закрывает меню паузы и сбрасывает ввод.



Файлы: GameLoopRunner, ManualGameLoopRunner, GameWorldUpdateService, GameScreenController.【F:CosmicCollector.MVC/Loop/GameLoopRunner.cs git\_url="https://github.com/vodoem/course-work-kpo/blob/master/CosmicCollector.ConsoleApp/Controllers/GameScreenController.csL157-L172】



PauseToggled



Публикатор 1: GameLoopRunner.ProcessCommands / ManualGameLoopRunner.ProcessCommands при TogglePauseCommand вызывает GameState.TogglePause() и публикует PauseToggled.



Публикатор 2: GameWorldUpdateService.ProcessResumeCountdown публикует PauseToggled(false) при завершении отсчёта.



Подписчик: GameScreenController.OnPauseToggled обновляет \_isPaused, открывает/закрывает меню паузы и сбрасывает ввод.



Файлы: GameLoopRunner, ManualGameLoopRunner, GameWorldUpdateService, GameScreenController.【F:CosmicCollector.MVC/Loop/GameLoopRunner.cs#L100-L146"}



CountdownTick



Публикатор: GameWorldUpdateService.ProcessResumeCountdown публикует значения 3..1 и т.д.



Подписчик: GameScreenController.OnCountdownTick обновляет \_countdownValue.



Файлы: GameWorldUpdateService, GameScreenController.



GameOver



Публикатор: GameWorldUpdateService.CheckEndConditions при истечении времени уровня или при энергии <= 0.



Подписчик: GameScreenController.OnGameOver → HandleGameEnd, завершает игровой экран.



Файлы: GameWorldUpdateService, GameScreenController.



LevelCompleted



Публикатор: GameWorldUpdateService.CheckEndConditions при выполнении целей уровня вызывает LevelService.AdvanceLevel и публикует LevelCompleted.



Подписчик: GameScreenController.OnLevelCompleted обновляет \_level из snapshot (игра продолжается).



Файлы: GameWorldUpdateService, LevelService, GameScreenController.



BonusActivated / CrystalCollected / DamageTaken / ObjectSpawned / ObjectDespawned



Публикатор: все эти события публикуются внутри GameWorldUpdateService при соответствующих игровых действиях (сбор кристаллов, урон, спавн/деспаун).



Подписчики в ConsoleApp: отсутствуют (подписок нет).



Файлы: GameWorldUpdateService, события в CosmicCollector.Core.Events.



RecordsUpdated



Публикатор: в коде не найден (событие только объявлено).



Подписчики: отсутствуют.



Файл: CosmicCollector.Core.Events.RecordsUpdated.



4\) Команды (CommandQueue): полный список

Команды определены в CosmicCollector.MVC.Commands:



TogglePauseCommand, SetMoveDirectionCommand, MoveLeftCommand, MoveRightCommand.



Кто создаёт и кто обрабатывает

Создаёт консольный UI (GameScreenController):



TogglePauseCommand — при нажатии паузы в ApplyInputState.



SetMoveDirectionCommand — при изменении направления (−1/0/1).



Обрабатывает движок (MVC loop):



GameLoopRunner.ProcessCommands (и ManualGameLoopRunner для тестов) применяет команды:



TogglePauseCommand → GameState.TogglePause() + PauseToggled



SetMoveDirectionCommand / MoveLeftCommand / MoveRightCommand → GameState.SetDroneMoveDirection(...)



Эффекты на GameState

TogglePauseCommand изменяет GameState.IsPaused (включает паузу или инициирует resume‑countdown).



SetMoveDirectionCommand и MoveLeft/MoveRight меняют GameState направление дрона через SetDroneMoveDirection (вызывается из loop).



Важно: выход в меню из паузы и действия на экране GameOver не реализованы командами — это локальная логика GameScreenController/GameEndController.



5\) Snapshot: что UI читает

Тип snapshot: CosmicCollector.Core.Snapshots.GameSnapshot, создаётся GameState.GetSnapshot() и отдаётся через GameSnapshotProvider.



Поля, которые читает console‑HUD/оверлеи (GameScreenView):



parSnapshot.parCurrentLevel, parSnapshot.parRequiredScore,



parSnapshot.parDrone.parEnergy, parSnapshot.parDrone.parScore,



parSnapshot.parLevelGoals (requiredBlue/Green/Red),



parSnapshot.parLevelProgress (collected...),



parSnapshot.parHasLevelTimer + parSnapshot.parLevelTimeRemainingSec,



списки объектов: parCrystals, parAsteroids, parBonuses, parBlackHoles,



позиция дрона parDrone.parPosition.



UI‑оверлей: parCountdownValue из CountdownTick рисуется поверх (3..2..1), а parIsPaused используется для отображения меню паузы. Эти значения приходят из GameScreenController и событий, а не из snapshot. 



6\) Навигация по экранам (state machine UI)

MainMenu → Game

MainMenuController.Run() на Play создаёт GameSession и GameScreenController, затем вызывает Run(); при GameEndAction.RestartGame перезапускает сессию.



Game → PauseMenu → Resume/Exit

Пауза отправляется через TogglePauseCommand (от кнопки P), что приводит к PauseToggled и открытию PauseMenuController.



PauseMenuController имеет режим Menu и ConfirmExit, поддерживает Resume или ExitToMenu. 



ExitToMenu выставляет флаги выхода и завершает игровой экран без GameOver.



Game → GameOver/Records flow

GameWorldUpdateService публикует GameOver при тайм‑ауте или нулевой энергии. GameScreenController ловит и завершает run, после чего показывает GameEndView и запускает GameEndController.



GameEndController решает, является ли счёт рекордом; если да — сохраняет через RecordsRepository.Add, затем показывает таблицу. 



LevelCompleted

В GameWorldUpdateService.CheckEndConditions при выполнении целей уровень увеличивается (LevelService.AdvanceLevel) и публикуется LevelCompleted. Игровой экран не заканчивается, просто обновляет \_level.



7\) Инварианты/правила для Avalonia

UI “тонкий”: весь расчёт мира, коллизий, бонусов и таймеров уже в Core (GameWorldUpdateService + GameState), UI только читает snapshot и шлёт команды. 



Нет таймеров в UI: частота тиков задаётся GameLoopRunner (StepSeconds = 1/60) и обработкой GameTick. UI должен реагировать на события/снимки, а не на UI‑таймеры. 



GameOver только по событию: экран завершения запускается исключительно из OnGameOver (событие от движка), а не по кнопке. 



Пауза + отсчёт: TogglePause включает паузу; при снятии паузы запускается отсчёт 3..1 в GameWorldUpdateService, который блокирует обновление мира и выдаёт CountdownTick до снятия паузы (PauseToggled(false)). 



Корректный stop/dispose: уход со страницы должен останавливать GameLoopRunner и input loop; в консоли это делает GameScreenController.Run() после выхода. 



8\) Минимальный API‑слой/фасад для Avalonia

Что можно переиспользовать

GameSessionFactory + GameSession — удобный фасад сборки зависимостей для UI (EventBus, CommandQueue, GameLoopRunner, SnapshotProvider, WorldBounds, Level). Это прямой аналог того, что нужно в Avalonia. 



GameSnapshotProvider — простой адаптер к GameState.GetSnapshot(). 



Что console‑специфично и не стоит копировать

ConsoleRenderer, ConsoleInputReader, ConsoleKeyStateProvider, WindowsKeyStateProvider, GameScreenView, PauseMenuView, GameEndView и всё в ConsoleApp.Views — это UI‑слой консоли. 



TL;DR для Avalonia (10–15 строк)

Entry: в консоли всё создаётся в Program.Main() и GameSessionFactory.Create().



Source of truth — GameState из Core; обновляется только GameWorldUpdateService.Update(...).



Game loop — GameLoopRunner (thread GameLoop, шаг 1/60). UI не таймерит. 



UI получает кадры через GameTick → GameSnapshotProvider.GetSnapshot() → GameScreenView.Render(...).



Команды UI: TogglePauseCommand и SetMoveDirectionCommand (остальное — не требуется консоли).



Пауза: TogglePause → PauseToggled(true); выход из паузы идёт через countdown 3..1 в Core с CountdownTick.



GameOver и LevelCompleted публикуются Core; UI слушает и реагирует. LevelCompleted не завершает игру. 



Рекорды — RecordsRepository в Persistence; GameEndController вызывает Add(...).



Снимок: GameSnapshot содержит уровень, очки, энергию, цели/прогресс, таймер и списки объектов — UI читает эти поля. 



Lifecycle: при выходе — остановить GameLoop + input loop. В консоли делает GameScreenController.Run().

