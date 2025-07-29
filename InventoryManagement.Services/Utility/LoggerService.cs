using Serilog;

namespace InventoryManagement.Services.Utility
{
    public class LoggerService<T> : ILoggerService<T> where T : class
    {
        private readonly ILogger _logger;

        public LoggerService() 
        {
            _logger = Log.ForContext<T>();
        }

        public void LogInfo(string message, params object[] args)
        {
            _logger.Information(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.Warning(message, args);
        }

        public void LogError(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        public void LogFatal(string message, params object[] args)
        {
            _logger.Fatal(message, args);
        }

        public void LogTrace(string message, params object[] args)
        {
            _logger.Verbose(message, args);
        }

        public void LogException(string message, Exception ex, params object[] args)
        {
            _logger.Error(message.ToString(), ex, args);
        }

        // Custom Functions

        public void MethodStart(string method)
        {
            _logger.Information($"Method Begin: {method}");
        }

        public void MethodEnd(string method)
        {
            _logger.Information($"Method End: {method}");
        }
    }
}
