using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Wanghzh.Prism.Regions
{
    public class RegionNavigationEventArgs : EventArgs
    {
        public RegionNavigationEventArgs(NavigationContext navigationContext)
        {
            if (navigationContext == null)
            {
                throw new ArgumentNullException("navigationContext");
            }
            this.NavigationContext = navigationContext;
        }
        public NavigationContext NavigationContext { get; private set; }
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
