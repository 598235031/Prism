using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using Unity;
using Wanghzh.Prism.Logging;
using Wanghzh.Prism.Messaging;
using Wanghzh.Prism.Properties;
using System.Reflection;
using System.Xml;
using Wanghzh.Prism.Modularity;
using System.Reflection.Emit;
using System.Threading;
using System.Windows.Forms;

namespace Wanghzh.Prism
{
    /// <summary>
    ///   create by wanghzh 20.6.8
    /// </summary>
    public class Mefstrapper : Bootstrapper, IDisposable
    {
        protected IRegionViewRegistry Container { get; private set; }
        string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
        IUserLogin loginInst = null;
        IUserUI formUIInst = null;
        public override void Run(bool runWithDefaultConfiguration)
        {
            this.Logger = this.CreateLogger();
            if (this.Logger == null)
            {
                throw new InvalidOperationException("no logger");
            }

            this.Logger.Log("start running", Category.Debug, Priority.Low);
            this.Logger.Log("ConfigureModuleCatalog", Category.Debug, Priority.Low);
            this.ConfigureModuleCatalog();

            this.Container = this.CreateContainer();
            if (this.Container == null)
            {
                throw new InvalidOperationException(" NullCompositionContainerException");
            }
            this.Logger.Log("ConfiguringServiceLocatorSingleton", Category.Debug, Priority.Low);
            this.ConfigureServiceLocator();


            this.Logger.Log("CreateModuleCatalog", Category.Debug, Priority.Low);
            this.ModuleCatalog = this.CreateModuleCatalog();
            if (this.ModuleCatalog == null)
            {
                throw new InvalidOperationException("no ModuleCatalog");
            }

            this.Logger.Log("InitializeModules", Category.Debug, Priority.Low);
            this.InitializeModules();


            this.Logger.Log("CreatingShell", Category.Debug, Priority.Low);
            this.Shell = this.CreateShell();
            if (this.Shell != null)
            {
                this.Logger.Log("InitializingShell", Category.Debug, Priority.Low);
                this.InitializeShell();
            }
            this.Logger.Log("BootstrapperSequenceCompleted", Category.Debug, Priority.Low);
        }

        protected IRegionViewRegistry CreateContainer()
        {
            return new UnityContainer();
        }

        protected override void ConfigureServiceLocator()
        {
            this.Container.Register<IMessenger>(new Messenger());
        }

        protected override DependencyObject CreateShell()
        {
            this.Container.GetInstance<IMessenger>().Register<bool>(this, "login", (result =>
            {
                if (result)
                { 
                    var ret = formUIInst.Init();
                    ((Form)loginInst.GetInstance()).Hide();
                    ((Form)formUIInst.Instance).Show();
                }
            }));
            this.Container.GetInstance<IMessenger>().Register<string>(this, "close", (result =>
            {
                ((Form)loginInst.GetInstance()).Close();
            }));
            formUIInst = Container.GetInstance<IUserUI>(Common.UserFrmTag);
            loginInst = this.Container.GetInstance<IUserLogin>(Common.UserLoginTag);

            return new DependencyObject();
        }

