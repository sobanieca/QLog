using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLog.Components.Abstract;
using QLog.Models;

namespace QLog.Components.Repository
{
    internal class DebugRepository : IRepository
    {
        public void Save(QLogEntry log)
        {
            Debug.WriteLine("{0} :: {1}", log.Area, log.Message);
        }

        public void SaveAll(List<QLogEntry> logs)
        {
            foreach (var log in logs)
            {
                Debug.WriteLine("{0} :: {1}", log.Area, log.Message);
            }
        }

        public void CleanLogsOlderThan(int noDays)
        {
            return;
        }
    }
}
