using System;
using System.Runtime.Serialization;
namespace Wanghzh.Prism.Modularity
{
    [Serializable]
    public partial class ModuleNotFoundException
    {
        protected ModuleNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
