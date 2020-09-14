using ShareLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanghzh.Prism;

namespace UI
{
    public class Activator : IModule
    {
 
        public  static   IRegionViewRegistry Insatce
        {
            get;
            private set;
        }

        public void Initialize(IRegionViewRegistry registry)
        {
            Console.WriteLine( DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+ "  "+ this.ToString() +" start.....");

            Insatce = registry;

            Test.Action();
        }
    }

   
}
