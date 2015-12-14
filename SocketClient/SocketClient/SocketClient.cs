using Newtonsoft.Json;
using System;
using WebSocket4Net;

namespace FingerprintAbstaction
{
    class SocketClient
    {
        private WebSocket websocketClient;

        private string url;
        private string protocol;
        private WebSocketVersion version;

        public void Setup(string url, string protocol, WebSocketVersion version)
        {
            this.url = url;
            this.protocol = protocol;
            this.version = WebSocketVersion.Rfc6455;

            websocketClient = new WebSocket(this.url, this.protocol, this.version);

            websocketClient.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocketClient_Error);
            websocketClient.Opened += new EventHandler(websocketClient_Opened);
            websocketClient.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocketClient_MessageReceived);
        }

        public void Start()
        {
            websocketClient.Open();

            char keyStroked;

            while (true)
            {
                keyStroked = Console.ReadKey().KeyChar;

                if (keyStroked.Equals('q'))
                {
                    Stop();
                    return;
                }
                else
                {
                    if (keyStroked.Equals('s'))
                    {
                        Console.WriteLine();
                        Console.WriteLine("Put here your message to server: ");

                        string message = Console.ReadLine();

                        websocketClient.Send(MessageBuilder("simple",message));
                    }
                    else if (keyStroked.Equals('n')) {
                        Console.WriteLine("turn on engine on server");
                        websocketClient.Send(MessageBuilder("start",""));
                    }
                    else if (keyStroked.Equals('f'))
                    {
                        Console.WriteLine("turn of engine on server");
                        websocketClient.Send(MessageBuilder("stop", ""));
                    }

                    ShowAvailableOptions();
                    continue;
                }
            }
        }

        private void Stop()
        {
            websocketClient.Close();
            Console.WriteLine("Client disconnected!");
        }

        private void ShowAvailableOptions()
        {
            Console.WriteLine();
            Console.WriteLine("Available options: ");
            Console.WriteLine("Press 's' key to send message to server");
            Console.WriteLine("Press 'q' key to close connection");
            Console.WriteLine("Press 'n' key to turn on engine");
            Console.WriteLine("Press 'f' key to turn off engine");
        }

        private void websocketClient_Opened(object sender, EventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine("Client successfully connected.");
            Console.WriteLine();

            websocketClient.Send(MessageBuilder("simple","Hello World!"));

            ShowAvailableOptions();
        }

        private void websocketClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine("Message Received. Server answered: " + e.Message);
        }

        private void websocketClient_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Console.WriteLine(e.Exception.GetType() + ": " + e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);

            if (e.Exception.InnerException != null)
            {
                Console.WriteLine(e.Exception.InnerException.GetType());
            }
            return;
        }

        private string MessageBuilder(string messageType, string messageContent) {
            MessageObject messageObject = new MessageObject();
            messageObject.MessageContent = messageContent;
            messageObject.MessageType = messageType;
            return JsonConvert.SerializeObject(messageObject);
        }
    }

    public class MessageObject
    {
        public string MessageType { get; set; }
        public string MessageContent { get; set; }
    }
}
