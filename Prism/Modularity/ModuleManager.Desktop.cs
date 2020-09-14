using System.Collections.Generic;
namespace Wanghzh.Prism.Modularity
{
    public partial class ModuleManager
    {
        public virtual IEnumerable<IModuleTypeLoader> ModuleTypeLoaders
        {
            get
            {
                if (this.typeLoaders == null)
                {
                    this.typeLoaders = new List<IModuleTypeLoader>
                                          {
                                              new FileModuleTypeLoader()
                                          };
                }
                return this.typeLoaders;
            }
            set
            {
                this.typeLoaders = value;
            }
        }
    }
}
