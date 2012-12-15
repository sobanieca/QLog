using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using QLogBrowser.Models;

namespace QLogBrowser.Helpers
{
    public class AreaColorHelper
    {
        public static Color GetColor(string strColor)
        {
            Color c = new Color();
            string[] colors = strColor.Split(',');
            c.A = 255;
            c.R = Byte.Parse(colors[0]);
            c.G = Byte.Parse(colors[1]);
            c.B = Byte.Parse(colors[2]);
            return c;
        }
    }
}
