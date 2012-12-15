using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QLogBrowser.Models
{
    public class WorkerTask
    {
        public Guid TaskId { get; set; }
        public BackgroundWorker Worker { get; set; }
    }
}
