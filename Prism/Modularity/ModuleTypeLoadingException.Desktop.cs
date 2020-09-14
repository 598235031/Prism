using System;
using System.Runtime.Serialization;
namespace Wanghzh.Prism.Modularity
{
    [Serializable]
    public partial class ModuleTypeLoadingException
    {
        protected ModuleTypeLoadingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
