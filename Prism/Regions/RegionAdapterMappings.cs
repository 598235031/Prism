using System;
using System.Collections.Generic;
using System.Globalization;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Regions
{
    public class RegionAdapterMappings
    {
        private readonly Dictionary<Type, IRegionAdapter> mappings = new Dictionary<Type, IRegionAdapter>();
        public void RegisterMapping(Type controlType, IRegionAdapter adapter)
        {
            if (controlType == null)
            {
                throw new ArgumentNullException("controlType");
            }
            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }
            if (mappings.ContainsKey(controlType))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                                                                  Resources.MappingExistsException, controlType.Name));
            }
            mappings.Add(controlType, adapter);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "controlType")]
        public IRegionAdapter GetMapping(Type controlType)
        {
            Type currentType = controlType;
            while (currentType != null)
            {
                if (mappings.ContainsKey(currentType))
                {
                    return mappings[currentType];
                }
                currentType = currentType.BaseType;
            }
            throw new KeyNotFoundException(String.Format(CultureInfo.CurrentCulture, Resources.NoRegionAdapterException, controlType));
        }
    }
}
