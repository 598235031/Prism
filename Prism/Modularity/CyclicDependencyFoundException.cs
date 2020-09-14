using System;
namespace Wanghzh.Prism.Modularity
{
    public partial class CyclicDependencyFoundException : ModularityException
    {
        public CyclicDependencyFoundException() : base() { }
        public CyclicDependencyFoundException(string message) : base(message) { }
        public CyclicDependencyFoundException(string message, Exception innerException) : base(message, innerException) { }
        public CyclicDependencyFoundException(string moduleName, string message, Exception innerException)
            : base(moduleName, message, innerException)
        {
        }
    }
}
