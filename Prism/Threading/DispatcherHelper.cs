using System;
using System.Text;
#if NETFX_CORE
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.Foundation;
#else
using System.Windows.Threading;
#if SILVERLIGHT
using System.Windows;
#endif
#endif
namespace Wanghzh.Prism.Threading
{
    public static class DispatcherHelper
    {
#if NETFX_CORE
        public static CoreDispatcher UIDispatcher
#else
        public static Dispatcher UIDispatcher
#endif
        {
            get;
            private set;
        }
        public static void CheckBeginInvokeOnUI(Action action)
        {
            if (action == null)
            {
                return;
            }
            CheckDispatcher();
#if NETFX_CORE
            if (UIDispatcher.HasThreadAccess)
#else
            if (UIDispatcher.CheckAccess())
#endif
            {
                action();
            }
            else
            {
#if NETFX_CORE
                UIDispatcher.RunAsync(CoreDispatcherPriority.Normal,  () => action());
#else
                UIDispatcher.BeginInvoke(action);
#endif
            }
        }
        private static void CheckDispatcher()
        {
            if (UIDispatcher == null)
            {
                var error = new StringBuilder("The DispatcherHelper is not initialized.");
                error.AppendLine();
#if SILVERLIGHT
#if WINDOWS_PHONE
                error.Append("Call DispatcherHelper.Initialize() at the end of App.InitializePhoneApplication.");
#else
                error.Append("Call DispatcherHelper.Initialize() in Application_Startup (App.xaml.cs).");
#endif
#elif NETFX_CORE
                error.Append("Call DispatcherHelper.Initialize() at the end of App.OnLaunched.");
#else
                error.Append("Call DispatcherHelper.Initialize() in the static App constructor.");
#endif
                throw new InvalidOperationException(error.ToString());
            }
        }
#if NETFX_CORE
        public static IAsyncAction RunAsync(Action action)
#else
        public static DispatcherOperation RunAsync(Action action)
#endif
        {
            CheckDispatcher();
#if NETFX_CORE
            return UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
#else
            return UIDispatcher.BeginInvoke(action);
#endif
        }
        public static void Initialize()
        {
#if SILVERLIGHT
            if (UIDispatcher != null)
#else
#if NETFX_CORE
            if (UIDispatcher != null)
#else
            if (UIDispatcher != null
                && UIDispatcher.Thread.IsAlive)
#endif
#endif
            {
                return;
            }
#if NETFX_CORE
            UIDispatcher = Window.Current.Dispatcher;
#else
#if SILVERLIGHT
            UIDispatcher = Deployment.Current.Dispatcher;
#else
            UIDispatcher = Dispatcher.CurrentDispatcher;
#endif
#endif
        }
        public static void Reset()
        {
            UIDispatcher = null;
        }
    }
}
