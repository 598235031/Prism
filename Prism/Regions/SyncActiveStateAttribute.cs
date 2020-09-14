using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Wanghzh.Prism.Regions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class SyncActiveStateAttribute : Attribute
    {
    }
}
