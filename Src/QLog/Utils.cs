using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QLog.Exceptions;
using QLog.Areas.Base;
using QLog.Areas.Default;
using QLog.Components;

namespace QLog
{
    /// <summary>
    /// Main tool that offers interface for operation on logs
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Cleans the logs older than given number of days.
        /// </summary>
        /// <param name="noDays"></param>
        public static void CleanLogsOlderThan(int noDays)
        {
            try
            {
                ComponentsService.Repository.CleanLogsOlderThan(noDays);
            }
            catch (Exception e)
            {
                ComponentsService.SilentModeHandle(e);
            }
        }
    }
}
