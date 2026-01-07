# События (Observer) — единый контракт

## Зачем
UI не должен «пуллить» модель, а должен реагировать на изменения состояния и перерисовываться.
Контроллеры и сервисы могут публиковать события, а View подписывается.

## Минимальный набор событий
- `GameStarted(level)`
- `GameTick(dt, tickNo)`
- `DroneMoved(x, y)`
- `ObjectSpawned(type, id)`
- `ObjectDespawned(id, reason)`
- `CrystalCollected(type, points, energyDelta)`
- `DamageTaken(sourceType, amount)`
- `BonusActivated(type, duration)`
- `PauseToggled(isPaused)`
- `CountdownTick(value)` (3..1)
- `LevelCompleted(result)` / `GameOver(result)`
- `RecordsUpdated(records)`

## Требование
События должны жить в общих библиотеках (Core/MVC), чтобы оба UI использовали один и тот же контракт.
