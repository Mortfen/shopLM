# shopLM

Учебный прототип маркетплейса для нишевых товаров и изделий для хобби. Проект объединяет веб-интерфейс на ASP.NET Core Razor Pages, отдельный ASP.NET Core Web API, PostgreSQL и локальную языковую модель, запускаемую через Ollama.

Проект создавался в учебных целях и в рамках дипломной работы: для практики backend-разработки, проектирования реляционной модели данных, аутентификации и интеграции локальной LLM. Это демонстрационный проект, а не готовая production-платформа для электронной коммерции.

## Возможности

- Просмотр каталога товаров и подробной информации о товарах, включая категории и изображения.
- Разделение ролей покупателей и продавцов, а также роль администратора.
- Создание заказов и просмотр информации о них.
- Хранение описания и характеристик товаров: категории, материала, цвета, размеров и других параметров.
- Эксперименты с поиском товаров с использованием локальной LLM через Ollama.
- Аутентификация запросов к API с помощью JWT, подписанных RSA-ключом.
- Обновление схемы PostgreSQL через миграции EF Core и заполнение начальными данными при запуске API.

> **О поиске с помощью LLM.** Текущая реализация не использует embeddings или векторную базу данных. Ollama выполняет локальный запуск языковой модели. Поэтому проект не следует описывать как поиск на основе embeddings или полноценный RAG-пайплайн.

## Технологии

| Область | Технология |
| --- | --- |
| Backend API | ASP.NET Core Web API (.NET 10) |
| Веб-интерфейс | ASP.NET Core Razor Pages (.NET 10) |
| ORM | Entity Framework Core |
| База данных | PostgreSQL 17, Npgsql |
| Пользователи и роли | ASP.NET Core Identity |
| Аутентификация API | JWT Bearer, подпись и проверка RSA-ключами |
| Локальная LLM | Ollama |
| Модель по умолчанию | `gemma3:4b` |
| Документация API | Swagger / OpenAPI |
| Локальный запуск | Docker Compose |

## Архитектура

```text
Браузер
   |
   v
shop.wb (ASP.NET Core Razor Pages)
   |
   | HTTP-запросы + Bearer JWT
   v
shop.api (ASP.NET Core Web API)
   |                         \
   | EF Core + Npgsql         \ HTTP
   v                           v
PostgreSQL 17                Ollama
                               |
                               v
                            gemma3:4b
```

Браузер взаимодействует с приложением Razor Pages. Веб-приложение обращается к API через HTTP-клиент, хранит JWT в серверной Session и использует Cookie Authentication для веб-сеанса. API проверяет Bearer-токены и применяет ролевую авторизацию для защищённых endpoint'ов.

## Модель данных

Основные сущности и связи между ними:

```text
User
 ├── Products
 └── Orders

Product
 ├── ProductInfo       (один к одному)
 ├── ProductImages     (один ко многим)
 └── OrderItems        (один ко многим)

Order
 └── OrderItems        (один ко многим)
                         └── Product
```

`OrderItems` связывает заказы и товары и хранит количество и цену единицы товара на момент покупки. Благодаря этому последующее изменение цены в каталоге не изменяет историческую стоимость уже оформленного заказа.

## Структура репозитория

```text
shopLM/
├── shop.api/                 # ASP.NET Core Web API, модели, EF Core и аутентификация
├── shop.wb/                  # Razor Pages и HTTP-клиент для API
├── shop.lm/                  # Отдельный/экспериментальный каталог проекта
├── shop.slnx                 # Текущее решение: shop.api и shop.wb
├── docker-composeDB.yml      # Сервис PostgreSQL
├── docker-composeLm.yml      # Сервис Ollama
├── docker-composeApp.yml     # API и веб-приложение
└── docker.txt                # Ранние заметки по Docker
```

## Запуск через Docker Compose

В ветке `docker` используются три Compose-файла вместо одного общего. Команды ниже выполняются из корневого каталога репозитория. Это локальная конфигурация для демонстрации: перед запуском требуется подготовить RSA-ключи и заменить путь к каталогу загружаемых файлов, привязанный к компьютеру разработчика.

### Требования

- Docker Desktop с Docker Compose.
- OpenSSL или другой инструмент для генерации пары RSA-ключей.
- Достаточно свободного места на диске и оперативной памяти для загрузки и запуска `gemma3:4b`.

### 1. Клонировать репозиторий

```bash
git clone https://github.com/Mortfen/shopLM.git
cd shopLM
git switch docker
```

### 2. Создать RSA-ключи для API

