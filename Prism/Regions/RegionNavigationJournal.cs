using System;
using System.Collections.Generic;
namespace Wanghzh.Prism.Regions
{
    public class RegionNavigationJournal : IRegionNavigationJournal
    {
        private Stack<IRegionNavigationJournalEntry> backStack = new Stack<IRegionNavigationJournalEntry>();
        private Stack<IRegionNavigationJournalEntry> forwardStack = new Stack<IRegionNavigationJournalEntry>();
        private bool isNavigatingInternal;
        public INavigateAsync NavigationTarget { get; set; }
        public IRegionNavigationJournalEntry CurrentEntry { get; private set; }
        public bool CanGoBack
        {
            get
            {
                return this.backStack.Count > 0;
            }
        }
        public bool CanGoForward
        {
            get
            {
                return this.forwardStack.Count > 0;
            }
        }
        public void GoBack()
        {
            if (this.CanGoBack)
            {
                IRegionNavigationJournalEntry entry = this.backStack.Peek();
                this.InternalNavigate(
                    entry,
                    result =>
                    {
                        if (result)
                        {
                            if (this.CurrentEntry != null)
                            {
                                this.forwardStack.Push(this.CurrentEntry);
                            }
                            this.backStack.Pop();
                            this.CurrentEntry = entry;
                        }
                    });
            }
        }
        public void GoForward()
        {
            if (this.CanGoForward)
            {
                IRegionNavigationJournalEntry entry = this.forwardStack.Peek();
                this.InternalNavigate(
                    entry,
                    result =>
                    {
                        if (result)
                        {
                            if (this.CurrentEntry != null)
                            {
                                this.backStack.Push(this.CurrentEntry);
                            }
                            this.forwardStack.Pop();
                            this.CurrentEntry = entry;
                        }
                    });
            }
        }
        public void RecordNavigation(IRegionNavigationJournalEntry entry)
        {
            if (!this.isNavigatingInternal)
            {
                if (this.CurrentEntry != null)
                {
                    this.backStack.Push(this.CurrentEntry);
                }
                this.forwardStack.Clear();
                this.CurrentEntry = entry;
            }
        }
        public void Clear()
        {
            this.CurrentEntry = null;
            this.backStack.Clear();
            this.forwardStack.Clear();
        }
        private void InternalNavigate(IRegionNavigationJournalEntry entry, Action<bool> callback)
        {
            this.isNavigatingInternal = true;
            this.NavigationTarget.RequestNavigate(
                entry.Uri,
                nr =>
                {
                    this.isNavigatingInternal = false;
                    if (nr.Result.HasValue)
                    {
                        callback(nr.Result.Value);
                    }
                },
                entry.Parameters);
        }
    }
}
