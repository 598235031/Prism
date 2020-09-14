using System;
using System.Runtime.Serialization;
namespace Wanghzh.Prism.Modularity
{
    [Serializable]
    public partial class ModuleTypeLoaderNotFoundException
    {
        protected ModuleTypeLoaderNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
