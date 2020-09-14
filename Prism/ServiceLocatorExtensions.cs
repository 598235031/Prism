using CommonServiceLocator;
using System;
 
namespace Wanghzh.Prism
{
    public static class ServiceLocatorExtensions
    {
        public static object TryResolve(this IServiceLocator locator, Type type)
        {
            if (locator == null) throw new ArgumentNullException("locator");
            try
            {
                return locator.GetInstance(type);
            }
            catch (ActivationException)
            {
                return null;
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T TryResolve<T>(this IServiceLocator locator) where T : class
        {
            return locator.TryResolve(typeof(T)) as T;
        }
    }
}
