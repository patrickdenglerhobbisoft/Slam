using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Hobbisoft.Slam.Tools.Analyzers
{
    public enum VisualizerCategory  { Database =1 , Slotworker =2 , General =4, Profling = 8 }
    public interface IVisualizer
    {

        //event EventHandler Loaded;
        //event EventHandler Unloaded;

    }
}
