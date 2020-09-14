using System;
using System.Globalization;
namespace Wanghzh.Prism.Regions
{
    public class RegionNavigationJournalEntry : Wanghzh.Prism.Regions.IRegionNavigationJournalEntry
    {
        public Uri Uri { get; set; }
        public NavigationParameters Parameters { get; set; }
        public override string ToString()
        {
            if (this.Uri != null)
            {
                return string.Format(CultureInfo.CurrentCulture, "RegionNavigationJournalEntry:'{0}'", this.Uri.ToString());
            }
            return base.ToString();
        }
    }
}
