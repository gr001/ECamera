using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using ECamera.Common;
using ECamera.ImageActions;

namespace ECamera
{
    public class ClaheProperties : ImagePropertiesBase
    {
        Action m_updateAction;
        public ClaheProperties(Action updateAction) : base("Clahe")
        {
            m_updateAction = updateAction;
        }

        public double? TileSize
        {
            get { return m_tileSize; }
            set
            {
                if (value != m_tileSize)
                {
                    m_tileSize = value;
                    m_updateAction();
                    NotifyPropertyChanged();
                }
            }
        }
        double? m_tileSize;

        [RangeValueAttribute(0, 255)]
        public double ClipLimit
        {
            get { return m_clipLimit; }
            set
            {
                if (value != m_clipLimit)
                {
                    if (value >= 0 && value <= 255)
                    {
                        m_clipLimit = value;
                        m_updateAction();
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        double m_clipLimit = 40;
    }

    public class Clahe : ImageAction
    {
        public ClaheProperties Properties { get; private set; }

        public sealed override ImagePropertiesBase PropertiesBase { get { return this.Properties; } }

        public Clahe()
        {
            this.Properties = new ClaheProperties(UpdateClahe);
            UpdateClahe();
        }

        protected override Mat ExecuteImpl(Mat inputImage)
        {
            var clahe = m_clahe;
            if (clahe == null || inputImage.Type().Channels != 1)
                return inputImage;

            //OpenCvSharp.Cv2.CvtColor(inputImage, inputImage, ColorConversionCodes.BGR2HSV_FULL);

            //OpenCvSharp.Mat outputImage = new Mat();
            clahe.Apply(inputImage, inputImage);

            //OpenCvSharp.Cv2.CvtColor(outputImage, outputImage, ColorConversionCodes.HSV2BGR_FULL);

            return inputImage;
        }

        private void UpdateClahe()
        {
            Size? tileSize = null;
            if (this.Properties.TileSize.HasValue)
                tileSize = new Size(this.Properties.TileSize.Value, this.Properties.TileSize.Value);

            var clahe = OpenCvSharp.CLAHE.Create(this.Properties.ClipLimit, tileSize);

            if (clahe != null)
            {
                var oldClahe = m_clahe;
                m_clahe = clahe;

                if (oldClahe != null)
                    oldClahe.Dispose();
            }
        }

        OpenCvSharp.CLAHE m_clahe;
    }
}
