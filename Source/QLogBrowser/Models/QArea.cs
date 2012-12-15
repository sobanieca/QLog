using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace QLogBrowser.Models
{
    [Serializable]
    public class QArea
    {
        public string Name { get; set; }
        public byte ColorR { get; set; }
        public byte ColorG { get; set; }
        public byte ColorB { get; set; }
        public bool IsSelected { get; set; }
    }
}
