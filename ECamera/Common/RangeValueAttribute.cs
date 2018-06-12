using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECamera.Common
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class RangeValueAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236

        // This is a positional argument    
        public RangeValueAttribute(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

        // This is a named argument
        public int Min { get; set; }
        public int Max { get; set; }
    }
}
