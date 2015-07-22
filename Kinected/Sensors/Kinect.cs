using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using Microsoft.Kinect.Input;

namespace Kinected.Sensors
{
    class Kinect
    {
        private KinectSensor sensor;
        private MultiSourceFrameReader multiSrcReader = null;

        private int bodyCount = 0;

        private Body[] bodies = null;

        public Kinect()
        {
            this.sensor = KinectSensor.GetDefault();

            if (this.sensor != null)
            {
                this.sensor.Open();
            }
        }

        public void Start()
        {
            if(this.isConnected)
            {
                this.bodyCount = this.sensor.BodyFrameSource.BodyCount;
                this.bodies = new Body[this.bodyCount];

                this.multiSrcReader = this.sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body);
                this.multiSrcReader.MultiSourceFrameArrived += multiSrcReader_MultiSourceFrameArrived;
            }
        }

        public void Stop()
        {
            if(this.isConnected)
            {
                this.multiSrcReader.MultiSourceFrameArrived -= multiSrcReader_MultiSourceFrameArrived;
            }
        }

        public event EventHandler<BodyFrameData> BodyFrameArrived;

        void multiSrcReader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            if(this.BodyFrameArrived != null)
            {
                MultiSourceFrameReference msFrameRef = e.FrameReference;

                try
                {
                    MultiSourceFrame frame = msFrameRef.AcquireFrame();

                    if (frame != null)
                    {
                        BodyFrameReference bodyFrameReference = frame.BodyFrameReference;
                        BodyFrame bodyFrame = bodyFrameReference.AcquireFrame();
                        if (bodyFrame != null)
                        {
                            using (bodyFrame)
                            {
                                bodyFrame.GetAndRefreshBodyData(this.bodies);
                                foreach (Body b in this.bodies)
                                {
                                    if (b.IsTracked)
                                    {
                                        BodyFrameData bfd = new BodyFrameData();
                                        bfd.Body = b;
                                        this.BodyFrameArrived(this, bfd);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                }
            }
        }

        public bool isConnected
        {
            get
            {
                return this.sensor != null && this.sensor.IsOpen && this.sensor.IsAvailable;
            }
        }

        ~Kinect()
        {
            if (this.isConnected)
            {
                this.sensor.Close();
            }
        }
    }

    public class BodyFrameData : EventArgs
    {
        public Body Body { get; set; }
    }
}
