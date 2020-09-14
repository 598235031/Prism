using System.Configuration;
namespace Wanghzh.Prism.Modularity
{
    public class ConfigurationStore : IConfigurationStore
    {
        public ModulesConfigurationSection RetrieveModuleConfigurationSection()
        {
            return ConfigurationManager.GetSection("modules") as ModulesConfigurationSection;
        }
    }
}
