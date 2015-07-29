using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Net;

using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using Microsoft.Kinect.Input;

using Kinected.Objects;
using Kinected.Sensors;

namespace Kinected.Forms
{
    class KinectedTrayApp : Form
    {
        private Kinect kinect;
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private StatusForm statusForm;
        private TcpServer server;

        [STAThread]
        static void Main(string[] args)
        {
            Application.Run(new KinectedTrayApp());
        }

        public KinectedTrayApp()
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Visible = false;
            this.ShowInTaskbar = false;
            base.OnLoad(e);

            this.trayMenu = new ContextMenu();
            this.trayMenu.MenuItems.Add("Connect to Kinect", tryConnect);
            this.trayMenu.MenuItems.Add("Show Status", OnStatus);
            this.trayMenu.MenuItems.Add("Exit", OnExit);

            this.trayIcon = new NotifyIcon();
            this.trayIcon.Text = "Kinected";
            this.trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
            this.trayIcon.ContextMenu = this.trayMenu;
            this.trayIcon.Visible = true;

            this.tryConnect(null, null);
        }

        void kinect_ImageFrameArrived(object sender, ImageFrameData e)
        {
            if (statusForm != null && statusForm.Visible)
            {
                statusForm.BodyCount = this.kinect.BodyCount;
                statusForm.ColorImage = e.Image;
            }
        }

        void kinect_BodyFrameArrived(object sender, BodyFrameData e)
        {
        }

        private void tryConnect(object sender, EventArgs e)
        {
            this.kinect = new Kinect();
            if (this.kinect.isConnected)
            {
                this.kinect.ImageFrameArrived += kinect_ImageFrameArrived;
                this.kinect.BodyFrameArrived += kinect_BodyFrameArrived;
                this.kinect.Start();
                this.server = new TcpServer(this.kinect);
                this.trayMenu.MenuItems[0].Enabled = false;

                IPHostEntry host;
                string localIP = "?";
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach(IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                    }
                }

                this.trayIcon.ShowBalloonTip(1000 * 5, "Kinected Active", "Your IP Address is: " + localIP, ToolTipIcon.Info);
            }
        }

        private void OnStatus(object sender, EventArgs e)
        {
            this.statusForm = new StatusForm();
            statusForm.Show();
        }

        private void OnExit(object sender, EventArgs e)
        {
            this.server.StopServer();
            Application.Exit();
        }
    }
}
