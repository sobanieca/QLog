using System.Collections.Generic;
using QLog.Models;

namespace QLog.Components.Abstract
{
    /// <summary>
    /// Interface for service buffer. Buffer needs to be thread safe.
    /// </summary>
    public interface IBuffer
    {
        /// <summary>
        /// Enqueues log inside buffer. Returns information if buffer is full and needs to be flushed.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="isBufferFull"></param>
        void Enqueue(QLogEntry log, out bool isBufferFull);
        /// <summary>
        /// Returns list of logs stored inside buffer.
        /// </summary>
        /// <returns></returns>
        List<QLogEntry> DequeueLogEntries();
        /// <summary>
        /// Returns true if buffer is empty.
        /// </summary>
        /// <returns></returns>
        bool IsEmpty();
    }
}
