using System;
using System.Windows;
using Wanghzh.Prism.Regions.Behaviors;
namespace Wanghzh.Prism.Regions
{
    public static class RegionContext
    {
        private static readonly DependencyProperty ObservableRegionContextProperty =
            DependencyProperty.RegisterAttached("ObservableRegionContext", typeof(ObservableObject<object>), typeof(RegionContext), null);
        public static ObservableObject<object> GetObservableContext(DependencyObject view)
        {
            if (view == null) throw new ArgumentNullException("view");
            ObservableObject<object> context = view.GetValue(ObservableRegionContextProperty) as ObservableObject<object>;
            if (context == null)
            {
                context = new ObservableObject<object>();
                view.SetValue(ObservableRegionContextProperty, context);
            }
           
            return context;
        }
    }
}
