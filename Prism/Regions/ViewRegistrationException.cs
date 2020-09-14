using System;
namespace Wanghzh.Prism.Regions
{
    public partial class ViewRegistrationException : Exception
    {
        public ViewRegistrationException()
        {
        }
        public ViewRegistrationException(string message) : base(message)
        {
        }
        public ViewRegistrationException(string message, Exception inner) : base(message, inner)
        {
        }
       
    }
}
