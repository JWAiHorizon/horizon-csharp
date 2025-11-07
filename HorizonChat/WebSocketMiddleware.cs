using System.Net.WebSockets;
using System.Text;

namespace HorizonChat;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;
    public WebSocketMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/ws")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var socket = await context.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine("Client connected");
                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                while (!result.CloseStatus.HasValue)
                {
                    // Echo back received text (placeholder for chat logic)
                    var text = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var outgoing = Encoding.UTF8.GetBytes(text);
                    await socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
                    result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                }
                await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                Console.WriteLine("Client disconnected");
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        else
        {
            await _next(context);
        }
    }
}
