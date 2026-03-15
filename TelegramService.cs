using System.Net.Http.Json;

namespace NotifierBot
{
    public class TelegramService
    {
        private readonly HttpClient _http = new();

        private readonly string _token;
        private readonly string _chatId;

        public TelegramService()
        {
            _token = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN") ?? "";
            _chatId = Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID") ?? "";

            if (string.IsNullOrWhiteSpace(_token))
                throw new Exception("TELEGRAM_TOKEN not found");

            if (string.IsNullOrWhiteSpace(_chatId))
                throw new Exception("TELEGRAM_CHAT_ID not found");
        }

        public async Task Send(string message)
        {
            var url = $"https://api.telegram.org/bot{_token}/sendMessage";

            await _http.PostAsJsonAsync(url, new
            {
                chat_id = _chatId,
                text = message
            });
        }
    }
}