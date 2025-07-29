using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Services.Utility
{
    public interface ILoggerService<T> where T : class
    {
        void LogInfo(string message, params object[] args);

        void LogWarning(string message, params object[] args);

        void LogError(string message, params object[] args);

        void LogDebug(string message, params object[] args);

        void LogFatal(string message, params object[] args);

        void LogTrace(string message, params object[] args);

        void LogException(string message, Exception ex, params object[] args);

        // Custom Functions

        void MethodStart(string method);

        void MethodEnd(string method);
    }
}
