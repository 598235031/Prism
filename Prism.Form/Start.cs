using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanghzh.Prism.Form;

namespace Wanghzh.Prism.Form
{
    public class Start : IModule,IDisposable
    {

        public static IRegionViewRegistry Registry
        {
            get; set;
        }

        public void Dispose()
        {
           var inst=    Registry.GetInstance<IUserUI>(Common.UserFrmTag);

            (inst.Instance as System.Windows.Forms.Form).Dispose();
        }

        public void Initialize(IRegionViewRegistry registry)
        {
            Registry = registry;
            registry.Register<IUserUI>(new StanderForm(), Common.UserFrmTag);
        }

    }
}
