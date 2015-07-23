using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
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

        private byte[] colorImageData;

        public event EventHandler<BodyFrameData> BodyFrameArrived;
        public event EventHandler<ImageFrameData> ImageFrameArrived;

        private ImageFrameData ifd = new ImageFrameData();
        private BodyFrameData bfd = new BodyFrameData();

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
            if (this.sensor != null)
            {
                FrameDescription colorFrameDescription = this.sensor.ColorFrameSource.FrameDescription;
                this.colorImageData = new byte[colorFrameDescription.Width * colorFrameDescription.Height * 4];

                this.bodyCount = this.sensor.BodyFrameSource.BodyCount;
                this.bodies = new Body[this.bodyCount];

                this.multiSrcReader = this.sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Body);
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

        private void HandleColorFrame(ColorFrameReference colorFrameReference)
        {
            ColorFrame colorFrame = colorFrameReference.AcquireFrame();

            if(colorFrame != null)
            {
                using(colorFrame)
                {
                    if(colorFrame.RawColorImageFormat == ColorImageFormat.Bgra)
                    {
                        colorFrame.CopyRawFrameDataToArray(this.colorImageData);
                    }
                    else
                    {
                        colorFrame.CopyConvertedFrameDataToArray(this.colorImageData, ColorImageFormat.Bgra);
                    }

                    this.ifd.createBitmap(colorFrame.FrameDescription.Width, colorFrame.FrameDescription.Height, PixelFormat.Format32bppArgb, this.colorImageData);
                    this.ImageFrameArrived(this, this.ifd);
                }
            }
        }

        private void HandleBodies(BodyFrameReference bodyFrameReference)
        {
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

        void multiSrcReader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrameReference msFrameRef = e.FrameReference;

            try
            {
                MultiSourceFrame frame = msFrameRef.AcquireFrame();

                if (frame != null)
                {
                    ColorFrameReference colorFrameReference = frame.ColorFrameReference;
                    BodyFrameReference bodyFrameReference = frame.BodyFrameReference;

                    if (this.ImageFrameArrived != null)
                    {
                        this.HandleColorFrame(colorFrameReference);
                    }
                    if (this.BodyFrameArrived != null)
                    {
                        this.HandleBodies(bodyFrameReference);
                    }
                }
            }
            catch (Exception)
            {
                // TODO: Handle exceptions
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
    public class ImageFrameData : EventArgs
    {
        private Bitmap image = null;

        public void createBitmap(int width, int height, PixelFormat pixelFormat, byte[] data)
        {
            this.image = new Bitmap(width, height, pixelFormat);
            BitmapData bmpData = this.image.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, this.image.PixelFormat);
            IntPtr ptr = bmpData.Scan0;

            int bytes = bmpData.Stride * this.image.Height;

            Marshal.Copy(data, 0, ptr, bytes);
            this.image.UnlockBits(bmpData);
        }

        public Bitmap Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }
    }
}
