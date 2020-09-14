using System;
namespace Wanghzh.Prism.Events
{
    [Obsolete]
    public interface IDispatcherFacade
    {
        void BeginInvoke(Delegate method, object arg);
    }
}
