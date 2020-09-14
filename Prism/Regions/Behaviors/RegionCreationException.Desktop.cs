using System;
using System.Runtime.Serialization;
namespace Wanghzh.Prism.Regions.Behaviors
{
    [Serializable]
    public partial class RegionCreationException
    {
        protected RegionCreationException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
