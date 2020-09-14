using System;
using System.Runtime.Serialization;
namespace Wanghzh.Prism.Regions
{
    [Serializable]
    public partial class UpdateRegionsException
    {
        protected UpdateRegionsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
