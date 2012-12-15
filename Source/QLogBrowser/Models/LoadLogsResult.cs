using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QLogBrowser.Models;

namespace QLogBrowser.Models
{
    public class LoadLogsResult
    {
        public List<QLog> Logs { get; set; }
        public long TotalLogsFound { get; set; }
        public Dictionary<string, int> AreasCount { get; set; }
        public bool ConnectionError { get; set; }
        public Guid TaskId { get; set; }
    }
}
