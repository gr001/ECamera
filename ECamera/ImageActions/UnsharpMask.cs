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
    public class UnsharpMaskProperties : ImagePropertiesBase
    {
        Action m_updateAction;
        public UnsharpMaskProperties(Action updateAction) : base("Unsharp masking")
        {
            m_updateAction = updateAction;
        }

        [RangeValueAttribute(0, 20)]
        public double Amount
        {
            get { return m_amount; }
            set
            {
                if (value != m_amount)
                {
                    if (value >= 0 && value <= 20)
                    {
                        m_amount = value;
                        m_updateAction();
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        double m_amount = 1;

        [RangeValueAttribute(0, 255)]
        public double Threshold
        {
            get { return m_threshold; }
            set
            {
                if (value != m_threshold)
                {
                    if (value >= 0 && value <= 255)
                    {
                        m_threshold = value;
                        m_updateAction();
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        double m_threshold = 0;

        [RangeValueAttribute(0, 20)]
        public double Sigma
        {
            get { return m_sigma; }
            set
            {
                if (value != m_sigma)
                {
                    if (value > 0 && value <= 20)
                    {
                        m_sigma = value;
                        m_updateAction();
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        double m_sigma = 1;
    }

    public class UnsharpMask : ImageAction
    {
        public UnsharpMaskProperties Properties { get; private set; }

        public sealed override ImagePropertiesBase PropertiesBase { get { return this.Properties; } }

        public UnsharpMask()
        {
            this.Properties = new UnsharpMaskProperties(UpdateUnsharpMask);
            UpdateUnsharpMask();
        }

        protected override Mat ExecuteImpl(Mat inputImage)
        {
            Mat blurred = new Mat();
            int kernelSize = (int)Math.Round(this.Properties.Sigma * 3);

            bool isGrayScale = inputImage.Type().Channels == 1;

            if (!isGrayScale)
                OpenCvSharp.Cv2.CvtColor(inputImage, inputImage, ColorConversionCodes.BGR2HSV_FULL);

            kernelSize = Math.Max(3, kernelSize);
            if (kernelSize % 2 == 0)
                kernelSize++;

            OpenCvSharp.Cv2.GaussianBlur(inputImage, blurred, new Size(kernelSize, kernelSize), this.Properties.Sigma, this.Properties.Sigma);

            Mat sharpened = inputImage * (1 + this.Properties.Amount) - blurred * this.Properties.Amount;

            if (this.Properties.Threshold > 0)
            {
                Mat lowConstrastMask = (inputImage - blurred).Abs();
                lowConstrastMask = lowConstrastMask.LessThan(this.Properties.Threshold);
                inputImage.CopyTo(sharpened, lowConstrastMask);
            }

            if (!isGrayScale)
                OpenCvSharp.Cv2.CvtColor(sharpened, sharpened, ColorConversionCodes.HSV2BGR_FULL);

            return sharpened;
        }

        private void UpdateUnsharpMask()
        {
        }
    }
}
