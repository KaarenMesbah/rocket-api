using WTelegram;

namespace rocket_api.telegram
{

    using WTelegram;

    public static class Telegram_login
    {
        private static Client? _client;
        private static readonly object _lock = new();

        public static async Task<Client> Main()
        {
            if (_client != null)
                return _client;

            lock (_lock)
            {
                if (_client != null)
                    return _client;

                _client = new Client(Config);
            }

            await _client.ConnectAsync();
            await _client.LoginUserIfNeeded();

            return _client;
        }



        static string? Config(string what)
        {
            if (what == "api_id") return "38094795";
            if (what == "api_hash") return "10bd2261dcb8fa595201f8d38e520470";
            if (what == "phone_number") return "+989352252461";
            if (what == "verification_code") return null;  // handle via web endpoint or saved session
            if (what == "first_name") return null;
            if (what == "last_name") return null;
            if (what == "password") return null;
            return null;
        }
    }
}