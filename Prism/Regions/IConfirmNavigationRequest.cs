using System;
namespace Wanghzh.Prism.Regions
{
    public interface IConfirmNavigationRequest : INavigationAware
    {
        void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback);
    }
}
