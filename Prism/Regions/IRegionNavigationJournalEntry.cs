using System;
namespace Wanghzh.Prism.Regions
{
    public interface IRegionNavigationJournalEntry
    {
        Uri Uri { get; set; }
        NavigationParameters Parameters { get; set; }
    }
}
