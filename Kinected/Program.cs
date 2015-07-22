using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using Microsoft.Kinect.Input;
using Kinected.Sensors;

namespace Kinected
{
    class KinectedService : ServiceBase
    {
        private Kinect kinect;

        public KinectedService()
        {
            this.ServiceName = "Kinected";
            this.EventLog.Log = "Application";

            this.CanHandlePowerEvent = true;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            this.kinect = new Kinect();
            this.kinect.Start();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnContinue()
        {
            base.OnContinue();
        }

        protected override void OnStop()
        {
            base.OnStop();

            this.kinect.Stop();
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnCustomCommand(int command)
        {
            base.OnCustomCommand(command);
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return base.OnPowerEvent(powerStatus);
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new KinectedService());
        }
    }
}
