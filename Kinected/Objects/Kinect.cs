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
        private BodyFrameData bfd = new BodyFrameData();

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
                                        this.bfd.Body = b;
                                        this.BodyFrameArrived(this, this.bfd);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // TODO: Handle exceptions
                }
            }
        }

        public bool isConnected
        {
            get
            {
                try
                {
                    return this.sensor != null && this.sensor.IsOpen && this.sensor.IsAvailable;
                }
                catch (Exception)
                {
                    // TODO: Properly handle exceptions
                    return false;
                }
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
