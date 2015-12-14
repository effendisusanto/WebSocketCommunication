using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperWebSocket;
using SuperSocket.SocketBase;

namespace BiometricService
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketService myServer = new SocketService();

            myServer.Setup();
            myServer.Start();
        
        }
    }
}
