using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QLogBrowser.Models
{
    public class DeleteLogsResult
    {
        public bool ConnectionError { get; set; }
        public Guid TaskId { get; set; }
    }
}
