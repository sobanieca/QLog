using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.ServiceRuntime;
using QLog.Components.Abstract;
using QLog.Helpers;
using QLog.Models;

namespace QLog.Components.Environment
{
    /// <summary>
    /// Base log component that is being used by QLog forks.
    /// </summary>
    internal class QLogEnvironment : IEnvironment
    {
        private Random _rand = new Random();

        /// <summary>
        /// Returns QLog entry basing on the passed informations
        /// </summary>
        /// <param name="area"></param>
        /// <param name="msg"></param>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public QLogEntry GetLog(Type area, string msg, string className, string methodName)
        {
            QLogEntry result = new QLogEntry();
            result.Guid = Guid.NewGuid();
            result.Message = msg;
            result.Area = area.Name;
            result.AreaColor = AreaHelper.GetAreaColor(area);
            result.ThreadId = Thread.CurrentThread.ManagedThreadId.ToString();
            result.SessionId = GetSessionId();
            result.UserAgent = GetUserAgent();
            result.UserHost = GetUserHost();
            result.Class = className;
            result.Method = methodName;
            result.InstanceId = GetInstanceId();
            result.DeploymentId = GetDeploymentId();
            result.CreatedOn = DateTime.UtcNow;
            TruncateHelper.Truncate(result);
            return result;
        }

        private string GetDeploymentId()
        {
            if (RoleEnvironment.IsAvailable)
                return RoleEnvironment.DeploymentId ?? "";
            return "";
        }

        private string GetInstanceId()
        {
            if (RoleEnvironment.IsAvailable)
            {
                if (RoleEnvironment.CurrentRoleInstance != null)
                {
                    return RoleEnvironment.CurrentRoleInstance.Id ?? "";
                }
            }
            return "";
        }

        private string GetSessionId()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session != null)
                    return HttpContext.Current.Session.SessionID ?? "";
                else
                    return "";
            }
            else
            {
                try
                {
                    return String.Format("{0}\\{1}", System.Environment.UserDomainName, System.Environment.UserName);
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        private string GetUserAgent()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Request != null)
                    return HttpContext.Current.Request.UserAgent ?? "";
                else
                    return "";
            }
            else
            {
                try
                {
                    return System.Environment.OSVersion.ToString();
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        private string GetUserHost()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Request != null)
                    return HttpContext.Current.Request.UserHostAddress ?? "";
                else
                    return "";
            }
            else
            {
                try
                {
                    return System.Environment.MachineName;
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
    }
}
