using System;

namespace QLog.Components.Abstract
{
    /// <summary>
    /// Interface for configuration reading.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Determines if log with provided area is valid within current configuration and should be stored.
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        bool IsValidLogArea(Type area);
        /// <summary>
        /// Determines whether stacktrace information is enabled.
        /// </summary>
        /// <returns></returns>
        bool IsStacktraceEnabled();
        /// <summary>
        /// Determines whether asynchronous logging is enabled.
        /// </summary>
        /// <returns></returns>
        bool IsAsyncLogEnabled();
        /// <summary>
        /// Determines whether asynchronous logging for given area is enabled.
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        bool IsAsyncLogEnabled(Type area);
        /// <summary>
        /// Determines whether silent mode is enabled.
        /// </summary>
        /// <returns></returns>
        bool IsSilentModeEnabled();
        /// <summary>
        /// Returns the QLog postfix that will be used when saving logs in data source. For instance it will be added to table name.
        /// </summary>
        /// <returns></returns>
        string GetDataSourcePostfix();
        /// <summary>
        /// Returns data source - for instance connection string.
        /// </summary>
        /// <returns></returns>
        string GetDataSource();
    }
}
