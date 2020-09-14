using System;
using System.Runtime.Serialization;
namespace Wanghzh.Prism.Modularity
{
    [Serializable]
    public partial class ModuleInitializeException
    {
        protected ModuleInitializeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
