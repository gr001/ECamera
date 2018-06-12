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
    public class ContrastStretchingProperties : ImagePropertiesBase
    {
        Action m_updateAction;
        public ContrastStretchingProperties(Action updateAction) : base("Contrast stretching")
        {
            m_updateAction = updateAction;
        }

        [RangeValueAttribute(0, 255)]
        public byte LowerThreshold
        {
            get { return m_lowerThreshold; }
            set
            {
                if (value != m_lowerThreshold)
                {
                    if (value >= 0 && value <= 255)
                    {
                        m_lowerThreshold = value;
                        m_updateAction();
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        byte m_lowerThreshold;

        [RangeValueAttribute(0, 255)]
        public byte UpperThreshold
        {
            get { return m_upperThreshold; }
            set
            {
                if (value != m_upperThreshold)
                {
                    if (value >= 0 && value <= 255)
                    {
                        m_upperThreshold = value;
                        m_updateAction();
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        byte m_upperThreshold = 255;
    }

    public class ContrastStretching : ImageAction
    {
        byte[] m_lut = new byte[256];

        public ContrastStretchingProperties Properties { get; private set; }

        public sealed override ImagePropertiesBase PropertiesBase { get { return this.Properties; } }

        public ContrastStretching()
        {
            this.Properties = new ContrastStretchingProperties(UpdateContrastStretching);
            UpdateContrastStretching();
        }

        protected override Mat ExecuteImpl(Mat inputImage)
        {
            var lut = m_lut;

            OpenCvSharp.Cv2.LUT(inputImage, lut, inputImage);

            return inputImage;
        }

        private void UpdateContrastStretching()
        {
            byte[] lut = new byte[256];

            double diff = (this.Properties.UpperThreshold - this.Properties.LowerThreshold);
            double a, b;

            if (diff == 0)
            {
                a = 0;
                b = 255;
            }
            else
            {
                a = 255 / (this.Properties.UpperThreshold - this.Properties.LowerThreshold);
                b = -a * this.Properties.LowerThreshold;
            }

            for (int i = 0; i <= 255; i++)
            {
                double val = a * i + b;

                if (val > 255)
                    val = 255;
                else if (val < 0)
                    val = 0;

                lut[i] = (byte)val;
            }

            m_lut = lut;
        }
    }
}
