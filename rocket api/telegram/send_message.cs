using TL;

namespace rocket_api.telegram
{
    public struct Ressult
    {
        public string ressult { get; set; }
    }
    public class send_message
    {
        public static send_message init = new send_message();

        public async Task<Ressult> Send(WTelegram.Client client, long id , string message , string type, long? accessHash)
        {
            InputPeer peer = type switch
            {
                "user" => new InputPeerUser(id, accessHash ?? 0),
                "chat" => new InputPeerChat((int)id),
                "channel" => new InputPeerChannel(id, accessHash ?? 0),
                _ => throw new Exception("Unknown peer type")
            };

            await client.SendMessageAsync(peer, message);
            return new Ressult{ressult = "suck ass"};
        }
    }
}
