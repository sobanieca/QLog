using QLog.Areas.Base;

namespace QLog.Areas.Default
{
    /// <summary>
    /// Represents the QWarn logs area.
    /// </summary>
    public sealed class QWarn : QArea
    {
        public override QAreaColor AreaColor
        {
            get
            {
                return new QAreaColor(206, 129, 0);
            }
        }

        public override uint Severity
        {
            get
            {
                return 4000;
            }
        }
    }
}
