using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QLogBrowser.Models
{
    [Serializable]
    public class StorageConnection
    {
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public string SourceDataPostfix { get; set; }
        public bool IsDevelopmentStorage { get; set; }
        public bool IsHttps { get; set; }
        public bool IsSelected { get; set; }
    }
}
