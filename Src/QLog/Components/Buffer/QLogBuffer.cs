using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using QLog.Components.Abstract;
using QLog.Models;

namespace QLog.Components.Buffer
{
    /// <summary>
    /// Base buffer component that is being used by QLog forks.
    /// </summary>
    internal class QLogBuffer : IBuffer
    {
        //Inner queues that store QLog entries
        private static Queue<QLogEntry> _logEntries = new Queue<QLogEntry>();

        //Locker object used for thread synchronization
        private static readonly object _locker = new object();

        protected int _bufferMaxCount;

        protected const string QLOG_BUFFER_MAX_COUNT_KEY = "QLogBufferMaxCount";
        private const string LOGS_QUEUE_KEY = "QLogQueue";

        //Default value for buffer configuration
        private const int QLOG_BUFFER_MAX_COUNT_DEFAULT = 100;

        /// <summary>
        /// Creates new instance of desktop buffer
        /// </summary>
        public QLogBuffer()
        {
            _bufferMaxCount = GetBufferMaxCount();
        }

        /// <summary>
        /// Returns the maximum number of log entries that can be stored inside buffer. 
        /// If this number is exceeded then the logger will be force to flush content of the buffer to the database
        /// </summary>
        /// <returns></returns>
        private int GetBufferMaxCount()
        {
            string cfgBufferCount = ConfigurationManager.AppSettings[QLOG_BUFFER_MAX_COUNT_KEY];
            if (String.IsNullOrWhiteSpace(cfgBufferCount))
                return QLOG_BUFFER_MAX_COUNT_DEFAULT;
            int count = QLOG_BUFFER_MAX_COUNT_DEFAULT;
            Int32.TryParse(cfgBufferCount, out count);
            return count;
        }

        /// <summary>
        /// Saves single QLog entry in to the buffer and returns information whether buffer is full and should be flushed
        /// </summary>
        /// <param name="log"></param>
        /// <param name="isBufferFull"></param>
        public void Enqueue(QLogEntry log, out bool isBufferFull)
        {
            isBufferFull = false;
            if (!EnqueueWithAdditionalBuffer(log, out isBufferFull))
                EnqueueLogWithStaticBuffer(log, out isBufferFull);
        }

        /// <summary>
        /// Tries to enqueue an object with an additional buffer. If it's not possible - returns false.
        /// </summary>
        /// <returns></returns>
        private bool EnqueueWithAdditionalBuffer(QLogEntry log, out bool isBufferFull)
        {
            isBufferFull = false;
            if ((HttpContext.Current == null) || (HttpContext.Current.Items == null))
                return false;

            if (HttpContext.Current.Items[LOGS_QUEUE_KEY] != null)
            {
                Queue<QLogEntry> logEntries = (Queue<QLogEntry>)HttpContext.Current.Items[LOGS_QUEUE_KEY];
                logEntries.Enqueue(log);
            }
            else
            {
                Queue<QLogEntry> logEntries = new Queue<QLogEntry>();
                logEntries.Enqueue(log);
                HttpContext.Current.Items[LOGS_QUEUE_KEY] = logEntries;
            }
            if (IsRequestBufferFull())
                isBufferFull = true;

            return true;
        }

        /// <summary>
        /// Returns information whether the request specific buffer is full and should be flushed apart from the standard Flush() policy
        /// </summary>
        /// <returns></returns>
        private bool IsRequestBufferFull()
        {
            //No need to check if HttpContext.Current is not null since this method is being run inside a block that already verifies it
            Queue<QLogEntry> logEntries = new Queue<QLogEntry>();
            if (HttpContext.Current.Items[LOGS_QUEUE_KEY] != null)
                logEntries = (Queue<QLogEntry>)HttpContext.Current.Items[LOGS_QUEUE_KEY];
            if (logEntries.Count >= _bufferMaxCount)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Enqueues log entry using static queue buffer _logEntries
        /// </summary>
        /// <param name="log"></param>
        /// <param name="isBufferFull"></param>
        private void EnqueueLogWithStaticBuffer(QLogEntry log, out bool isBufferFull)
        {
            isBufferFull = false;
            lock (_locker)
            {
                _logEntries.Enqueue(log);
                if (IsStaticBufferFull())
                    isBufferFull = true;
            }
        }

        /// <summary>
        /// Retrieves QLog entries from the buffer that will be later saved to the database. Number of entries taken can be modified through configuration file. 
        /// Otherwise default value will be applied.
        /// </summary>
        /// <returns></returns>
        public List<QLogEntry> DequeueLogEntries()
        {
            List<QLogEntry> result = new List<QLogEntry>();

            result.AddRange(DequeueAdditionalBufferLogEntries());
            result.AddRange(DequeueStaticBufferLogEntries());

            return result;
        }

        /// <summary>
        /// Retrieves log entries from the request buffer.
        /// </summary>
        /// <returns></returns>
        private List<QLogEntry> DequeueAdditionalBufferLogEntries()
        {
            List<QLogEntry> result = new List<QLogEntry>();

            if ((HttpContext.Current != null) && (HttpContext.Current.Items != null))
            {
                if (HttpContext.Current.Items[LOGS_QUEUE_KEY] != null)
                {
                    Queue<QLogEntry> logEntries = (Queue<QLogEntry>)HttpContext.Current.Items[LOGS_QUEUE_KEY];
                    for (int i = 0; i < _bufferMaxCount; i++)
                    {
                        if (logEntries.Count > 0)
                            result.Add(logEntries.Dequeue());
                        else
                            break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves log entries from the static buffer.
        /// </summary>
        /// <returns></returns>
        private List<QLogEntry> DequeueStaticBufferLogEntries()
        {
            List<QLogEntry> result = new List<QLogEntry>();

            lock (_locker)
            {
                for (int i = 0; i < _bufferMaxCount; i++)
                {
                    if (_logEntries.Count > 0)
                    {
                        result.Add(_logEntries.Dequeue());
                    }
                    else
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns information whether the buffer is empty, or whether it should be flushed to the database
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            if (IsStaticBufferEmpty() && IsAdditionalBufferEmpty())
                return true;
            return false;
        }

        /// <summary>
        /// Returns information whether request specific buffer is empty
        /// </summary>
        /// <returns></returns>
        private bool IsAdditionalBufferEmpty()
        {
            if ((HttpContext.Current != null) && (HttpContext.Current.Items != null))
            {
                if (HttpContext.Current.Items[LOGS_QUEUE_KEY] != null)
                {
                    Queue<QLogEntry> logEntries = (Queue<QLogEntry>)HttpContext.Current.Items[LOGS_QUEUE_KEY];
                    if (logEntries.Count == 0)
                        return true;
                    else return false;
                }
                else
                    return true;
            }
            else
                return true;
        }

        /// <summary>
        /// Returns information whether static buffer is empty
        /// </summary>
        /// <returns></returns>
        private bool IsStaticBufferEmpty()
        {
            lock (_locker)
            {
                if (_logEntries.Count == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns information whether the static buffer is full and should be flushed apart from the standard Flush() policy
        /// </summary>
        /// <returns></returns>
        private bool IsStaticBufferFull()
        {
            //No need to use lock here since method is being run inside lock already
            if (_logEntries.Count >= _bufferMaxCount)
                return true;
            return false;
        }
    }
}
