using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECamera.Common;

namespace ECamera.ImageActions
{
    public class ImagePropertiesBase : ObservableBase
    {
        public ImagePropertiesBase(string description)
        {
            this.Description = description;
        }

        public string Description { get; private set;}

        public bool IsEnabled
        {
            get { return m_isEnabled; }
            set
            {
                if (m_isEnabled != value)
                {
                    m_isEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }
        volatile bool m_isEnabled = true;
    }
}
