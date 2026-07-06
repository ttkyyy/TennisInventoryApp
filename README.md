## Работа с БД из кода/Задание

Написать консольную утилиту, реализующую CRUD-запросы к БД через:

- ADO.NET.
- Entity Framework.

Адрес сервера СУБД должен задаваться в строке подключения из конфига консольной утилиты встроенными средствами [web:34][web:35][web:36].

## Запуск проекта

1. Открой терминал в папке проекта.
2. Выполни команду:

```bash
dotnet run
```

## Настройка базы данных

Перед запуском проверь строку подключения в `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-L7JJ04J\\SQLEXPRESS;Database=TableTennisInventory;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Если у тебя другое имя SQL Server или другая база данных, замени их на свои значения.
