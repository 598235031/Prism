using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

namespace Wanghzh.Prism
{
    /// <summary>
    /// 公开系统的模块接口
    /// </summary>
    public interface IRegionViewRegistry
    {
        bool IsRegistered<T>();
        bool IsRegistered<T>(string key);

        /// <summary>
        /// 注册单例模式
        /// </summary>
        /// <typeparam name="TInterface">对象</typeparam>
        /// <param name="instance">实例对象</param>
        /// <param name="key">默认 “”</param>
        void Register<TInterface>(TInterface instance, string key = "") where TInterface: class;
        /// <summary>
        /// 注册 接口实例
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="key"></param>
        void Register<TInterface, TClass>(string key = "")  where TInterface : class  where TClass : class, TInterface;


        /// <summary>
        /// 卸载实例
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="instance"></param>
        void Unregister<TClass>(TClass instance)  where TClass : class;
        /// <summary>
        /// 卸载实例
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="key"></param>
        void Unregister<TClass>(string key="")   where TClass : class;

        /// <summary>
        /// 默认实例对象
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        TService GetInstance<TService>();
        /// <summary>
        /// 带有key 实例
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        TService GetInstance<TService>(string key);
        /// <summary>
        /// 获取所有实例
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        IEnumerable<TService> GetAllInstances<TService>();

    }
}
