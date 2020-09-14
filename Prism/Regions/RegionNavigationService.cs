using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using CommonServiceLocator;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Regions
{
    public class RegionNavigationService : IRegionNavigationService
    {
        private readonly IServiceLocator serviceLocator;
        private readonly IRegionNavigationContentLoader regionNavigationContentLoader;
        private IRegionNavigationJournal journal;
        private NavigationContext currentNavigationContext;
        public RegionNavigationService(IServiceLocator serviceLocator, IRegionNavigationContentLoader regionNavigationContentLoader, IRegionNavigationJournal journal)
        {
            if (serviceLocator == null)
            {
                throw new ArgumentNullException("serviceLocator");
            }
            if (regionNavigationContentLoader == null)
            {
                throw new ArgumentNullException("regionNavigationContentLoader");
            }
            if (journal == null)
            {
                throw new ArgumentNullException("journal");
            }
            this.serviceLocator = serviceLocator;
            this.regionNavigationContentLoader = regionNavigationContentLoader;
            this.journal = journal;
            this.journal.NavigationTarget = this;
        }
        public IRegion Region { get; set; }
        public IRegionNavigationJournal Journal
        {
            get
            {
                return this.journal;
            }
        }
        public event EventHandler<RegionNavigationEventArgs> Navigating;
        private void RaiseNavigating(NavigationContext navigationContext)
        {
            if (this.Navigating != null)
            {
                this.Navigating(this, new RegionNavigationEventArgs(navigationContext));
            }
        }
        public event EventHandler<RegionNavigationEventArgs> Navigated;
        private void RaiseNavigated(NavigationContext navigationContext)
        {
            if (this.Navigated != null)
            {
                this.Navigated(this, new RegionNavigationEventArgs(navigationContext));
            }
        }
        public event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed;
        private void RaiseNavigationFailed(NavigationContext navigationContext, Exception error)
        {
            if (this.NavigationFailed != null)
            {
                this.NavigationFailed(this, new RegionNavigationFailedEventArgs(navigationContext, error));
            }
        }
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is marshalled to callback")]
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback)
        {
            this.RequestNavigate(target, navigationCallback, null);
        }
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            if (navigationCallback == null) throw new ArgumentNullException("navigationCallback");
            try
            {
                this.DoNavigate(target, navigationCallback, navigationParameters);
            }
            catch (Exception e)
            {
                this.NotifyNavigationFailed(new NavigationContext(this, target), navigationCallback, e);
            }
        }
        private void DoNavigate(Uri source, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (this.Region == null)
            {
                throw new InvalidOperationException(Resources.NavigationServiceHasNoRegion);
            }
            this.currentNavigationContext = new NavigationContext(this, source, navigationParameters);
            RequestCanNavigateFromOnCurrentlyActiveView(
                this.currentNavigationContext,
                navigationCallback,
                this.Region.ActiveViews.ToArray(),
                0);
        }
        private void RequestCanNavigateFromOnCurrentlyActiveView(
            NavigationContext navigationContext,
            Action<NavigationResult> navigationCallback,
            object[] activeViews,
            int currentViewIndex)
        {
            if (currentViewIndex < activeViews.Length)
            {
                var vetoingView = activeViews[currentViewIndex] as IConfirmNavigationRequest;
                if (vetoingView != null)
                {
                    vetoingView.ConfirmNavigationRequest(
                        navigationContext,
                        canNavigate =>
                        {
                            if (this.currentNavigationContext == navigationContext && canNavigate)
                            {
                                RequestCanNavigateFromOnCurrentlyActiveViewModel(
                                    navigationContext,
                                    navigationCallback,
                                    activeViews,
                                    currentViewIndex);
                            }
                            else
                            {
                                this.NotifyNavigationFailed(navigationContext, navigationCallback, null);
                            }
                        });
                }
                else
                {
                    RequestCanNavigateFromOnCurrentlyActiveViewModel(
                        navigationContext,
                        navigationCallback,
                        activeViews,
                        currentViewIndex);
                }
            }
            else
            {
                ExecuteNavigation(navigationContext, activeViews, navigationCallback);
            }
        }
        private void RequestCanNavigateFromOnCurrentlyActiveViewModel(
            NavigationContext navigationContext,
            Action<NavigationResult> navigationCallback,
            object[] activeViews,
            int currentViewIndex)
        {
            var frameworkElement = activeViews[currentViewIndex] as FrameworkElement;
            if (frameworkElement != null)
            {
                var vetoingViewModel = frameworkElement.DataContext as IConfirmNavigationRequest;
                if (vetoingViewModel != null)
                {
                    vetoingViewModel.ConfirmNavigationRequest(
                        navigationContext,
                        canNavigate =>
                        {
                            if (this.currentNavigationContext == navigationContext && canNavigate)
                            {
                                RequestCanNavigateFromOnCurrentlyActiveView(
                                    navigationContext,
                                    navigationCallback,
                                    activeViews,
                                    currentViewIndex + 1);
                            }
                            else
                            {
                                this.NotifyNavigationFailed(navigationContext, navigationCallback, null);
                            }
                        });
                    return;
                }
            }
            RequestCanNavigateFromOnCurrentlyActiveView(
                navigationContext,
                navigationCallback,
                activeViews,
                currentViewIndex + 1);
        }
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is marshalled to callback")]
        private void ExecuteNavigation(NavigationContext navigationContext, object[] activeViews, Action<NavigationResult> navigationCallback)
        {
            try
            {
                NotifyActiveViewsNavigatingFrom(navigationContext, activeViews);
                object view = this.regionNavigationContentLoader.LoadContent(this.Region, navigationContext);
                this.RaiseNavigating(navigationContext);
                this.Region.Activate(view);
                IRegionNavigationJournalEntry journalEntry = this.serviceLocator.GetInstance<IRegionNavigationJournalEntry>();
                journalEntry.Uri = navigationContext.Uri;
                journalEntry.Parameters = navigationContext.Parameters;
                this.journal.RecordNavigation(journalEntry);
                InvokeOnNavigationAwareElement(view, (n) => n.OnNavigatedTo(navigationContext));
                navigationCallback(new NavigationResult(navigationContext, true));
                this.RaiseNavigated(navigationContext);
            }
            catch (Exception e)
            {
                this.NotifyNavigationFailed(navigationContext, navigationCallback, e);
            }
        }
        private void NotifyNavigationFailed(NavigationContext navigationContext, Action<NavigationResult> navigationCallback, Exception e)
        {
            var navigationResult =
                e != null ? new NavigationResult(navigationContext, e) : new NavigationResult(navigationContext, false);
            navigationCallback(navigationResult);
            this.RaiseNavigationFailed(navigationContext, e);
        }
        private static void NotifyActiveViewsNavigatingFrom(NavigationContext navigationContext, object[] activeViews)
        {
            InvokeOnNavigationAwareElements(activeViews, (n) => n.OnNavigatedFrom(navigationContext));
        }
        private static void InvokeOnNavigationAwareElements(IEnumerable<object> items, Action<INavigationAware> invocation)
        {
            foreach (var item in items)
            {
                InvokeOnNavigationAwareElement(item, invocation);
            }
        }
        private static void InvokeOnNavigationAwareElement(object item, Action<INavigationAware> invocation)
        {
            var navigationAwareItem = item as INavigationAware;
            if (navigationAwareItem != null)
            {
                invocation(navigationAwareItem);
            }
            FrameworkElement frameworkElement = item as FrameworkElement;
            if (frameworkElement != null)
            {
                INavigationAware navigationAwareDataContext = frameworkElement.DataContext as INavigationAware;
                if (navigationAwareDataContext != null)
                {
                    invocation(navigationAwareDataContext);
                }
            }
        }
    }
}
