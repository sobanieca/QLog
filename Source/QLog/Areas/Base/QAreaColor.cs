using System;

namespace QLog.Areas.Base
{
    /// <summary>
    /// Class that is used to store the color of an area.
    /// </summary>
    public sealed class QAreaColor
    {
        private byte _r;
        private byte _g;
        private byte _b;

        public QAreaColor(byte r, byte g, byte b)
        {
            _r = r;
            _g = g;
            _b = b;
        }

        public override string ToString()
        {
            return String.Format("{0},{1},{2}", _r, _g, _b);
        }
    }
}
