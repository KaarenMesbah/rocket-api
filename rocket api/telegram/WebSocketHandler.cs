namespace rocket_api.telegram
{
    using System.Net.WebSockets;
    using System.Text;

    public static class WebSocketHandler
    {
        private static readonly List<WebSocket> _clients = new();

        public static async Task AddClient(WebSocket socket)
        {
            _clients.Add(socket);

            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                    break;
            }

            _clients.Remove(socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
        }

        public static async Task Broadcast(string message)
        {
            var data = Encoding.UTF8.GetBytes(message);

            foreach (var socket in _clients.ToArray())
            {
                if (socket.State != WebSocketState.Open)
                    continue;

                await socket.SendAsync(
                    data,
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );
            }
        }
    }

}
