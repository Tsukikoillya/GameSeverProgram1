using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameProgram1.Controller;
using GameProgram1.DAO;
using SocketGameProtocol1;

namespace GameProgram1
{
    class Server
    {
        private Socket socket;
        private List<Client> clientList = new List<Client>();
        private ControllerManager controllerManager;
        public Server(string IpStr,int port)//通过Ip和端口号来初始化
        {
            controllerManager = new ControllerManager(this);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(IpStr),port);
            socket.Bind(point);
            socket.Listen(5);
            StartAccept();
        }
        void StartAccept()
        {
            socket.BeginAccept(AcceptCallback, null);
        }
        void AcceptCallback(IAsyncResult iar)
        {
            Socket clientSocket = socket.EndAccept(iar);
            clientList.Add(new Client(clientSocket, this));//clientList.Add(new Client(socket, this));
            StartAccept();

        }
        public void HandleRequest(MainPack pack,Client client)
        {
            controllerManager.HandleRequest(pack,client);
        }
    }
}
