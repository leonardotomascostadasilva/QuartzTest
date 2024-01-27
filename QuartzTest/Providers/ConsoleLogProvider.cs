using Quartz.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace QuartzTest.Providers
{
    public class ConsoleLogProvider : ILoggerProvider, ILogProvider
    {
        public Logger GetLogger(string name)
        {
            return (level, func, exception, parameters) =>
            {
                if (level >= (Quartz.Logging.LogLevel)LogLevel.Information && func != null)
                {
                    Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
                }
                return true;
            };
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        public ILogger CreateLogger(string categoryName)
        {
            throw new NotImplementedException();
        }
    }
}