Dockerfile API копирует каталог `shop.api/key` в образ, однако этот каталог и файлы ключей игнорируются Git. Поэтому перед сборкой образа нужно создать локальную пару ключей.

```bash
mkdir -p shop.api/key
openssl genrsa -out shop.api/key/key.pem 2048
openssl rsa -in shop.api/key/key.pem -pubout -out shop.api/key/key.pub
```

В Windows команды можно выполнить через Git Bash или WSL, если в окружении установлен OpenSSL. Не добавляйте ключи в Git: закрытый ключ `key.pem` не должен публиковаться.

### 3. Заменить путь к загруженным изображениям

Перед сборкой приложения откройте `docker-composeApp.yml`. В текущей конфигурации том API указывает на путь на компьютере разработчика:

```yaml
- O:/testDocker/shop.api/wwwroot:/app/wwwroot
```

Замените его на локальный каталог, например:

```yaml
- ./api-uploads:/app/wwwroot/uploads
```

Создайте каталог до запуска:

```bash
mkdir -p api-uploads
```

В PowerShell можно выполнить `New-Item -ItemType Directory -Force api-uploads`. Том используется для хранения загружаемых изображений товаров.

### 4. Запустить PostgreSQL

```bash
docker compose -f docker-composeDB.yml up -d
docker compose -f docker-composeDB.yml exec postgres pg_isready -U postgres -d hobbyshop
```

Продолжайте после того, как `pg_isready` сообщит, что PostgreSQL принимает подключения.

### 5. Запустить Ollama и загрузить модель

```bash
docker compose -f docker-composeLm.yml up -d
docker compose -f docker-composeLm.yml exec ollama ollama pull gemma3:4b
```

При первом запуске модель скачивается из сети, поэтому загрузка может занять время. Файлы модели сохраняются в именованном Docker volume.

### 6. Собрать и запустить приложения

```bash
docker compose -f docker-composeApp.yml up --build -d
```

При запуске API применяет миграции EF Core и выполняет заполнение начальными данными. Если приложение не стартовало, проверьте логи:

```bash
docker compose -f docker-composeApp.yml logs -f hobbyshop-api hobbyshop-web
```

### Локальные адреса

| Сервис | Адрес |
| --- | --- |
| Веб-приложение Razor Pages | <http://localhost:5000> |
| API | <http://localhost:7168> |
| Swagger UI (окружение Development) | <http://localhost:7168/swagger> |
| API Ollama | <http://localhost:11434> |
| PostgreSQL | `localhost:5432` |

Compose-файлы используют общую Docker-сеть `hobbyshop-net`. Поэтому API обращается к PostgreSQL по адресу `postgres:5432`, а к Ollama — `ollama:11434`. Веб-приложение использует `hobbyshop-api:8080` для внутренних запросов к API и `localhost:7168` для обращений с хост-машины.

В текущих Compose-файлах заданы демонстрационные учётные данные БД и окружение `Development`. Эти настройки рассчитаны на локальную оценку проекта. При изменении имени пользователя или пароля необходимо согласованно обновить конфигурацию PostgreSQL и строку подключения API.

### Остановка контейнеров

```bash
docker compose -f docker-composeApp.yml down
docker compose -f docker-composeLm.yml down
docker compose -f docker-composeDB.yml down
```

Эти команды не удаляют именованные volumes, поэтому данные PostgreSQL и загруженная модель должны сохраниться.

## База данных и аутентификация

- Миграции EF Core обновляют схему базы данных при запуске API.
- `DbSeeder` создаёт начальные роли и демонстрационные данные.
- ASP.NET Core Identity управляет пользователями и ролями `Buyer`, `Seller` и `Admin`.
- API проверяет подпись JWT с помощью открытого RSA-ключа; закрытый ключ используется кодом, который выпускает токены.
- Веб-приложение хранит токен API в серверной Session и передаёт его в заголовке `Authorization: Bearer` при вызовах защищённых endpoint'ов.

## Ограничения текущей версии

- Это учебный демонстрационный проект, а не платёжная система или готовый шаблон production-развёртывания.
- Поиск с использованием LLM работает через Ollama; embeddings и векторная база данных не используются.
- Автоматизированный тестовый проект в репозитории пока отсутствует. Для ручного изучения API доступен Swagger.
- Для запуска требуются подготовка общей Docker-сети, локальных RSA-ключей и каталога для загружаемых файлов, как описано выше.

## Лицензия

Отдельная лицензия проекта в репозитории не указана. Перед повторным использованием или распространением кода проверьте статус лицензирования.

