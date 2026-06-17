using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using System.Text.Json.Serialization;

namespace shop.api.Data.Controllers
{
  
        [ApiController]
        [Route("api/[controller]")]
        public class ChatController : ControllerBase
        {
        private readonly DB _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public ChatController(DB db, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new { answer = "Пустой запрос." });

            var query = request.Message.ToLower();

            // 1. Загружаем все доступные товары с Info
            var allProducts = await _db.Products
                .Include(p => p.Info)
                .Where(p => p.IsAvailable)
                .ToListAsync();

            Console.WriteLine($"=== Всего доступных товаров: {allProducts.Count}");
            foreach (var p in allProducts)
            {
                Console.WriteLine($"  - {p.Title} | Материал: {p.Info?.Material} | Категория: {p.Info?.Category}");
            }

            // 2. Разбиваем запрос на слова, отсекаем короткие и стоп-слова
            var stopWords = new HashSet<string> { "есть", "ищу", "хочу", "нужно", "найди", "покажи", "какие", "можно", "мне", "для", "при", "или", "что" };
            var words = query
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 2 && !stopWords.Contains(w))
                .ToList();

            Console.WriteLine($"=== Слова для поиска: {string.Join(", ", words)}");

            // 3. Ищем в памяти по каждому слову
            var products = allProducts.Where(p =>
                words.Any(w =>
                    p.Title.ToLower().Contains(w) ||
                    (p.Info != null && p.Info.FullDescription.ToLower().Contains(w)) ||
                    (p.Info != null && p.Info.ShortDescription.ToLower().Contains(w)) ||
                    (p.Info != null && p.Info.Material.ToLower().Contains(w)) ||
                    (p.Info != null && p.Info.Category.ToLower().Contains(w)) ||
                    (p.Info != null && p.Info.Color.ToLower().Contains(w))
                )
            ).Take(5).ToList();

            Console.WriteLine($"=== Найдено по запросу '{query}': {products.Count}");

            // 4. Формируем контекст
            string context;
            if (products.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine("Найденные товары в каталоге:");
                foreach (var p in products)
                {
                    sb.AppendLine($"- Название: {p.Title}");
                    sb.AppendLine($"  Цена: {p.Price}₽");
                    sb.AppendLine($"  Категория: {p.Info?.Category ?? "не указана"}");
                    sb.AppendLine($"  Материал: {p.Info?.Material ?? "не указан"}");
                    sb.AppendLine($"  Цвет: {p.Info?.Color ?? "не указан"}");
                    sb.AppendLine($"  Описание: {p.Info?.ShortDescription ?? ""}");
                }
                context = sb.ToString();
            }
            else
            {
                // Если ничего не нашли — передаём весь каталог чтобы модель могла ответить
                var sb = new StringBuilder();
                sb.AppendLine("По запросу ничего не найдено. Вот весь каталог:");
                foreach (var p in allProducts)
                {
                    sb.AppendLine($"- {p.Title}, цена: {p.Price}₽, категория: {p.Info?.Category ?? "не указана"}");
                }
                context = sb.ToString();
            }

            // 5. Системный промпт
            var systemPrompt = $"""
            Ты — краткий помощник магазина handmade товаров HobbyShop.
            Отвечай ТОЛЬКО по-русски, ТОЛЬКО на основе каталога ниже.
            Максимум 3-4 предложения. Без лишних слов.

            Правила:
            - Если товар найден: назови название, цену, кратко опиши.
            - Если не найден: скажи "Такого товара нет" и предложи другой запрос.
            - Не выдумывай товары которых нет в каталоге.
            - Не отвечай на вопросы не связанные с магазином.

            {context}

            Вопрос покупателя: {request.Message}
            Ответ помощника:
            """;

            // 6. Запрос в Ollama
            var ollamaUrl = _config["Ollama:BaseUrl"] ?? "http://localhost:11434";
            var model = _config["Ollama:Model"] ?? "gemma3:4b";

            var client = _httpClientFactory.CreateClient("ollama");
            var ollamaRequest = new
            {
                model,
                prompt = $"{systemPrompt}\n\nПользователь: {request.Message}\nПомощник:",
                stream = false
            };

            var response = await client.PostAsJsonAsync($"{ollamaUrl}/api/generate", ollamaRequest);

            if (!response.IsSuccessStatusCode)
                return StatusCode(500, new { answer = "Ошибка при обращении к модели." });

            var ollamaResponse = await response.Content.ReadFromJsonAsync<OllamaResponse>();

            return Ok(new { answer = ollamaResponse?.Response ?? "Нет ответа от модели." });
        }

    }



    public record ChatRequest(string Message);

        public class OllamaResponse
        {
            [JsonPropertyName("response")]
            public string Response { get; set; } = "";
        }
    }

