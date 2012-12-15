using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Microsoft.WindowsAzure.Storage.Table;

namespace QLogBrowser.Models
{
    public class QLog : TableEntity
    {
        public long Id { get; set; }
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
