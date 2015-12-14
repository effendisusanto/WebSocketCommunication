using System;
using System.Linq;
using SuperWebSocket;
using SuperSocket.SocketBase;
using Newtonsoft.Json;
using System.Threading;

namespace BiometricService
{
    class SocketService
    {

        private WebSocketServer appServer;

        public void Setup()
        {
            Console.WriteLine("Press any key to start the WebSocketServer!");
            Console.ReadKey();
            Console.WriteLine();

            appServer = new WebSocketServer();

            if (!appServer.Setup(8006)) //Setup with listening port
            {
                Console.WriteLine("Failed to setup!");
                Console.ReadKey();
                return;
            }

            appServer.NewSessionConnected += new SessionHandler<WebSocketSession>(appServer_NewSessionConnected);
            appServer.SessionClosed += new SessionHandler<WebSocketSession, CloseReason>(appServer_SessionClosed);
            appServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(appServer_NewMessageReceived);

            Console.WriteLine();
        }

        public void Start()
        {
            if (!appServer.Start())
            {
                Console.WriteLine("Failed to start!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("The server started successfully! Press any key to see application options.");
            Console.ReadKey();

            ShowAvailableOptions();

            char keyStroked;

            while (true)
            {
                keyStroked = Console.ReadKey().KeyChar;

                if (keyStroked.Equals('q'))
                {
                    Stop();
                    return;
                }

                if (keyStroked.Equals('s'))
                {
                    Console.WriteLine();
                    Console.WriteLine("Put here your message to clients: ");

                    string message = Console.ReadLine();

                    foreach (WebSocketSession session in appServer.GetAllSessions())
                    {
                        session.Send("Message to client: " + message);
                    }
                }
                if (keyStroked.Equals('g')) {
                    Console.WriteLine("We send 100 line of message to the client");
                    GenerateBroadcastMessage();
                }

                ShowAvailableOptions();
                continue;
            }
        }

        public void Stop()
        {
            appServer.Stop();

            Console.WriteLine();
            Console.WriteLine("The server was stopped!");
        }

        public void ShowAvailableOptions()
        {
            Console.WriteLine();
            Console.WriteLine("Available options: ");
            Console.WriteLine("Press 'q' key to stop the server.");
            Console.WriteLine("Press 's' key to send message to client.");
            Console.WriteLine("Press 'g' key to generate message to client.");
        }

        private void appServer_NewMessageReceived(WebSocketSession session, string message)
        {
            MessageHandler(session, message);
        }

        private void appServer_NewSessionConnected(WebSocketSession session)
        {
            Console.WriteLine();
            Console.WriteLine("New session connected! Sessions counter: " + appServer.SessionCount);
            session.Send("Hello new client!");
        }

        private void appServer_SessionClosed(WebSocketSession session, CloseReason value)
        {
            Console.WriteLine();
            Console.WriteLine("Client disconnected! Sessions counter: " + appServer.SessionCount);
        }

        private void BroadcastMessage(string messageType, string messageContent) {
            MessageObject msgObject = new MessageObject() {MessageType=messageType, MessageContent=messageContent };
            string jsonMessage = JsonConvert.SerializeObject(msgObject);
            foreach (WebSocketSession session in appServer.GetAllSessions())
            {
                session.Send(jsonMessage);
            }
        }

        private void GenerateBroadcastMessage() {
            ThreadStart job = new ThreadStart(ThreadJob);
            Thread thread = new Thread(job);
            thread.Start();
        }

        private void MessageHandler(WebSocketSession session, string message) {
            MessageObject messageObject = JsonConvert.DeserializeObject<MessageObject>(message);
            if (messageObject.MessageType == "simple") {
                Console.WriteLine("Client said: " + message);
                //Send the received message back
                session.Send("Server responded back: " + message);
            }
            else if (messageObject.MessageType == "start")
            {
                Console.WriteLine("Client request to start the engine");
                if (isPreviewing)
                {
                    session.Send("The engine is currently running, please stop the engine first");
                }
                else {
                    GenerateBroadcastMessage();
                }
            }
            else if(messageObject.MessageType=="stop") {
                Console.WriteLine("Client stop the engine");
                if (!isPreviewing) {
                    session.Send("The engine is not running");
                }
                isPreviewing = false;
            }
        }

        private void ThreadJob() {
            isPreviewing = true;
            int i = 0;
            while(isPreviewing && appServer.GetAllSessions().Count()>0){
                i++;
                foreach (WebSocketSession session in appServer.GetAllSessions())
                {
                    session.Send("This is generate at: " + i);
                    Console.WriteLine("This is generate at: " + i);
                }
                Thread.Sleep(500);
            }
        }
        bool isPreviewing = false;
    }

    public class MessageObject {
        public string MessageType { get; set; }
        public string MessageContent { get; set; }
    }
}
