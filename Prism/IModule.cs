using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanghzh.Prism.Ioc;
using Wanghzh.Prism.Regions;

namespace Wanghzh.Prism
{
    /// <summary>
    /// 公开 用户自定义的模块
    /// </summary>
    public interface IModule
    {
        void Initialize(IRegionViewRegistry registry);
    }
}
