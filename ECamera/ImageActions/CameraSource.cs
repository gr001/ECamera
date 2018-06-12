using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Windows;
using System.Collections.ObjectModel;
using ECamera.Common;
using ECamera.ImageActions;

namespace ECamera
{
    public class CameraSourceProperties : ImagePropertiesBase
    {
        Action m_startCamera;
        Action m_closeCamera;

        public CameraSourceProperties(Action startCameraAction, Action closeCameraAction) : base("Camera")
        {
            m_startCamera = startCameraAction;
            m_closeCamera = closeCameraAction;
        }
        
        public int CameraSourceId
        {
            get { return m_cameraSourceId; }
            set
            {
                if (value != m_cameraSourceId)
                {
                    m_closeCamera();

                    m_cameraSourceId = value;

                    m_startCamera();

                    NotifyPropertyChanged();
                }
            }
        }
        int m_cameraSourceId = -1;

        public ObservableCollection<int> CameraSourceIds
        {
            get { return m_cameraSourceIds; }
        }
        ObservableCollection<int> m_cameraSourceIds = new ObservableCollection<int>();
    }

    public class CameraSource : ImageAction
    {
        OpenCvSharp.VideoCapture m_videoCapture;// = new OpenCvSharp.VideoCapture(OpenCvSharp.CaptureDevice.Any, 0);
        object m_sync = new object();

        public CameraSourceProperties Properties { get; private set; }

        public sealed override ImagePropertiesBase PropertiesBase { get { return this.Properties; } }

        public CameraSource()
        {
            this.Properties = new CameraSourceProperties(StartCamera, CloseCamera);

            EnumSources();
        }

        private void EnumSources()
        {
            int sourceId = 0;
            this.Properties.CameraSourceIds.Add(-1);
            while (true)
            {
                try
                {
                    using(Mat image = new Mat() )
                    using (OpenCvSharp.VideoCapture videoCapture = new OpenCvSharp.VideoCapture(OpenCvSharp.CaptureDevice.Any, sourceId))
                    {
                        if (videoCapture.IsOpened() && videoCapture.Read(image))
                            this.Properties.CameraSourceIds.Add(sourceId++);
                        else
                            break;
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        OpenCvSharp.Mat m_image = new OpenCvSharp.Mat();

        protected override Mat ExecuteImpl(Mat inputImage)
        {
            lock (m_sync)
            {
                if (m_videoCapture != null)
                {
                    if (m_videoCapture.Read(m_image) && m_image.Width != 0 && m_image.Height != 0)
                        return m_image.Clone();
                }
            }

            return null;
        }

        void CloseCamera()
        {
            lock (m_sync)
            {
                if (m_videoCapture != null)
                    m_videoCapture.Dispose();
                m_videoCapture = null;

                if (m_image != null)
                    m_image.Dispose();
            }
        }

        void StartCamera()
        {
            lock (m_sync)
            {
                System.Diagnostics.Debug.Assert(m_videoCapture == null);

                if (m_videoCapture == null && this.Properties.CameraSourceId >= 0)
                {
                    try
                    {
                        m_videoCapture = new OpenCvSharp.VideoCapture(OpenCvSharp.CaptureDevice.Any, this.Properties.CameraSourceId);
                        m_image = new Mat();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Cannot start camera.\nException:\n" + ex.ToString());
                    }
                }
            }
        }
    }
}
