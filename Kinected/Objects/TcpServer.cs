using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;

namespace Kinected.Objects
{
    public class RecievedObject
    {
        public TcpClient client;
        public const int BufferSize = 2048;
        public byte[] Buffer = new byte[BufferSize];
    }
    class TcpServer
    {
        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        private IPAddress address;
        private TcpListener server;

        private void StartServer()
        {
            this.address = IPAddress.Parse("127.0.0.1");
            this.server = new TcpListener(address, 1337);
            this.server.Start();

            while(true)
            {
                this.resetEvent.Reset();
                
                this.server.BeginAcceptTcpClient(AcceptCallback, this.server);

                this.resetEvent.WaitOne();
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            this.resetEvent.Set();
            Console.WriteLine("Connection received!");

            TcpClient client = ((TcpListener)ar.AsyncState).EndAcceptTcpClient(ar);
            RecievedObject recievedData = new RecievedObject();
            recievedData.client = client;
            client.GetStream().BeginRead(recievedData.Buffer, 0, RecievedObject.BufferSize, new AsyncCallback(handleData), recievedData);
        }

        public void handleData(IAsyncResult ar)
        {
            RecievedObject recvData = (RecievedObject)ar.AsyncState;

            int bytesRead = recvData.client.GetStream().EndRead(ar);

            if(bytesRead > 0)
            {
                // TODO: Add command handling here
                Console.WriteLine(Encoding.ASCII.GetString(recvData.Buffer, 0, bytesRead));
            }
            RecievedObject recievedData = new RecievedObject();
            recievedData.client = recvData.client;
            recvData.client.GetStream().BeginRead(recievedData.Buffer, 0, RecievedObject.BufferSize, new AsyncCallback(handleData), recievedData);
        }

        public TcpServer()
        {
            Thread coreThread = new Thread(new ThreadStart(this.StartServer));
            coreThread.Start();
        }
    }
}
