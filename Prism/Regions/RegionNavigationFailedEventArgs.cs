using System;
namespace Wanghzh.Prism.Regions
{
    public class RegionNavigationFailedEventArgs : EventArgs
    {
        public RegionNavigationFailedEventArgs(NavigationContext navigationContext)
        {
            if (navigationContext == null)
            {
                throw new ArgumentNullException("navigationContext");
            }
            this.NavigationContext = navigationContext;
        }
        public RegionNavigationFailedEventArgs(NavigationContext navigationContext, Exception error)
            : this(navigationContext)
        {
            this.Error = error;
        }
        public NavigationContext NavigationContext { get; private set; }
        public Exception Error { get; private set; }
        public Uri Uri
        {
            get
            {
                if (this.NavigationContext != null)
                {
                    return this.NavigationContext.Uri;
                }
                return null;
            }
        }
    }
}
