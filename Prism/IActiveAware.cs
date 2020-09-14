using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Wanghzh.Prism
{
    public interface IActiveAware
    {
        bool IsActive { get; set; }
        event EventHandler IsActiveChanged;
    }
}
