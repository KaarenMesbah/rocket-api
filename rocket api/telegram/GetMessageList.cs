using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TL;
using WTelegram;
using static rocket_api.telegram.Get_Chat_ID;

namespace rocket_api.telegram
{
    public class GetMessageList
    {
        public struct MessageInfo
        {
            public int Id { get; set; }
            public long FromId { get; set; }
            public string Text { get; set; }
            public DateTime Date { get; set; }
            public string MediaType { get; set; } //(optional)
            public string FileName { get; set; }
        }

        public static async Task<List<MessageInfo>> GetUserChatHistory(Messages_Dialogs dialogs, WTelegram.Client client, long userId, long accessHash, int limit)
        {
            if (dialogs.users.TryGetValue(userId, out var user))
            {
                var peer = user.ToInputPeer();
                var history = await client.Messages_GetHistory(peer, limit: limit);
                var messages = new List<MessageInfo>();

                foreach (var msgBase in history.Messages)
                {
                    if (msgBase is TL.Message msg)
                    {
                        string v = GetMediaType(msg.media);
                        string b = "";
                        string f = "";
                        if (v == "Document")
                        {
                            if (msg.media is TL.MessageMediaDocument docMedia)
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
                        var messageInfo = new MessageInfo
                        {
                            Id = msg.id,
                            FromId = (msg.from_id as TL.PeerUser)?.user_id ?? 0,
                            Date = msg.date,
                            Text = msg.message,
                            MediaType = b,
                            FileName = f
                        };

                        // check if the message is a text message
                        messages.Add(messageInfo);
                        

                    }

                }

                return messages;
            }

            return new List<MessageInfo> { };
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