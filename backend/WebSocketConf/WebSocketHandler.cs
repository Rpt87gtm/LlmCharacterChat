using System.Text;
using System.Net.WebSockets;

namespace llmChat.WebSocketConf
{

    public static class WebSocketHandler
    {
        private static readonly Dictionary<string, List<WebSocket>> ActiveConnections = new();

        public static async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
        {
            var chatId = context.Request.Path.Value?.Split("/").Last();

            if (chatId == null)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid Chat ID", CancellationToken.None);
                return;
            }

            lock (ActiveConnections)
            {
                if (!ActiveConnections.ContainsKey(chatId))
                {
                    ActiveConnections[chatId] = new List<WebSocket>();
                }

                ActiveConnections[chatId].Add(webSocket);
            }

            await ReceiveMessages(chatId, webSocket);
        }

        private static async Task ReceiveMessages(string chatId, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);

                await BroadcastMessage(chatId, messageJson);
            } while (!result.CloseStatus.HasValue);

            lock (ActiveConnections)
            {
                if (ActiveConnections.ContainsKey(chatId))
                {
                    ActiveConnections[chatId].Remove(webSocket);
                }
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private static async Task BroadcastMessage(string chatId, string messageJson)
        {
            if (!ActiveConnections.ContainsKey(chatId)) return;

            var messageBuffer = Encoding.UTF8.GetBytes(messageJson);
            var tasks = ActiveConnections[chatId].Select(socket =>
                socket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None));

            await Task.WhenAll(tasks);
        }
    }

}
