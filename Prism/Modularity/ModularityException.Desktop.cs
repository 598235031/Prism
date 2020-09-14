using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
namespace Wanghzh.Prism.Modularity
{
    [Serializable]
    public partial class ModularityException
    {
        protected ModularityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ModuleName = info.GetValue("ModuleName", typeof(string)) as string;
        }
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ModuleName", this.ModuleName);
        }
    }
}
