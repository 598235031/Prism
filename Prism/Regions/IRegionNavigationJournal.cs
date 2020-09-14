using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
namespace Wanghzh.Prism.Regions
{
    public interface IRegionNavigationJournal
    {
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        IRegionNavigationJournalEntry CurrentEntry {get;}
        INavigateAsync NavigationTarget { get; set; }
        void GoBack();
        void GoForward();
        void RecordNavigation(IRegionNavigationJournalEntry entry);
        void Clear();
    }
}
