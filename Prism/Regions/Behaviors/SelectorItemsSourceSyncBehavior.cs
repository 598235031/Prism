using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Regions.Behaviors
{
    public class SelectorItemsSourceSyncBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        public static readonly string BehaviorKey = "SelectorItemsSourceSyncBehavior";
        private bool updatingActiveViewsInHostControlSelectionChanged;
        private Selector hostControl;
        public DependencyObject HostControl
        {
            get
            {
                return this.hostControl;
            }
            set
            {
                this.hostControl = value as Selector;
            }
        }
        protected override void OnAttach()
        {
            bool itemsSourceIsSet = this.hostControl.ItemsSource != null;
            itemsSourceIsSet = itemsSourceIsSet || (BindingOperations.GetBinding(this.hostControl, ItemsControl.ItemsSourceProperty) != null);
            if (itemsSourceIsSet)
            {
                throw new InvalidOperationException(Resources.ItemsControlHasItemsSourceException);
            }
            this.SynchronizeItems();
            this.hostControl.SelectionChanged += this.HostControlSelectionChanged;
            this.Region.ActiveViews.CollectionChanged += this.ActiveViews_CollectionChanged;
            this.Region.Views.CollectionChanged += this.Views_CollectionChanged;
        }
        private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int startIndex = e.NewStartingIndex;
                foreach (object newItem in e.NewItems)
                {
                    this.hostControl.Items.Insert(startIndex++, newItem);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object oldItem in e.OldItems)
                {
                    this.hostControl.Items.Remove(oldItem);
                }
            }
        }
        private void SynchronizeItems()
        {
            List<object> existingItems = new List<object>();
            foreach (object childItem in this.hostControl.Items)
            {
                existingItems.Add(childItem);
            }
            foreach (object view in this.Region.Views)
            {
                this.hostControl.Items.Add(view);
            }
            foreach (object existingItem in existingItems)
            {
                this.Region.Add(existingItem);
            }
        }
        private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.updatingActiveViewsInHostControlSelectionChanged)
            {
                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (this.hostControl.SelectedItem != null
                    && this.hostControl.SelectedItem != e.NewItems[0]
                    && this.Region.ActiveViews.Contains(this.hostControl.SelectedItem))
                {
                    this.Region.Deactivate(this.hostControl.SelectedItem);
                }
                this.hostControl.SelectedItem = e.NewItems[0];
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove &&
                     e.OldItems.Contains(this.hostControl.SelectedItem))
            {
                this.hostControl.SelectedItem = null;
            }
        }
        private void HostControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.updatingActiveViewsInHostControlSelectionChanged = true;
                object source;
                source = e.OriginalSource;
                if (source == sender)
                {
                    foreach (object item in e.RemovedItems)
                    {
                        if (this.Region.Views.Contains(item) && this.Region.ActiveViews.Contains(item))
                        {
                            this.Region.Deactivate(item);
                        }
                    }
                    foreach (object item in e.AddedItems)
                    {
                        if (this.Region.Views.Contains(item) && !this.Region.ActiveViews.Contains(item))
                        {
                            this.Region.Activate(item);
                        }
                    }
                }
            }
            finally
            {
                this.updatingActiveViewsInHostControlSelectionChanged = false;
            }
        }
    }
}
