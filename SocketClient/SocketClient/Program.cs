using WebSocket4Net;

namespace FingerprintAbstaction
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new SocketClient();
            client.Setup("ws://127.0.0.1:8006", "basic", WebSocketVersion.Rfc6455);
            client.Start();
        }
    }
}