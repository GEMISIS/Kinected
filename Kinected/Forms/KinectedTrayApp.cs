using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using Microsoft.Kinect.Input;

using Kinected.Sensors;

namespace Kinected.Forms
{
    class KinectedTrayApp : Form
    {
        private Kinect kinect;
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private StatusForm statusForm;

        [STAThread]
        static void Main(string[] args)
        {
            Application.Run(new KinectedTrayApp());
        }

        public KinectedTrayApp()
        {
            this.trayMenu = new ContextMenu();
            this.trayMenu.MenuItems.Add("Show Status", OnStatus);
            this.trayMenu.MenuItems.Add("Exit", OnExit);

            this.trayIcon = new NotifyIcon();
            this.trayIcon.Text = "Kinected";
            this.trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
            this.trayIcon.ContextMenu = this.trayMenu;
            this.trayIcon.Visible = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Visible = false;
            this.ShowInTaskbar = false;
            base.OnLoad(e);

            this.kinect = new Kinect();
            this.kinect.ImageFrameArrived += kinect_ImageFrameArrived;
            this.kinect.Start();
        }

        void kinect_ImageFrameArrived(object sender, ImageFrameData e)
        {
            if(statusForm.Visible)
            {
                statusForm.ColorImage = e.Image;
            }
        }

        private void OnStatus(object sender, EventArgs e)
        {
            this.statusForm = new StatusForm();
            statusForm.Show();
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
