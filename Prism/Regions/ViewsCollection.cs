using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
namespace Wanghzh.Prism.Regions
{
    public partial class ViewsCollection : IViewsCollection
    {
        private readonly ObservableCollection<ItemMetadata> subjectCollection;
        private readonly Dictionary<ItemMetadata, MonitorInfo> monitoredItems =
            new Dictionary<ItemMetadata, MonitorInfo>();
        private readonly Predicate<ItemMetadata> filter;
        private Comparison<object> sort;
        private List<object> filteredItems = new List<object>();
        public ViewsCollection(ObservableCollection<ItemMetadata> list, Predicate<ItemMetadata> filter)
        {
            this.subjectCollection = list;
            this.filter = filter;
            this.MonitorAllMetadataItems();
            this.subjectCollection.CollectionChanged += this.SourceCollectionChanged;
            this.UpdateFilteredItemsList();
        }
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public Comparison<object> SortComparison
        {
            get { return this.sort; }
            set
            {
                if (this.sort != value)
                {
                    this.sort = value;
                    this.UpdateFilteredItemsList();
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }
        private IEnumerable<object> FilteredItems
        {
            get { return this.filteredItems; }
        }
        public bool Contains(object value)
        {
            return this.FilteredItems.Contains(value);
        }
        public IEnumerator<object> GetEnumerator()
        {
            return
                this.FilteredItems.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler handler = this.CollectionChanged;
            if (handler != null) handler(this, e);
        }
        private void NotifyReset()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        private void ResetAllMonitors()
        {
            this.RemoveAllMetadataMonitors();
            this.MonitorAllMetadataItems();
        }
        private void MonitorAllMetadataItems()
        {
            foreach (var item in this.subjectCollection)
            {
                this.AddMetadataMonitor(item, this.filter(item));
            }
        }
        private void RemoveAllMetadataMonitors()
        {
            foreach (var item in this.monitoredItems)
            {
                item.Key.MetadataChanged -= this.OnItemMetadataChanged;
            }
            this.monitoredItems.Clear();
        }
        private void AddMetadataMonitor(ItemMetadata itemMetadata, bool isInList)
        {
            itemMetadata.MetadataChanged += this.OnItemMetadataChanged;
            this.monitoredItems.Add(
                itemMetadata,
                new MonitorInfo
                    {
                        IsInList = isInList
                    });
        }
        private void RemoveMetadataMonitor(ItemMetadata itemMetadata)
        {
            itemMetadata.MetadataChanged -= this.OnItemMetadataChanged;
            this.monitoredItems.Remove(itemMetadata);
        }
        private void OnItemMetadataChanged(object sender, EventArgs e)
        {
            ItemMetadata itemMetadata = (ItemMetadata) sender;
            MonitorInfo monitorInfo;
            bool foundInfo = this.monitoredItems.TryGetValue(itemMetadata, out monitorInfo);
            if (!foundInfo) return;
            if (this.filter(itemMetadata))
            {
                if (!monitorInfo.IsInList)
                {
                    monitorInfo.IsInList = true;
                    this.UpdateFilteredItemsList();
                    NotifyAdd(itemMetadata.Item);
                }
            }
            else
            {
                monitorInfo.IsInList = false;
                this.RemoveFromFilteredList(itemMetadata.Item);
            }
        }
        private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.UpdateFilteredItemsList();
                    foreach (ItemMetadata itemMetadata in e.NewItems)
                    {
                        bool isInFilter = this.filter(itemMetadata);
                        this.AddMetadataMonitor(itemMetadata, isInFilter);
                        if (isInFilter)
                        {
                            NotifyAdd(itemMetadata.Item);
                        }
                    }
                    if (this.sort != null)
                    {
                        this.NotifyReset();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (ItemMetadata itemMetadata in e.OldItems)
                    {
                        this.RemoveMetadataMonitor(itemMetadata);
                        if (this.filter(itemMetadata))
                        {
                            this.RemoveFromFilteredList(itemMetadata.Item);
                        }
                    }
                    break;
                default:
                    this.ResetAllMonitors();
                    this.UpdateFilteredItemsList();
                    this.NotifyReset();
                    break;
            }
        }
        private void NotifyAdd(object item)
        {
            int newIndex = this.filteredItems.IndexOf(item);
            this.NotifyAdd(new[] { item }, newIndex);
        }
        
        private void RemoveFromFilteredList(object item)
        {
            int index = this.filteredItems.IndexOf(item);
            this.UpdateFilteredItemsList();
            this.NotifyRemove(new[] { item }, index);
        }
        private void UpdateFilteredItemsList()
        {
            this.filteredItems = this.subjectCollection.Where(i => this.filter(i)).Select(i => i.Item)
                .OrderBy<object, object>(o => o, new RegionItemComparer(this.SortComparison)).ToList();
        }
        
        private class MonitorInfo
        {
            public bool IsInList { get; set; }
        }
        private class RegionItemComparer : Comparer<object>
        {
            private readonly Comparison<object> comparer;
            public RegionItemComparer(Comparison<object> comparer)
            {
                this.comparer = comparer;
            }
            public override int Compare(object x, object y)
            {
                if (this.comparer == null)
                {
                    return 0;
                }
                return this.comparer(x, y);
            }
        }
    }
}
