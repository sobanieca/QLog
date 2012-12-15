using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QLogBrowser.Models;
using QLogBrowser.Repositories;
using System.Threading;

namespace QLogBrowser.Services
{
    public class LogService
    {
        private LogRepository _logRepository;

        public LogService()
        {
            _logRepository = new LogRepository();
        }

        public DeleteLogsResult DeleteLogsOlderThan(QLogBrowserSettings settings)
        {
            DeleteLogsResult result = new DeleteLogsResult();
            result.TaskId = settings.TaskId;
            try
            {
                _logRepository.DeleteLogsOlderThan(settings);
            }
            catch (Exception)
            {
                result.ConnectionError = true;
            }
            return result;
        }

        public LoadLogsResult LoadLogs(QLogBrowserSettings settings)
        {
            LoadLogsResult result = new LoadLogsResult();
            result.ConnectionError = false;
            result.TaskId = settings.TaskId;
            result.Logs = new List<QLog>();
            result.AreasCount = new Dictionary<string, int>();
            try
            {
                result.Logs = _logRepository.LoadLogs(settings);
                result.TotalLogsFound = result.Logs.Count;
                for (int i = 0; i < result.Logs.Count; i++)
                {
                    string area = result.Logs[i].Area;
                    if (result.AreasCount.ContainsKey(area))
                    {
                        result.AreasCount[area]++;
                    }
                    else
                    {
                        result.AreasCount.Add(area, 1);
                    }
                }
            }
            catch (Exception)
            {
                result.ConnectionError = true;
            }
            return result;
        }

        public ScanAreasResult ScanAreas(QLogBrowserSettings settings)
        {
            ScanAreasResult result = new ScanAreasResult();
            result.TaskId = settings.TaskId;
            try
            {
                result.Areas = _logRepository.ScanAreas(settings);
            }
            catch (Exception)
            {
                result.ConnectionError = true;
            }
            return result;
        }
    }
}
