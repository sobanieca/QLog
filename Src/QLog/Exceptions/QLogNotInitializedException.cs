using System;

namespace QLog.Exceptions
{
    /// <summary>
    /// Exception that is being thrown when logger hasn't been initialized. Namely - when Logger.Initialize() method hasn't been called.
    /// </summary>
    internal class QLogNotInitializedException : ApplicationException
    {
        public QLogNotInitializedException(string message) : base(message)
        {
            
        }
    }
}
