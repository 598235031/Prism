using ShareLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanghzh.Prism;

namespace ClassLibrary1
{
    public class Activator : IModule
    {
 
        public void Initialize(IRegionViewRegistry registry)
        {
            Console.WriteLine( DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+ "  "+ this.ToString() +" start.....");

            registry.Register<IAction, Teacher>("teacher");
        }
    }
}
