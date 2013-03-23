using System;

namespace QLog.Exceptions
{
    /// <summary>
    /// Exception that is being thrown when logger is unable to connect to the database.
    /// </summary>
    internal class QLogDataSourcePostfixException : ApplicationException
    {
        internal QLogDataSourcePostfixException(string message)
            : base(message)
        {

        }
    }
}
