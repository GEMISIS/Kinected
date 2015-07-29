using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Kinect;
using Kinected.Sensors;

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
        private object getPlayerLock = new object();

        private IPAddress address;
        private TcpListener server;
        private Kinect kinect;
        private Thread coreThread;

        public TcpServer(Kinect kinect)
        {
            this.kinect = kinect;
            coreThread = new Thread(new ThreadStart(this.StartServer));
            coreThread.Start();
        }
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

            this.server.Stop();
        }

        public void StopServer()
        {
            coreThread.Abort();
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            this.resetEvent.Set();
            //Console.WriteLine("Connection received!");

            TcpClient client = ((TcpListener)ar.AsyncState).EndAcceptTcpClient(ar);
            RecievedObject recievedData = new RecievedObject();
            recievedData.client = client;
            client.GetStream().BeginRead(recievedData.Buffer, 0, RecievedObject.BufferSize, new AsyncCallback(handleData), recievedData);
        }

        private void SendResponse(NetworkStream stream, String response)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(response);
            stream.Write(bytes, 0, bytes.Length);
        }

        private void GetPlayerID(NetworkStream stream)
        {
            if (Monitor.TryEnter(this.getPlayerLock))
            {
                ulong bodyID = this.kinect.GetHandRaisedPlayer();
                for (int i = 0; i < 10000 && bodyID == 0; i += 1)
                {
                    bodyID = this.kinect.GetHandRaisedPlayer();
                    Thread.Sleep(1);
                }
                this.SendResponse(stream, bodyID.ToString());
                Monitor.Exit(this.getPlayerLock);
            }
            else
            {
                this.SendResponse(stream, "-1");
            }
        }

        private string GetPlayerPosition(ulong bodyID)
        {
            Body body = this.kinect.GetBody(bodyID);
            if(body != null)
            {
                Joint head = body.Joints[JointType.Head];
                if(head != null && head.TrackingState != TrackingState.NotTracked)
                {
                    CameraSpacePoint headPosition = head.Position;
                    if (headPosition != null)
                    {
                        return headPosition.X + "," + headPosition.Y + "," + headPosition.Z;
                    }
                }
            }
            return "null";
        }

        public void handleData(IAsyncResult ar)
        {
            RecievedObject recvData = (RecievedObject)ar.AsyncState;

            int bytesRead = recvData.client.GetStream().EndRead(ar);

            if(bytesRead > 0)
            {
                string[] commands = Encoding.ASCII.GetString(recvData.Buffer, 0, bytesRead).Trim().Split(',');
                //Console.WriteLine(commands[0]);
                switch (commands[0])
                {
                    case "getPlayerID":
                        this.GetPlayerID(recvData.client.GetStream());
                        break;
                    case "getPlayerPosition":
                        string positionString = "null";
                        if(commands.Length > 1)
                        {
                            positionString = this.GetPlayerPosition(ulong.Parse(commands[1]));
                        }
                        this.SendResponse(recvData.client.GetStream(), positionString);
                        break;
                }
            }
            RecievedObject recievedData = new RecievedObject();
            recievedData.client = recvData.client;
            recvData.client.GetStream().BeginRead(recievedData.Buffer, 0, RecievedObject.BufferSize, new AsyncCallback(handleData), recievedData);
        }
    }
}
