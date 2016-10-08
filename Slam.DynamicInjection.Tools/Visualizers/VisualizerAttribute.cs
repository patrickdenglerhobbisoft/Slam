using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Slam.Tools
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class VisualizerAttribute : System.Attribute
    {
        public VisualizerCategory Category;
        public Size Size;

        public VisualizerAttribute(VisualizerCategory Category)
        {
            this.Category = Category;
            Size = new Size(300, 300);
        }
    }
  
}
