using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QLogBrowser.Models
{
    [Serializable]
    public class QLogBrowserSettings
    {
        public List<StorageConnection> Connections { get; set; }
        public List<QArea> Areas { get; set; }
        public int Limit { get; set; }
        public string ContainingText { get; set; }
        public string SessionId { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string InstanceId { get; set; }
        public string DeploymentId { get; set; }
        public string ThreadId { get; set; }
        public string UserHost { get; set; }
        public string UserAgent { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public Guid TaskId { get; set; }
        public int DeleteLogsNoDays { get; set; }
    }
}
