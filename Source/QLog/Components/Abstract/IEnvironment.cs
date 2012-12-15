using System;
using QLog.Models;

namespace QLog.Components.Abstract
{
    /// <summary>
    /// Interface for retrieving log entry depending on the environment (Desktop app, Web app, etc.)
    /// </summary>
    public interface IEnvironment
    {
        /// <summary>
        /// Returns QLogEntry that will be stored inside data source.
        /// </summary>
        /// <param name="area"></param>
        /// <param name="msg"></param>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        QLogEntry GetLog(Type area, string msg, string className, string methodName);
    }
}
