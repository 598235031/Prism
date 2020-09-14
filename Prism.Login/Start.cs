using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wanghzh.Prism.Login
{
    public class Start : IModule
    {

        public static IRegionViewRegistry Registry
        {
            get; set;
        }

        public void Initialize(IRegionViewRegistry registry)
        {
            Registry = registry;
            registry.Register<IUserLogin>(new StanderLogin(), Common.UserLoginTag);
        }

    }
}
