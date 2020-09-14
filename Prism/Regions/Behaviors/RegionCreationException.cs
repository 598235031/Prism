using System;
namespace Wanghzh.Prism.Regions.Behaviors
{
    public partial class RegionCreationException : Exception
    {
        public RegionCreationException()
        {
        }
        public RegionCreationException(string message) 
            : base(message)
        {
        }
        public RegionCreationException(string message, Exception inner) 
            : base(message, inner)
        {
        }
    }
}
