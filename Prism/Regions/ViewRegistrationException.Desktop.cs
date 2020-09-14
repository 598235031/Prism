using System;
using System.Runtime.Serialization;
namespace Wanghzh.Prism.Regions
{
    [Serializable]
    public partial class ViewRegistrationException
    {
        protected ViewRegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
