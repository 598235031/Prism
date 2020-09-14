using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using CommonServiceLocator;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Regions
{
    public class Region : IRegion
    {
        private ObservableCollection<ItemMetadata> itemMetadataCollection;
        private string name;
        private ViewsCollection views;
        private ViewsCollection activeViews;
        private object context;
        private IRegionManager regionManager;
        private IRegionNavigationService regionNavigationService;
        private Comparison<object> sort;
        public Region()
        {
            this.Behaviors = new RegionBehaviorCollection(this);
            this.sort = Region.DefaultSortComparison;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public IRegionBehaviorCollection Behaviors { get; private set; }
        public object Context
        {
            get
            {
                return this.context;
            }
            set
            {
                if (this.context != value)
                {
                    this.context = value;
                    this.OnPropertyChanged("Context");
                }
            }
        }
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != null && this.name != value)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.CannotChangeRegionNameException, this.name));
                }
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(Resources.RegionNameCannotBeEmptyException);
                }
                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }
        public virtual IViewsCollection Views
        {
            get
            {
                if (this.views == null)
                {
                    this.views = new ViewsCollection(ItemMetadataCollection, x => true);
                    this.views.SortComparison = this.sort;
                }
                return this.views;
            }
        }
        public virtual IViewsCollection ActiveViews
        {
            get
            {
                if (this.activeViews == null)
                {
                    this.activeViews = new ViewsCollection(ItemMetadataCollection, x => x.IsActive);
                    this.activeViews.SortComparison = this.sort;
                }
                return this.activeViews;
            }
        }
        public Comparison<object> SortComparison
        {
            get
            {
                return this.sort;
            }
            set
            {
                this.sort = value;
                if (this.activeViews != null)
                {
                    this.activeViews.SortComparison = this.sort;
                }
                if (this.views != null)
                {
                    this.views.SortComparison = this.sort;
                }
            }
        }
        public IRegionManager RegionManager
        {
            get
            {
                return this.regionManager;
            }
            set
            {
                if (this.regionManager != value)
                {
                    this.regionManager = value;
                    this.OnPropertyChanged("RegionManager");
                }
            }
        }
        public IRegionNavigationService NavigationService
        {
            get
            {
                if (this.regionNavigationService == null)
                {
                    this.regionNavigationService = ServiceLocator.Current.GetInstance<IRegionNavigationService>();
                    this.regionNavigationService.Region = this;
                }
                return this.regionNavigationService;
            }
            set
            {
                this.regionNavigationService = value;
            }
        }
        protected virtual ObservableCollection<ItemMetadata> ItemMetadataCollection
        {
            get
            {
                if (this.itemMetadataCollection == null)
                {
                    this.itemMetadataCollection = new ObservableCollection<ItemMetadata>();
                }
                return this.itemMetadataCollection;
            }
        }
        public IRegionManager Add(object view)
        {
            return this.Add(view, null, false);
        }
        public IRegionManager Add(object view, string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.StringCannotBeNullOrEmpty, "viewName"));
            }
            return this.Add(view, viewName, false);
        }
        public virtual IRegionManager Add(object view, string viewName, bool createRegionManagerScope)
        {
            IRegionManager manager = createRegionManagerScope ? this.RegionManager.CreateRegionManager() : this.RegionManager;
            this.InnerAdd(view, viewName, manager);
            return manager;
        }
        public virtual void Remove(object view)
        {
            ItemMetadata itemMetadata = this.GetItemMetadataOrThrow(view);
            this.ItemMetadataCollection.Remove(itemMetadata);
            DependencyObject dependencyObject = view as DependencyObject;
            if (dependencyObject != null && Regions.RegionManager.GetRegionManager(dependencyObject) == this.RegionManager)
            {
                dependencyObject.ClearValue(Regions.RegionManager.RegionManagerProperty);
            }
        }
        public virtual void Activate(object view)
        {
            ItemMetadata itemMetadata = this.GetItemMetadataOrThrow(view);
            if (!itemMetadata.IsActive)
            {
                itemMetadata.IsActive = true;
            }
        }
        public virtual void Deactivate(object view)
        {
            ItemMetadata itemMetadata = this.GetItemMetadataOrThrow(view);
            if (itemMetadata.IsActive)
            {
                itemMetadata.IsActive = false;
            }
        }
        public virtual object GetView(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.StringCannotBeNullOrEmpty, "viewName"));
            }
            ItemMetadata metadata = this.ItemMetadataCollection.FirstOrDefault(x => x.Name == viewName);
            if (metadata != null)
            {
                return metadata.Item;
            }
            return null;
        }
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback)
        {
            this.RequestNavigate(target, navigationCallback, null);
        }
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            this.NavigationService.RequestNavigate(target, navigationCallback, navigationParameters);
        }
        private void InnerAdd(object view, string viewName, IRegionManager scopedRegionManager)
        {
            if (this.ItemMetadataCollection.FirstOrDefault(x => x.Item == view) != null)
            {
                throw new InvalidOperationException(Resources.RegionViewExistsException);
            }
            ItemMetadata itemMetadata = new ItemMetadata(view);
            if (!string.IsNullOrEmpty(viewName))
            {
                if (this.ItemMetadataCollection.FirstOrDefault(x => x.Name == viewName) != null)
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Resources.RegionViewNameExistsException, viewName));
                }
                itemMetadata.Name = viewName;
            }
            DependencyObject dependencyObject = view as DependencyObject;
            if (dependencyObject != null)
            {
                Regions.RegionManager.SetRegionManager(dependencyObject, scopedRegionManager);
            }
            this.ItemMetadataCollection.Add(itemMetadata);
        }
        private ItemMetadata GetItemMetadataOrThrow(object view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }
            ItemMetadata itemMetadata = this.ItemMetadataCollection.FirstOrDefault(x => x.Item == view);
            if (itemMetadata == null)
            {
                throw new ArgumentException(Resources.ViewNotInRegionException, "view");
            }
            return itemMetadata;
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
        public static int DefaultSortComparison(object x, object y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    Type xType = x.GetType();
                    Type yType = y.GetType();
                    ViewSortHintAttribute xAttribute = xType.GetCustomAttributes(typeof(ViewSortHintAttribute), true).FirstOrDefault() as ViewSortHintAttribute;
                    ViewSortHintAttribute yAttribute = yType.GetCustomAttributes(typeof(ViewSortHintAttribute), true).FirstOrDefault() as ViewSortHintAttribute;
                    return ViewSortHintAttributeComparison(xAttribute, yAttribute);
                }
            }
        }
        private static int ViewSortHintAttributeComparison(ViewSortHintAttribute x, ViewSortHintAttribute y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    return string.Compare(x.Hint, y.Hint, StringComparison.Ordinal);
                }
            }
        }
    }
}
