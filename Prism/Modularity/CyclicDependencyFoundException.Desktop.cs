using System;
using System.Runtime.Serialization;
namespace Wanghzh.Prism.Modularity
{
    [Serializable]
    public partial class CyclicDependencyFoundException 
    {
        protected CyclicDependencyFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
