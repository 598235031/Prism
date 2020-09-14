using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wanghzh.Prism
{
    /// <summary>
    /// 用户界面
    /// </summary>
    public interface IUserUI
    {
        Ret Init();

        object Instance
        {
            get;
        }
    }
}
