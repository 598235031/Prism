using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using CommonServiceLocator;
using Wanghzh.Prism.Properties;
 
namespace Wanghzh.Prism.Regions
{
    public class RegionNavigationContentLoader : IRegionNavigationContentLoader
    {
        private readonly IServiceLocator serviceLocator;
        public RegionNavigationContentLoader(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }
        public object LoadContent(IRegion region, NavigationContext navigationContext)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (navigationContext == null) throw new ArgumentNullException("navigationContext");
            string candidateTargetContract = this.GetContractFromNavigationContext(navigationContext);
            var candidates = this.GetCandidatesFromRegion(region, candidateTargetContract);
            var acceptingCandidates =
                candidates.Where(
                    v =>
                    {
                        var navigationAware = v as INavigationAware;
                        if (navigationAware != null && !navigationAware.IsNavigationTarget(navigationContext))
                        {
                            return false;
                        }
                        var frameworkElement = v as FrameworkElement;
                        if (frameworkElement == null)
                        {
                            return true;
                        }
                        navigationAware = frameworkElement.DataContext as INavigationAware;
                        return navigationAware == null || navigationAware.IsNavigationTarget(navigationContext);
                    });
            var view = acceptingCandidates.FirstOrDefault();
            if (view != null)
            {
                return view;
            }
            view = this.CreateNewRegionItem(candidateTargetContract);
            region.Add(view);
            return view;
        }
        protected virtual object CreateNewRegionItem(string candidateTargetContract)
        {
            object newRegionItem;
            try
            {           
                newRegionItem = this.serviceLocator.GetInstance<object>(candidateTargetContract);
            }
            catch (ActivationException e)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Resources.CannotCreateNavigationTarget, candidateTargetContract),
                    e);
            }
            return newRegionItem;
        }
        protected virtual string GetContractFromNavigationContext(NavigationContext navigationContext)
        {
            if (navigationContext == null) throw new ArgumentNullException("navigationContext");
            var candidateTargetContract = UriParsingHelper.GetAbsolutePath(navigationContext.Uri);
            candidateTargetContract = candidateTargetContract.TrimStart('/');
            return candidateTargetContract;
        }
        protected virtual IEnumerable<object> GetCandidatesFromRegion(IRegion region, string candidateNavigationContract)
        {
            if (region == null) throw new ArgumentNullException("region");
            return region.Views.Where(v =>
                string.Equals(v.GetType().Name, candidateNavigationContract, StringComparison.Ordinal) ||
                string.Equals(v.GetType().FullName, candidateNavigationContract, StringComparison.Ordinal));
        }
    }
}
