using System.Text;
using System.Text.Json;

namespace AIHubTaskTracker.Services
{
    public class TelegramService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public TelegramService(IConfiguration config)
        {
            _config = config;
            _http = new HttpClient();
        }

        public async Task SendMessageAsync(string message)
        {
            var token = _config["Telegram:BotToken"];
            var chatId = _config["Telegram:ChatId"];
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(chatId))
                return;

            var payload = new
            {
                chat_id = chatId,
                text = message,
                parse_mode = "Markdown"
            };

            var json = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            await _http.PostAsync($"https://api.telegram.org/bot{token}/sendMessage", json);
        }
    }
}
