using QLog.Areas.Base;

namespace QLog.Areas.Default
{
    /// <summary>
    /// Represents the QError logs area.
    /// </summary>
    public sealed class QError : QArea
    {
        public override QAreaColor AreaColor
        {
            get
            {
                return new QAreaColor(219, 36, 36);
            }
        }

        public override uint Severity
        {
            get
            {
                return 5000;
            }
        }
    }
}
