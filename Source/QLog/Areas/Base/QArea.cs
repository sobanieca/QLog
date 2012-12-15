
namespace QLog.Areas.Base
{
    /// <summary>
    /// Abstract class that is used as a base class for a new user defined areas.
    /// </summary>
    public abstract class QArea
    {
        /// <summary>
        /// Rgb color in format: "R,G,B"
        /// </summary>
        public virtual QAreaColor AreaColor { get { return new QAreaColor(0, 0, 0); } }
        /// <summary>
        /// Severity value for a given area
        /// </summary>
        public virtual uint Severity { get { return 2000; } }
    }
}
