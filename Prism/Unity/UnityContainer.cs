using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using Unity.Lifetime;

namespace Wanghzh.Prism
{
    /// <summary>
    /// ioc  use  unity  
    /// </summary>
    /// <remarks>
    /// create by wanghzh 20.6.2
    /// </remarks>
    public sealed class UnityContainer : IRegionViewRegistry
    {
        readonly Unity.IUnityContainer Container = new Unity.UnityContainer();

    

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return Container.ResolveAll<TService>();
        }

        public TService GetInstance<TService>()
        {
          return  Container.Resolve<TService>();
        }

        public TService GetInstance<TService>(string key)
        {
           return Container.Resolve<TService>(key);
        }

        public bool IsRegistered<T>()
        {
            return Container.IsRegistered<T>();
        }

        public bool IsRegistered<T>(string key)
        {
            return Container.IsRegistered<T>(key);
        }

        public void Register<TInterface>(TInterface instance, string key = "") where TInterface : class
        {
            if (string.IsNullOrEmpty(key))
            {
                //Container.RegisterInstance<TInterface>(instance, new ContainerControlledLifetimeManager);
                Container.RegisterInstance<TInterface>(instance);
            }
            else
            {
                Container.RegisterInstance<TInterface>(key, instance);
            }
        }

        public void Unregister<TClass>(TClass instance) where TClass : class
        {
            throw new NotImplementedException();
        }

        public void Unregister<TClass>(string key = "") where TClass : class
        {
            throw new NotImplementedException();
        }

        void IRegionViewRegistry.Register<TInterface, TClass>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {

                Container.RegisterType<TInterface, TClass>();

            }
            else
            {
                Container.RegisterType<TInterface, TClass>(key);
            }
        }
    }
}
