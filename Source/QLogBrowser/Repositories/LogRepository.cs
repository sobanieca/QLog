using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QLogBrowser.Models;
using System.Threading;
using QLogBrowser.Helpers;
using System.Data.SqlClient;
using System.Windows.Media;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System.Net;

namespace QLogBrowser.Repositories
{
    public class LogRepository
    {
        public void DeleteLogsOlderThan(QLogBrowserSettings settings)
        {
            CloudStorageAccount storageAccount = GetStorageAccount(settings);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            string tablePrefix = GetTablePrefix(settings);
            List<CloudTable> qLogTables = new List<CloudTable>(tableClient.ListTables(tablePrefix));
            string dateLimit = DateTime.UtcNow.AddDays(-1 * settings.DeleteLogsNoDays).ToString("yyyyMMdd");
            foreach (var qLogTable in qLogTables)
            {
                String tableDate = qLogTable.Name.Substring(qLogTable.Name.Length - 8, 8);
                if (String.Compare(tableDate, dateLimit) == -1)
                    qLogTable.DeleteIfExists();
            }
        }

        private string GetTablePrefix(QLogBrowserSettings settings)
        {
            StorageConnection currentConnection = settings.Connections.FirstOrDefault(x => x.IsSelected);
            string tablePrefix = "qlog";
            if (!String.IsNullOrWhiteSpace(currentConnection.SourceDataPostfix))
            {
                tablePrefix = String.Format("qlog{0}", currentConnection.SourceDataPostfix.ToLower());
            }
            return tablePrefix;
        }

        public List<QLog> LoadLogs(QLogBrowserSettings settings)
        {
            CloudStorageAccount storageAccount = GetStorageAccount(settings);
            List<QLog> result = new List<QLog>();
            List<string> partitionsList = GetPartitionsList(settings.DateFrom, settings.DateTo);
            foreach (string partition in partitionsList)
            {
                if (!AppendLogs(storageAccount, partition, result, settings))
                    break;
                if (settings.TaskId != MainWindow.CurrentTaskId)
                    break;
            }

            return result;
        }

        private bool AppendLogs(CloudStorageAccount storageAccount, string partition, List<QLog> result, QLogBrowserSettings settings)
        {
            string[] partitionSplit = partition.Split(',');
            string tableName = partitionSplit[0];
            string tablePartition = partitionSplit[1];

            string postfix = StorageConnectionHelper.GetPostfix(settings);
            if(!String.IsNullOrWhiteSpace(postfix))
                tableName = String.Format("qlog{0}{1}", postfix, tableName);
            else
                tableName = String.Format("qlog{0}", tableName);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableName);
            if (table.Exists())
            {
                TableQuery<QLog> logsQuery = new TableQuery<QLog>();
                string filter = GetFilters(tablePartition, settings);
                logsQuery.Where(filter);

                foreach (QLog log in table.ExecuteQuery(logsQuery))
                {
                    if (!String.IsNullOrWhiteSpace(settings.ContainingText))
                        if(!log.Message.ToLower().Contains(settings.ContainingText.ToLower()))
                            continue;

                    log.CreatedOn = log.CreatedOn.ToLocalTime();
                    result.Add(log);
                    if (result.Count >= settings.Limit)
                        return false;
                }

                return true;
            }
            else
                return true;
        }

