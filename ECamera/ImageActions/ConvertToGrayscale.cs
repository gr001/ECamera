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
    public class ConvertToGrayscaleProperties : ImagePropertiesBase
    {
        public ConvertToGrayscaleProperties() : base("Convert to grayscale")
        { }
    }

    public class ConvertToGrayscale : ImageAction
    {
        public ConvertToGrayscaleProperties Properties { get; private set; } = new ConvertToGrayscaleProperties();

        public sealed override ImagePropertiesBase PropertiesBase { get { return this.Properties; } }

        public ConvertToGrayscale()
        {
        }

        protected override Mat ExecuteImpl(Mat inputImage)
        {
            return inputImage.CvtColor(OpenCvSharp.ColorConversionCodes.BGR2GRAY);
        }
    }
}
