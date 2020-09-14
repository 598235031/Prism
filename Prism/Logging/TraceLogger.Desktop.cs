using System.Diagnostics;
namespace Wanghzh.Prism.Logging
{
    public class TraceLogger : ILoggerFacade
    {
        public void Log(string message, Category category, Priority priority)
        {
            if (category == Category.Exception)
            {
                Trace.TraceError(message);
            }
            else
            {
                Trace.TraceInformation(message);
            }
        }
    }
}
