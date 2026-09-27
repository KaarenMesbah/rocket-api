using System.Reflection.Metadata;
using TL;

namespace rocket_api.telegram
{
    public class Get_Chat_ID
    {
        public static Get_Chat_ID init = new Get_Chat_ID();

        public struct User
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public long AccessHash { get; set; }
            public string username { get; set; }
            public string? thumbnail { get; set; }
            public bool bot { get; set; }
            public long? imageId { get; set; }
            public int note { get; set; }
            public long chat { get; set; }
        }

        public struct Grupe
        {
            public string channel { get; set; }
            public long AccessHash { get; set; }
        }

        public struct Chat
        {
            public string channel { get; set; }
            public long AccessHash { get; set; }
        }

        public struct Response
        {
            public List<User> User { get; set; }
            public List<Grupe> Grupe { get; set; }
            public List<Chat> Chat { get; set; }
        }

        public async Task<Response> Get_all(Messages_Dialogs dialogs, WTelegram.Client client)
        {
            var Chats = new List<Chat>();
            var Users = new List<User>();
            var Groups = new List<Grupe>();

            foreach (var d in dialogs.dialogs)
            {
                var peer = d.Peer;

                switch (peer)
                {
                    case TL.PeerUser u:
                        var user = dialogs.users[u.user_id];
                        int unread = 0;
                        long lol = 0;

                        // Each dialog in dialogs.dialogs corresponds to a peer
                        if (d is TL.Dialog dialog)
                        {
                            unread = dialog.unread_count; // <-- number of unread messages
                            lol = dialog.TopMessage;
                        }
                        string thumbnailBase64 = "";

                        // Use stripped_thumb if available
                        if (user.photo is TL.UserProfilePhoto upp && upp.stripped_thumb != null)
                        {
                            var gg = upp.stripped_thumb;
                            thumbnailBase64 = $"data:image/jpeg;base64,{Convert.ToBase64String(upp.stripped_thumb)}";
                        }

                        Users.Add(new User
                        {
                            Id = user.id,
                            username = user.username,
                            Name = $"{user.first_name} {user.last_name}".Trim(),
                            AccessHash = user.access_hash,
                            thumbnail = thumbnailBase64,
                            bot = user.IsBot,
                            imageId = user.photo?.photo_id,
                            note = unread,
                            chat = lol
                        });
                        break;

                    //case TL.PeerChat c:
                    //    var chat = dialogs.chats[c.chat_id];
                    //    Chats.Add(new Chat { channel = chat.Title });
                    //    break;

                    //case TL.PeerChannel ch:
                    //    var channel = dialogs.chats[ch.channel_id] as TL.Channel;
                    //    if (channel != null)
                    //        Groups.Add(new Grupe { channel = channel.Title });
                    //    else
                    //        Groups.Add(new Grupe { channel = "unknown" });
                    //    break;
                }
            }

            return new Response { Chat = Chats, User = Users, Grupe = Groups };
        }
    }
}