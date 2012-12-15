using System;
using QLog.Models;
using QLog.Areas.Base;
using System.Collections.Generic;

namespace QLog.Components.Abstract
{
    /// <summary>
    /// Interface for objects that are responsible for all operations on the logs storage
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Saves single log entry in data source.
        /// </summary>
        /// <param name="log"></param>
        void Save(QLogEntry log);
        /// <summary>
        /// Saves list of entries in data source.
        /// </summary>
        /// <param name="logs"></param>
        void SaveAll(List<QLogEntry> logs);
        /// <summary>
        /// Cleans the log entries older than specified number of days from the database
        /// </summary>
        /// <param name="noDays"></param>
        void CleanLogsOlderThan(int noDays);
    }
}
