using QLog.Areas.Base;

namespace QLog.Areas.Default
{
    /// <summary>
    /// Represents the QCritical logs area.
    /// </summary>
    public sealed class QCritical : QArea
    {
        public override QAreaColor AreaColor
        {
            get
            {
                return new QAreaColor(255, 0, 0);
            }
        }

        public override uint Severity
        {
            get
            {
                return 6000;
            }
        }
    }
}
