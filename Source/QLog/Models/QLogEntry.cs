using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace QLog.Models
{
    /// <summary>
    /// Model representing single QLog entry.
    /// </summary>
    public class QLogEntry : TableEntity
    {
        public Guid Guid { get; set; }
        public string Message { get; set; }
        public string Area { get; set; }
        public string AreaColor { get; set; }
        public string ThreadId { get; set; }
        public string SessionId { get; set; }
        public string UserAgent { get; set; }
        public string UserHost { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public string InstanceId { get; set; }
        public string DeploymentId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
