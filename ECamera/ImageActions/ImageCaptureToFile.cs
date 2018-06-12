using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Windows;
using ECamera.ImageActions;

namespace ECamera
{
    public class ImageCaptureToFileProperties : ImagePropertiesBase
    {
        public ImageCaptureToFileProperties() : base("Image capture to file")
        {
        }

        public string FilePath
        {
            get { return m_filePath; }
            set
            {

                if (m_filePath != value)
                {
                    m_filePath = value;
                    NotifyPropertyChanged();
                }
            }
        }
        string m_filePath;

        public bool CaptureRequested
        {
            get { return m_captureRequested; }
            set
            {
                if (m_captureRequested != value)
                {
                    m_captureRequested = value;
                    NotifyPropertyChanged();
                }
            }
        }
        volatile bool m_captureRequested = false;
    }

    public class ImageCaptureToFile : ImageAction
    {
        public ImageCaptureToFileProperties Properties { get; } = new ImageCaptureToFileProperties();
        public sealed override ImagePropertiesBase PropertiesBase { get { return this.Properties; } }

        public ImageCaptureToFile()
        {
        }

        protected override Mat ExecuteImpl(Mat inputImage)
        {
            if (this.Properties.CaptureRequested)
            {
                Mat clonedImage = inputImage.Clone();
                string filePath = this.Properties.FilePath;
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback((obj) =>
                {
                    try
                    {
                        clonedImage.SaveImage(filePath);
                    }
                    catch
                    {
                        clonedImage.Dispose();
                    }
                }));

                this.Properties.CaptureRequested = false;
            }

            return inputImage;
        }
    }
}
