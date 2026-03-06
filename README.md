# projectTracker - Трекер проектов - REST API
Мазур Анна Игоревна ЭФБО-02-23
Веб-служба для управления проектами и задачами с хранением данных в памяти процесса. Демонстрирует конвейер обработки запросов с использованием middleware в ASP.NET Core.

## Технологии
- .NET 8
- ASP.NET Core
- In-memory хранилище (ConcurrentDictionary)

## Требования
- .NET 8 SDK
- Любой REST-клиент (curl, Postman, Swagger)

## Установка и запуск

### 1. Клонирование репозитория
git clone <url-репозитория>
cd projectTracker

### 2. Сборка и запуск проекта
dotnet build
dotnet run

## Доступные эндпоинты
Проекты
GET	/api/projects	Получить все проекты
POST	/api/projects	Создать новый проект
GET	/api/projects/{id}	Получить проект по ID
PUT	/api/projects/{id}	Обновить проект
GET	/api/projects/{id}/tasks	Получить все задачи проекта
Задачи
GET	/api/projects/{projectId}/tasks/{taskId}	Получить задачу по ID
PUT	/api/projects/{projectId}/tasks/{taskId}	Обновить задачу
POST	/api/projects/{projectId}/tasks	Создать новую задачу в проекте

Создание проекта
curl -X POST https://localhost:5001/api/projects \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Разработка мобильного приложения",
    "status": 1
  }'
  
Получение всех проектов
curl https://localhost:5001/api/projects

Создание задачи в проекте
curl -X POST https://localhost:5001/api/projects/1/tasks \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Разработать дизайн",
    "priority": 2
  }'
