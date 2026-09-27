namespace rocket_api.telegram
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using TL;
    using WTelegram;

    public class TelegramService : BackgroundService
    {
        private readonly ILogger<TelegramService> _logger;
        private Client _client;

        public TelegramService(ILogger<TelegramService> logger)
        {
            _logger = logger;
        }

        public struct MessageInfo
        {
            public int id { get; set; }
            public long fromId { get; set; }
            public string text { get; set; }
            public DateTime date { get; set; }
            public string mediaType { get; set; }//(optional)
            public string fileName { get; set; }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogWarning("TelegramService ExecuteAsync STARTED");
            _client = await Telegram_login.Main();
            _client.OnUpdates += OnUpdates;
            var state = await _client.Updates_GetState();

            int pts = state.pts;
            int qts = state.qts;
            var date = state.date;
            var diff = await _client.Updates_GetDifference(pts,date,qts);


            //await _client.ConnectAsync();
            //await _client.LoginUserIfNeeded();

            //_logger.LogInformation("Telegram client connected");

            // keep service alive
            while (!stoppingToken.IsCancellationRequested)
                await Task.Delay(1000, stoppingToken);
        }
        private Task OnUpdates(UpdatesBase updates)
        {
            foreach (var update in updates.UpdateList)
            {
                switch (update)
                {
                    case UpdateNewMessage unm:
                        HandleMessage(unm.message as Message);
                        break;

                    case UpdateReadHistoryInbox urhi:
                        // urhi.Peer = the chat
                        // urhi.MaxId = last read message id
                        _logger.LogInformation("Unread count updated for peer {Peer}", urhi.peer);
                        // TODO: update your local dialog list / UI 
                        
                        break;

                    case UpdateReadMessagesContents urmc:
                        _logger.LogInformation("Messages marked read: {Ids}", string.Join(",", urmc.messages));
                        break;

                    case UpdateUserStatus uus:
                        _logger.LogInformation("User {UserId} status changed: {Status}", uus.user_id, uus.status);
                        break;

                    default:
                        _logger.LogDebug("Other update type: {Type}", update.GetType().Name);
                        break;
                }
            }
            return Task.CompletedTask;
        }

        private Task HandleMessage(Message message)
        {
                    _logger.LogInformation("New message: {Text}", message.message);
            string v = GetMediaType(message.media);
            string b = "";
            string f = "";
            if (v == "Document")
            {
                if (message.media is TL.MessageMediaDocument docMedia)
                {
                    var doc = docMedia.document as TL.Document;

                    foreach (var attr in doc.attributes)
                    {
                        switch (attr)
                        {
                            case TL.DocumentAttributeFilename fn:
                                f = fn.file_name;
                                break;

                            case TL.DocumentAttributeVideo:
                                b = "Video";
                                break;

                            case TL.DocumentAttributeAudio audio:
                                b = "Audio";
                                break;

                            case TL.DocumentAttributeSticker:
                                b = "Sticker";
                                break;
                            case TL.DocumentAttributeAnimated:
                                b = "anime";
                                break;

                        }
                    }
                }
            }
            else
            {
                b = v;
            }
            var n = new MessageInfo {
                id = message.id, 
                fromId = message.from_id.ID,
                date = message.date,
                text = message.message,
                mediaType = b,
                fileName = f
            };
            _ = WebSocketHandler.Broadcast(
                JsonSerializer.Serialize(n)
            );

            return Task.CompletedTask;
        }
        static string GetMediaType(MessageMedia media)
        {
            return media switch
            {
                null => "Text",

                TL.MessageMediaPhoto => "Photo",
                TL.MessageMediaDocument => "Document",
                TL.MessageMediaContact => "Contact",
                TL.MessageMediaGeo => "Location",
                TL.MessageMediaVenue => "Venue",
                TL.MessageMediaGame => "Game",
                TL.MessageMediaInvoice => "Invoice",
                TL.MessageMediaPoll => "Poll",
                TL.MessageMediaWebPage => "WebPage",
                TL.MessageMediaUnsupported => "Unsupported",

                _ => "Unknown"
            };
        }

    }
}
