using System;
using System.Globalization;
using System.IO;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Logging
{
    public class TextLogger : ILoggerFacade, IDisposable
    {
        private readonly TextWriter writer;
        public TextLogger()
            : this(Console.Out)
        {
        }
        public TextLogger(TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            this.writer = writer;
        }
        public void Log(string message, Category category, Priority priority)
        {
            string messageToLog = String.Format(CultureInfo.InvariantCulture, Resources.DefaultTextLoggerPattern, DateTime.Now,
                                                category.ToString().ToUpper(CultureInfo.InvariantCulture), message, priority.ToString());
            writer.WriteLine(messageToLog);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (writer != null)
                {
                    writer.Dispose();
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
