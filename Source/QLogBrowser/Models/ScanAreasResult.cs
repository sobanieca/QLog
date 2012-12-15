using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QLogBrowser.Models
{
    public class ScanAreasResult
    {
        public List<QArea> Areas { get; set; }
        public bool ConnectionError { get; set; }
        public Guid TaskId { get; set; }
    }
}
