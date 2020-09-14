using System;
using System.Runtime.Serialization;
namespace Wanghzh.Prism.Modularity
{
    [Serializable]
    public partial class DuplicateModuleException
    {
        protected DuplicateModuleException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
