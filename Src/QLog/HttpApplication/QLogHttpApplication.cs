using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QLog.HttpApplication
{
    /// <summary>
    /// HttpApplication that sets up handlers for all
    /// events that are connected with QLog mechanisms.
    /// </summary>
    public class QLogHttpApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Constructor that sets up all event handlers
        /// </summary>
        public QLogHttpApplication()
        {
            this.EndRequest += new EventHandler(QLogHttpApplication_EndRequest);
            this.Error += new EventHandler(QLogHttpApplication_Error);
            this.Disposed += new EventHandler(QLogHttpApplication_Disposed);
        }

        void QLogHttpApplication_Disposed(object sender, EventArgs e)
        {
            try
            {
                QLog.Logger.Flush(false);
            }
            catch (Exception)
            {
                //No need to raise any exception here or perform any actions.
            }
        }

        void QLogHttpApplication_Error(object sender, EventArgs e)
        {
            if (HttpContext.Current != null)
            {
                Exception exception = HttpContext.Current.Server.GetLastError();
                QLog.Logger.LogError(exception);
            }
        }

        void QLogHttpApplication_EndRequest(object sender, EventArgs e)
        {
            QLog.Logger.Flush();
        }
    }
}