        ModuleInfoGroup shareModule = new ModuleInfoGroup() { InitializationMode = InitializationMode.OnDemand };
        List<ModuleInfoGroup> modulegroup = new List<ModuleInfoGroup>();
        List<IModule> activatorModules = new List<IModule>();
        protected override void InitializeModules()
        {
            //优先加载 共享项目
            foreach (var item in shareModule)
            {
                try
                {
                    Assembly.LoadFile(applicationPath + item.ModuleName);
                }
                catch (Exception ex)
                {
                    this.Logger.Log("加载共享程序集失败 " + ex.Message, Category.Exception, Priority.High);
                }
            }
            //加载 程序集 组
            foreach (var group in modulegroup)
            {

                //优先加载 共享程序集
                var shareModules = group.Where<ModuleInfo>(x => x.InitializationMode == InitializationMode.OnDemand);
                foreach (var item in shareModules.ToList())
                {
                    try
                    {
                        Assembly.LoadFile(applicationPath + item.ModuleName);
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Log("加载程序集组的共享程序集失败 " + ex.Message, Category.Exception, Priority.High);
                    }

                }

                foreach (var moduleInfo in group)
                {
                    if (moduleInfo.InitializationMode == InitializationMode.OnDemand) continue;

                    //优先加载 依赖项目
                    foreach (var item in moduleInfo.DependsOn)
                    {
                        var ass = Assembly.LoadFile(applicationPath + item);
                        if (!string.IsNullOrEmpty(item))
                        {
                            var tempmodleInterface = ass.GetType(moduleInfo.ModuleType);
                            if (tempmodleInterface != null)
                            {
                                if (typeof(IModule).IsAssignableFrom(tempmodleInterface)) //并且是集成该接口
                                {
                                    IModule moduleType = (IModule)ass.CreateInstance(moduleInfo.ModuleType);
                                    moduleType.Initialize(this.Container);
                                    activatorModules.Add(moduleType);
                                }
                            }
                        }
                    }
                    var assem = Assembly.LoadFile(applicationPath + moduleInfo.ModuleName);
                    if (string.IsNullOrEmpty(moduleInfo.ModuleType))
                    {
                        continue;
                    }
                    var modleInterface = assem.GetType(moduleInfo.ModuleType);
                    if (modleInterface != null)
                    {
                        if (typeof(IModule).IsAssignableFrom(modleInterface)) //并且是集成该接口
                        {


                            IModule moduleType = (IModule)assem.CreateInstance(moduleInfo.ModuleType);
                            moduleType.Initialize(this.Container);
                            activatorModules.Add(moduleType);

                        }
                        else
                        {
                            //该类型 没有继承imodule
                        }
                    }
                    else
                    {
                        //没有找到   activtor的类型
                    }
                }


            }
        }

        protected override void ConfigureModuleCatalog()
        {
            //读取配置文件
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + "config.xml");
            //共享程序，优先加载
            var shareNodes = doc.SelectNodes("ROOT/Share/ModuleInfo");
            foreach (XmlNode item in shareNodes)
            {
                shareModule.Add(new ModuleInfo() { ModuleName = item.Attributes["ModuleName"].InnerText, ModuleType = item.Attributes["ModuleType"].InnerText });
            }

            var appNode = doc.SelectNodes("ROOT/APP/ModuleInfoGroup");
            var listModulegroup = new List<string>();
            foreach (XmlNode item in appNode)
            {
                listModulegroup.Add(item.InnerText);
            }

            appNode = doc.SelectNodes("ROOT/ModuleInfoGroup");
            var list = new List<XmlNode>(); //本系统使用 程序集组
            foreach (XmlNode item in appNode)
            {
                var moduleName = item.Attributes["key"].InnerText;
                if (listModulegroup.Contains(moduleName))
                {
                    list.Add(item);
                }
            }
            //扫描 每个程序集组
            foreach (XmlNode item in list)
            {
                ModuleInfoGroup group = new ModuleInfoGroup() { InitializationMode = InitializationMode.WhenAvailable };
                //解析 模块的共享程序集
                var shareModuleNode = item.SelectNodes("Share/ModuleInfo");
                foreach (XmlNode node in shareModuleNode)
                {
                    group.Add(new ModuleInfo() { ModuleName = node.Attributes["ModuleName"].InnerText, ModuleType = node.Attributes["ModuleType"].InnerText, InitializationMode = InitializationMode.OnDemand });

                }

                var moduleNodes = item.SelectNodes("ModuleInfo");
                foreach (XmlNode modulenode in moduleNodes)
                {
                    var localModule = new ModuleInfo() { ModuleName = modulenode.Attributes["ModuleName"].InnerText, ModuleType = modulenode.Attributes["ModuleType"].InnerText, InitializationMode = InitializationMode.WhenAvailable };
                    //解析模块的依赖项目
                    foreach (XmlNode depentnode in modulenode.SelectNodes("DependentModules/Module"))
                    {
                        localModule.DependsOn.Add(depentnode.InnerText.Trim());
                    }
                    group.Add(localModule);
                }
                //加入 队列
                modulegroup.Add(group);
            }
        }

        protected override void InitializeShell()
        {
            Thread.CurrentThread.IsBackground = true;
            System.Windows.Forms.Application.Run(loginInst.GetInstance() as Form);

         }

        public void Dispose()
        {
            shareModule.Clear();
            modulegroup.Clear();

            foreach (var item in activatorModules)
            {
                var inst = item as IDisposable;
                if (inst != null)
                    inst.Dispose();
            }
            try
            {
                System.Windows.Forms.Application.ExitThread();
                System.Windows.Forms.Application.Exit();
                System.Environment.Exit(0);
            }
            catch
            {

            }
        }
    }
}
