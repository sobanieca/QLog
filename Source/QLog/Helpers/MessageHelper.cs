using System;

namespace QLog.Helpers
{
    /// <summary>
    /// Simple helper for obtaining message text basing on the passed arguments.
    /// </summary>
    internal static class MessageHelper
    {
        internal static string GetMessage(string msg, Exception e, params object[] args)
        {
            if (!String.IsNullOrWhiteSpace(msg))
            {
                if (args.Length > 0)
                    return String.Format(msg, args);
                if (e == null)
                    return msg;
                else
                {
                    string exceptionMessage = GetExceptionMessage(e);
                    string innerExceptionMessage = GetInnerExceptionMessage(e);
                    return String.Format("{0}, Exception: {1}, InnerException: {2}", msg, exceptionMessage, innerExceptionMessage);
                }
            }
            if (e != null)
            {
                string exceptionMessage = GetExceptionMessage(e);
                string innerExceptionMessage = GetInnerExceptionMessage(e);
                return String.Format("Exception: {0}, InnerException: {1}", exceptionMessage, innerExceptionMessage);
            }
            return "";
        }

        private static string GetInnerExceptionMessage(Exception e)
        {
            if (!(e.InnerException == null))
            {
                string result = "";
                if (!String.IsNullOrWhiteSpace(e.InnerException.Message))
                    result += e.InnerException.Message;
                if (!String.IsNullOrWhiteSpace(e.InnerException.StackTrace))
                    result += e.InnerException.StackTrace;
                return result;
            }
            else
                return "[None]";
        }

        private static string GetExceptionMessage(Exception e)
        {
            string result = "";
            if (!String.IsNullOrWhiteSpace(e.Message))
                result += e.Message;
            if (!String.IsNullOrWhiteSpace(e.StackTrace))
                result += e.StackTrace;
            return result;
        }
    }
}
