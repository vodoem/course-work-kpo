# Формат файла records.json

Файл хранит рекорды между запусками (UTF-8, JSON).

## Структура
- `formatVersion` (int) — версия формата.
- `records` (array) — записи.

## Одна запись
- `playerName` (string)
- `score` (int)
- `level` (int)
- `timestampUtc` (string, ISO 8601, UTC)

## Пример
```json
{
  "formatVersion": 1,
  "records": [
    {
      "playerName": "Vadim",
      "score": 12450,
      "level": 18,
      "timestampUtc": "2025-11-25T20:14:33Z"
    }
  ]
}
```
