using System;
namespace Wanghzh.Prism.Regions
{
    public partial class UpdateRegionsException : Exception
    {
        public UpdateRegionsException()
        {
        }
        public UpdateRegionsException(string message) 
            : base(message)
        {
        }
        public UpdateRegionsException(string message, Exception inner) 
            : base(message, inner)
        {
        }
    }
}
