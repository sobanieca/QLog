using QLog.Areas.Base;

namespace QLog.Areas.Default
{
    /// <summary>
    /// Represents the QInfo logs area.
    /// </summary>
    public sealed class QInfo : QArea
    {
        public override QAreaColor AreaColor
        {
            get
            {
                return new QAreaColor(10, 152, 0);
            }
        }

        public override uint Severity
        {
            get
            {
                return 3000;
            }
        }
    }
}