        private string GetFilters(string tablePartition, QLogBrowserSettings settings)
        {
            string partitionCondition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, tablePartition);
            string rowCondition = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThan, ToRowKey(settings.DateTo)),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, ToRowKey(settings.DateFrom))
                );
            string result = TableQuery.CombineFilters(partitionCondition, TableOperators.And, rowCondition);

            if (!AllStandardAreasSelected(settings))
                result = TableQuery.CombineFilters(result, TableOperators.And, GetAreasFilter(settings));

            result = GetFilterFor("SessionId", settings.SessionId, result);
            result = GetFilterFor("Class", settings.ClassName, result);
            result = GetFilterFor("Method", settings.MethodName, result);
            result = GetFilterFor("InstanceId", settings.InstanceId, result);
            result = GetFilterFor("DeploymentId", settings.DeploymentId, result);
            result = GetFilterFor("ThreadId", settings.ThreadId, result);
            result = GetFilterFor("UserHost", settings.UserHost, result);
            result = GetFilterFor("UserAgent", settings.UserAgent, result);

            return result;
        }

        private string GetFilterFor(string property, string val, string result)
        {
            if (!String.IsNullOrWhiteSpace(val))
            {
                result = TableQuery.CombineFilters(result, TableOperators.And, TableQuery.GenerateFilterCondition(property, QueryComparisons.Equal, val));
                return result;
            }
            else
                return result;
            
        }

        private string GetAreasFilter(QLogBrowserSettings settings)
        {
            string result = "";
            foreach (QArea area in settings.Areas.Where(x => x.IsSelected).ToList())
            {
                string areaFilter = TableQuery.GenerateFilterCondition("Area", QueryComparisons.Equal, area.Name);
                if (result != "")
                    result = TableQuery.CombineFilters(result, TableOperators.Or, areaFilter);
                else
                    result = areaFilter;
            }
            return result;
        }

        private string ToRowKey(DateTime date)
        {
            return (DateTime.MaxValue - date.ToUniversalTime()).Ticks.ToString("d19");
        }

        private List<string> GetPartitionsList(DateTime dateFrom, DateTime dateTo)
        {
            List<string> result = new List<string>();
            int hoursDiff = (int)(dateTo - dateFrom).TotalHours;
            for (int i = 0; i <= hoursDiff; i++)
            {
                DateTime partitionDate = dateTo.AddHours((-1) * i).ToUniversalTime();
                result.Add(String.Format("{0},{1}", partitionDate.ToString("yyyyMMdd"), partitionDate.ToString("HH")));
            }
            return result;
        }

        private bool AllStandardAreasSelected(QLogBrowserSettings settings)
        {
            if (settings.Areas.Count != 6)
                return false;
            if (settings.Areas.FirstOrDefault(x => x.Name == "QTrace" && x.IsSelected) == null)
                return false;
            if (settings.Areas.FirstOrDefault(x => x.Name == "QDebug" && x.IsSelected) == null)
                return false;
            if (settings.Areas.FirstOrDefault(x => x.Name == "QInfo" && x.IsSelected) == null)
                return false;
            if (settings.Areas.FirstOrDefault(x => x.Name == "QWarn" && x.IsSelected) == null)
                return false;
            if (settings.Areas.FirstOrDefault(x => x.Name == "QError" && x.IsSelected) == null)
                return false;
            if (settings.Areas.FirstOrDefault(x => x.Name == "QCritical" && x.IsSelected) == null)
                return false;

            return true;
        }

        public List<QArea> ScanAreas(QLogBrowserSettings settings)
        {
            StorageConnection selectedConnection = settings.Connections.FirstOrDefault(x => x.IsSelected);
            List<QArea> result = new List<QArea>();

            List<QLog> logs = LoadLogs(settings);
            foreach (var log in logs)
            {
                if (result.FirstOrDefault(x => x.Name == log.Area) == null)
                {
                    QArea area = new QArea();
                    area.IsSelected = true;
                    area.Name = log.Area;
                    Color areaColor = AreaColorHelper.GetColor(log.AreaColor);
                    area.ColorR = areaColor.R;
                    area.ColorG = areaColor.G;
                    area.ColorB = areaColor.B;
                    result.Add(area);
                }
            }
            
            return result;
        }

        private CloudStorageAccount GetStorageAccount(QLogBrowserSettings settings)
        {
            StorageConnection currentConnection = settings.Connections.FirstOrDefault(x => x.IsSelected);
            var result = CloudStorageAccount.Parse(StorageConnectionHelper.GetConnectionString(currentConnection));
            ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(result.TableEndpoint);
            tableServicePoint.UseNagleAlgorithm = false;
            tableServicePoint.Expect100Continue = false;
            return result;
        }
    }
}
