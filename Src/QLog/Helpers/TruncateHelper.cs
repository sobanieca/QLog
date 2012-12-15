
using QLog.Models;
namespace QLog.Helpers
{
    /// <summary>
    /// Helper for truncating strings to the specified length. Used mainly to truncate all fields of the QLog
    /// that are longer then the corresponding columns definition in the database.
    /// </summary>
    internal static class TruncateHelper
    {
        //Columns width in the QLog table lengths:
        private const int AREA_MAX_LENGTH = 50;
        private const int AREA_COLOR_MAX_LENGTH = 20;
        private const int THREAD_ID_MAX_LENGTH = 50;
        private const int SESSION_ID_MAX_LENGTH = 200;
        private const int USER_AGENT_MAX_LENGTH = 500;
        private const int USER_HOST_MAX_LENGTH = 50;
        private const int CLASS_MAX_LENGTH = 500;
        private const int METHOD_MAX_LENGTH = 300;

        internal static void Truncate(QLogEntry log)
        {
            log.Area = TruncateString(log.Area, AREA_MAX_LENGTH);
            log.AreaColor = TruncateString(log.AreaColor, AREA_COLOR_MAX_LENGTH);
            log.ThreadId = TruncateString(log.ThreadId, THREAD_ID_MAX_LENGTH);
            log.SessionId = TruncateString(log.SessionId, SESSION_ID_MAX_LENGTH);
            log.UserAgent = TruncateString(log.UserAgent, USER_AGENT_MAX_LENGTH);
            log.UserHost = TruncateString(log.UserHost, USER_HOST_MAX_LENGTH);
            log.Class = TruncateString(log.Class, CLASS_MAX_LENGTH);
            log.Method = TruncateString(log.Method, METHOD_MAX_LENGTH);
        }

        private static string TruncateString(string s, int maxLength)
        {
            if (s.Length > maxLength)
            {
                return s.Substring(0, maxLength);
            }
            return s;
        }
    }
}
