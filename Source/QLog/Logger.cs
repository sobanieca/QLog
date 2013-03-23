using System;
using System.Collections.Generic;
using QLog.Models;
using System.Threading;
using QLog.Helpers;
using System.Diagnostics;
using System.Reflection;
using QLog.Exceptions;
using QLog.Areas.Base;
using QLog.Areas.Default;
using QLog.Components;

namespace QLog
{
    /// <summary>
    /// Main tool that is responsible for logging
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Writes a log at the QTrace area with a specified message
        /// </summary>
        /// <param name="msg"></param>
        public static void LogTrace(string msg)
        {
            DoLog<QTrace>(msg, null);
        }

        /// <summary>
        /// Writes a log at the QTrace area with a specified message (applying String.Format() first)
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void LogTrace(string msg, params object[] args)
        {
            DoLog<QTrace>(msg, null, args);
        }

        /// <summary>
        /// Writes a log at the QTrace area with a specified exception
        /// </summary>
        /// <param name="e"></param>
        public static void LogTrace(Exception e)
        {
            DoLog<QTrace>(null, e);
        }

        /// <summary>
        /// Writes a log at the QTrace area with a specified message and exception
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public static void LogTrace(string msg, Exception e)
        {
            DoLog<QTrace>(msg, e);
        }

        /// <summary>
        /// Writes a log at the QDebug area with a specified message
        /// </summary>
        /// <param name="msg"></param>
        public static void LogDebug(string msg)
        {
            DoLog<QDebug>(msg, null);
        }

        /// <summary>
        /// Writes a log at the QDebug area with a specified message (applying String.Format() first)
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void LogDebug(string msg, params object[] args)
        {
            DoLog<QDebug>(msg, null, args);
        }

        /// <summary>
        /// Writes a log at the QDebug area with a specified exception
        /// </summary>
        /// <param name="e"></param>
        public static void LogDebug(Exception e)
        {
            DoLog<QDebug>(null, e);
        }

        /// <summary>
        /// Writes a log at the QDebug area with a specified message and exception
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public static void LogDebug(string msg, Exception e)
        {
            DoLog<QDebug>(msg, e);
        }

        /// <summary>
        /// Writes a log at the QInfo area with a specified message
        /// </summary>
        /// <param name="msg"></param>
        public static void LogInfo(string msg)
        {
            DoLog<QInfo>(msg, null);
        }

        /// <summary>
        /// Writes a log at the QInfo area with a specified message (applying String.Format() first)
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void LogInfo(string msg, params object[] args)
        {
            DoLog<QInfo>(msg, null, args);
        }

        /// <summary>
        /// Writes a log at the QInfo area with a specified exception
        /// </summary>
        /// <param name="e"></param>
        public static void LogInfo(Exception e)
        {
            DoLog<QInfo>(null, e);
        }

        /// <summary>
        /// Writes a log at the QInfo area with a specified message and exception
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public static void LogInfo(string msg, Exception e)
        {
            DoLog<QInfo>(msg, e);
        }

        /// <summary>
        /// Writes a log at the QWarn area with a specified message
        /// </summary>
        /// <param name="msg"></param>
        public static void LogWarn(string msg)
        {
            DoLog<QWarn>(msg, null);
        }

        /// <summary>
        /// Writes a log at the QWarn area with a specified message (applying String.Format() first)
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void LogWarn(string msg, params object[] args)
        {
            DoLog<QWarn>(msg, null, args);
        }

        /// <summary>
        /// Writes a log at the QWarn area with a specified exception
        /// </summary>
        /// <param name="e"></param>
        public static void LogWarn(Exception e)
        {
            DoLog<QWarn>(null, e);
        }

        /// <summary>
        /// Writes a log at the QWarn area with a specified message and exception
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public static void LogWarn(string msg, Exception e)
        {
            DoLog<QWarn>(msg, e);
        }

        /// <summary>
        /// Writes a log at the QError area with a specified message
        /// </summary>
        /// <param name="msg"></param>
        public static void LogError(string msg)
        {
            DoLog<QError>(msg, null);
        }

        /// <summary>
        /// Writes a log at the QError area with a specified message (applying String.Format() first)
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void LogError(string msg, params object[] args)
        {
            DoLog<QError>(msg, null, args);
        }

        /// <summary>
        /// Writes a log at the QError area with a specified exception
        /// </summary>
        /// <param name="e"></param>
        public static void LogError(Exception e)
        {
            DoLog<QError>(null, e);
        }

        /// <summary>
        /// Writes a log at the QError area with a specified message and exception
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public static void LogError(string msg, Exception e)
        {
            DoLog<QError>(msg, e);
        }

        /// <summary>
        /// Writes a log at the QCritical area with a specified message
        /// </summary>
        /// <param name="msg"></param>
        public static void LogCritical(string msg)
        {
            DoLog<QCritical>(msg, null);
        }

        /// <summary>
        /// Writes a log at the QCritical area with a specified message (applying String.Format() first)
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void LogCritical(string msg, params object[] args)
        {
            DoLog<QCritical>(msg, null, args);
        }

