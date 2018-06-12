using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using ECamera.ImageActions;

namespace ECamera
{
    public class HistogramEqualizationProperties : ImagePropertiesBase
    {
        public HistogramEqualizationProperties() : base("Histogram equalization")
        { }
    }

    public class HistogramEqualization : ImageAction
    {
        public HistogramEqualizationProperties Properties { get; } = new HistogramEqualizationProperties();

        public sealed override ImagePropertiesBase PropertiesBase { get { return this.Properties; } }

        public HistogramEqualization()
        {
        }

        protected override Mat ExecuteImpl(Mat inputImage)
        {
            if (inputImage.Type().Channels == 1)
                OpenCvSharp.Cv2.EqualizeHist(inputImage, inputImage);

            return inputImage;
        }
    }
}
