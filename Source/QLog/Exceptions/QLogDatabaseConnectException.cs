using System;

namespace QLog.DataSource.Sql.Exceptions
{
    /// <summary>
    /// Exception that is being thrown when logger is unable to connect to the database.
    /// </summary>
    internal class QLogDatabaseConnectException : ApplicationException
    {
        internal QLogDatabaseConnectException(string message)
            : base(message)
        {

        }
    }
}