        /// <summary>
        /// Writes a log at the QCritical area with a specified exception
        /// </summary>
        /// <param name="e"></param>
        public static void LogCritical(Exception e)
        {
            DoLog<QCritical>(null, e);
        }

        /// <summary>
        /// Writes a log at the QCritical area with a specified message and exception
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public static void LogCritical(string msg, Exception e)
        {
            DoLog<QCritical>(msg, e);
        }

        /// <summary>
        /// Writes a log at the specified QArea with a specified message
        /// </summary>
        /// <typeparam name="Area"></typeparam>
        /// <param name="msg"></param>
        public static void Log<Area>(string msg) where Area : QArea
        {
            DoLog<Area>(msg, null);
        }

        /// <summary>
        /// Writes a log at the specified QArea with a specified message (applying String.Format() first)
        /// </summary>
        /// <typeparam name="Area"></typeparam>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void Log<Area>(string msg, params object[] args) where Area : QArea
        {
            DoLog<Area>(msg, null, args);
        }

        /// <summary>
        /// Writes a log at the specified QArea with a specified exception
        /// </summary>
        /// <typeparam name="Area"></typeparam>
        /// <param name="e"></param>
        public static void Log<Area>(Exception e) where Area : QArea
        {
            DoLog<Area>(null, e);
        }

        /// <summary>
        /// Writes a log at the specified QArea with a specified message and exception
        /// </summary>
        /// <typeparam name="Area"></typeparam>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public static void Log<Area>(string msg, Exception e) where Area : QArea
        {
            DoLog<Area>(msg, e);
        }

        /// <summary>
        /// Runs a new thread that saves asynchronously all of the logs stored in the IBuffer using IRepository
        /// </summary>
        public static void Flush()
        {
            Flush(true);
        }

        /// <summary>
        /// Saves all of the logs stored in the IBuffer using IRepository
        /// </summary>
        /// <param name="async">Specifies whether saving to the database should be done asynchronously or synchronously</param>
        public static void Flush(bool async)
        {
            try
            {
                if (ComponentsService.Config.IsAsyncLogEnabled())
                {
                    if (!ComponentsService.Buffer.IsEmpty())
                    {
                        List<QLogEntry> logEntries = ComponentsService.Buffer.DequeueLogEntries();
                        if (async)
                        {
                            Thread saveThread = new Thread(new ParameterizedThreadStart(FlushBuffer));
                            saveThread.Start(logEntries);
                        }
                        else
                            FlushBuffer(logEntries);
                    }
                }
            }
            catch (Exception e)
            {
                ComponentsService.SilentModeHandle(e);
            }
        }

        /// <summary>
        /// Creates and saves a log message at the specified QArea. Depending on the settings the message is being saved to buffer 
        /// that will be flushed asynchronously later, or directly to the database via IRepository.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="args"></param>
        private static void DoLog<Area>(string msg, Exception exception, params object[] args) where Area : QArea
        {
            try
            {
                if (ComponentsService.Config.IsValidLogArea(typeof(Area)))
                {
                    msg = MessageHelper.GetMessage(msg, exception, args);
                    Tuple<string, string> ClassMethod = GetCallingClassAndMethod();
                    QLogEntry log = ComponentsService.Environment.GetLog(typeof(Area), msg, ClassMethod.Item1, ClassMethod.Item2);
                    if (ComponentsService.Config.IsAsyncLogEnabled(typeof(Area)))
                    {
                        bool isBufferFull = false;
                        ComponentsService.Buffer.Enqueue(log, out isBufferFull);
                        if (isBufferFull)
                            Flush();
                    }
                    else
                        ComponentsService.Repository.Save(log);
                }
            }
            catch (Exception e)
            {
                ComponentsService.SilentModeHandle(e);
            }
        }

        /// <summary>
        /// Saves the buffer content into the database using given IRepository
        /// </summary>
        private static void FlushBuffer(object arg)
        {
            try
            {
                List<QLogEntry> logEntries = (List<QLogEntry>)arg;
                ComponentsService.Repository.SaveAll(logEntries);
            }
            catch (Exception e)
            {
                ComponentsService.SilentModeHandle(e);
            }
        }

        /// <summary>
        /// Retrieves the class and method that called log method. If stacktrace usage is disabled, then no informations about method and class will be set.
        /// </summary>
        /// <returns></returns>
        private static Tuple<string, string> GetCallingClassAndMethod()
        {
            string methodName = "";
            string className = "";
            if (ComponentsService.Config.IsStacktraceEnabled())
            {
                try
                {
                    StackTrace st = new StackTrace(3, false);
                    StackFrame frame = st.GetFrame(0);
                    MethodBase method = frame.GetMethod();
                    if (method.ReflectedType != null)
                    {
                        methodName = method.Name;
                        className = method.ReflectedType.FullName;
                    }
                    return new Tuple<string, string>(className, methodName);
                }
                catch (Exception) { }
            }
            return new Tuple<string, string>(className, methodName);
        }

    }
}
