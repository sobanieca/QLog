using QLog.Areas.Base;

namespace QLog.Areas.Default
{
    /// <summary>
    /// Represents the QTrace logs area.
    /// </summary>
    public sealed class QTrace : QArea
    {
        public override QAreaColor AreaColor
        {
            get
            {
                return new QAreaColor(147, 188, 255);
            }
        }

        public override uint Severity
        {
            get
            {
                return 0;
            }
        }
    }
}
