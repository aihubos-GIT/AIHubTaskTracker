using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AIHubTaskTracker.Services
{
    public class TelegramService
    {
        private readonly HttpClient _client;
        private readonly string _botToken;
        private readonly string _chatId;

        public TelegramService(HttpClient client, string botToken, string chatId)
        {
            _client = client;
            _botToken = botToken;
            _chatId = chatId;
        }

        public async Task SendMessageAsync(string message)
        {
            var payload = new { chat_id = _chatId, text = message };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            await _client.PostAsync($"https://api.telegram.org/bot{_botToken}/sendMessage", content);
        }
    }
}
