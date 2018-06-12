using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using ECamera.ImageActions;

namespace ECamera
{
    public class FlippingProperties : ImagePropertiesBase
    {
        public FlippingProperties() : base("Flipping")
        {
        }

        public bool FlipHorizontally
        {
            get { return m_flipHorizontally; }
            set
            {
                if (value != m_flipHorizontally)
                {
                    m_flipHorizontally = value;
                    NotifyPropertyChanged();
                }
            }
        }
        bool m_flipHorizontally;

        public bool FlipVertically
        {
            get { return m_flipVertically; }
            set
            {
                if (value != m_flipVertically)
                {
                    m_flipVertically = value;
                    NotifyPropertyChanged();
                }
            }
        }
        bool m_flipVertically;
    }

    public class Flipping : ImageAction
    {
        public FlippingProperties Properties { get; private set; } = new FlippingProperties();

        public sealed override ImagePropertiesBase PropertiesBase { get { return this.Properties; } }

        public Flipping()
        {
        }

        protected override Mat ExecuteImpl(Mat inputImage)
        {
            FlipMode? flipMode = null;

            if (this.Properties.FlipVertically && this.Properties.FlipHorizontally)
                flipMode = FlipMode.XY;
            else if (this.Properties.FlipVertically)
                flipMode = FlipMode.Y;
            else if (this.Properties.FlipHorizontally)
                flipMode = FlipMode.X;

            if (flipMode.HasValue)
                OpenCvSharp.Cv2.Flip(inputImage, inputImage, flipMode.Value);

            return inputImage;
        }
    }
}
