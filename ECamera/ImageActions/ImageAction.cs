using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECamera.Common;
using ECamera.ImageActions;

namespace ECamera
{
    public abstract class ImageAction : ObservableBase
    {

        public abstract ImagePropertiesBase PropertiesBase { get; }

        public OpenCvSharp.Mat Execute(OpenCvSharp.Mat inputImage)
        {
            if (this.PropertiesBase.IsEnabled && inputImage != null)
               return ExecuteImpl(inputImage);

            return inputImage;
        }

        protected abstract OpenCvSharp.Mat ExecuteImpl(OpenCvSharp.Mat inputImage);

    }
}
