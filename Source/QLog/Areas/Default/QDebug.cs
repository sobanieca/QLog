using QLog.Areas.Base;

namespace QLog.Areas.Default
{
    /// <summary>
    /// Represents the QDebug logs area.
    /// </summary>
    public sealed class QDebug : QArea
    {
        public override QAreaColor AreaColor
        {
            get
            {
                return new QAreaColor(77, 82, 255);
            }
        }

        public override uint Severity
        {
            get
            {
                return 1000;
            }
        }
    }
}
