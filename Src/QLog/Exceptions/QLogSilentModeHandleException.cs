using System;

namespace QLog.Exceptions
{
    /// <summary>
    /// Exception that is being thrown when logger is unable to verify if the silent mode is enabled or disabled.
    /// </summary>
    internal class QLogSilentModeHandleException : ApplicationException
    {
        public QLogSilentModeHandleException(string message, Exception e) : base(message, e)
        {
            
        }
    }
}
