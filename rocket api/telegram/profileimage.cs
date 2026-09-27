using TL;
using static rocket_api.telegram.Get_Chat_ID;

namespace rocket_api.telegram
{
    public class Profileimage
    {
        public struct result
        {
            public string? res { get; set; }
        }
        public static async Task<result> GetProfileImageBase64(Messages_Dialogs dialogs, WTelegram.Client client, long userId, long accessHash)
        {
            if (dialogs.users.TryGetValue(userId, out var user))
            {
                var peer = user.ToInputPeer();

                try
                {
                    var photos = await client.Photos_GetUserPhotos(user, 0, 0, 1);

                    // check if we got any photos
                    if (photos?.photos == null || photos.photos.Count() == 0)
                    {
                        Console.WriteLine($"No photos available for user {userId}");
                        return new result { res = null };
                    }

                    // first photo is usable
                    if (!(photos.photos[0] is TL.Photo photo))
                    {
                        Console.WriteLine($"First photo is not a valid TL.Photo for user {userId}");
                        return new result { res = null };
                    }

                    // pick size
                    var size = photo.sizes.OfType<TL.PhotoSize>()
                                          .OrderBy(s => s.w * s.h)
                                          .FirstOrDefault();

                    if (size == null)
                    {
                        Console.WriteLine($"No usable photo size for user {userId}");
                        return new result { res = null };
                    }

                    var inputLocation = new TL.InputPhotoFileLocation
                    {
                        id = photo.id,
                        access_hash = photo.access_hash,
                        file_reference = photo.file_reference,
                        thumb_size = size.type 
                    };

                    using var stream = new MemoryStream();
                    await client.DownloadFileAsync(inputLocation, stream);

                    var bytes = stream.ToArray();
                    string base64 = Convert.ToBase64String(bytes);
                    return new result { res = $"data:image/jpeg;base64,{base64}" };
                }
                catch (RpcException ex) when (ex.Code == 400 && ex.Message.Contains("USER_ID_INVALID"))
                {
                    Console.WriteLine($"Invalid or inaccessible user: {userId}");
                    return new result { res = null };
                }

                catch (RpcException ex)
                {
                    Console.WriteLine($"Telegram RPC error for user {userId}: {ex.Message} (Code {ex.Code})");
                    return new result { res = null };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error fetching profile image for user {userId}: {ex}");
                    return new result { res = null };
                }
            }
            return new result { res = null };
        }


    }

}
