using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using QLog.Components.Abstract;
using QLog.Models;

namespace QLog.Components.Repository
{
    internal class AzureTableRepository : IRepository
    {
        private const string DEFAULT_TABLE_NAME = "qlog{0}";
        private const string POSTFIX_TABLE_NAME = "qlog{0}{1}";

        /// <summary>
        /// Saves single log entry in data source.
        /// </summary>
        /// <param name="log"></param>
        public void Save(QLogEntry log)
        {
            CloudStorageAccount storageAccount = GetStorageAccount(); 
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            string tableName = GetTableName(log);
            CloudTable table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();
            log.PartitionKey = log.CreatedOn.ToString("HH");
            log.RowKey = (DateTime.MaxValue - log.CreatedOn).Ticks.ToString("d19");
            table.Execute(TableOperation.Insert(log));
        }

        /// <summary>
        /// Saves list of entries in data source.
        /// </summary>
        /// <param name="logs"></param>
        public void SaveAll(List<QLogEntry> logs)
        {
            CloudStorageAccount storageAccount = GetStorageAccount();
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            Dictionary<string, List<QLogEntry>> tablesMapping = new Dictionary<string, List<QLogEntry>>();

            foreach (QLogEntry log in logs)
            {
                string tableName = GetTableName(log);
                if (tablesMapping.ContainsKey(tableName))
                {
                    tablesMapping[tableName].Add(log);
                }
                else
                {
                    List<QLogEntry> tableLogs = new List<QLogEntry>();
                    tableLogs.Add(log);
                    tablesMapping.Add(tableName, tableLogs);
                }
            }

            foreach (string key in tablesMapping.Keys)
            {
                CloudTable table = tableClient.GetTableReference(key);
                table.CreateIfNotExists();

                TableBatchOperation batchOperation = new TableBatchOperation();

                foreach (var log in tablesMapping[key])
                {
                    log.PartitionKey = log.CreatedOn.ToString("HH");
                    log.RowKey = (DateTime.MaxValue - log.CreatedOn).Ticks.ToString("d19");
                    batchOperation.Insert(log);
                }

                table.ExecuteBatch(batchOperation);
            }
        }

        /// <summary>
        /// Cleans the log entries older than specified number of days from the database
        /// </summary>
        /// <param name="noDays"></param>
        public void CleanLogsOlderThan(int noDays)
        {
            CloudStorageAccount storageAccount = GetStorageAccount();
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            List<CloudTable> qLogTables = new List<CloudTable>(tableClient.ListTables("qlog"));
            string dateLimit = DateTime.UtcNow.AddDays(-1 * noDays).ToString("yyyyMMdd");
            foreach (var qLogTable in qLogTables)
            {
                String tableDate = qLogTable.Name.Substring(qLogTable.Name.Length - 8, 8);
                if (String.Compare(tableDate, dateLimit) == -1)
                    qLogTable.DeleteIfExists();
            }
        }

        private string GetTableName(QLogEntry log)
        {
            string tableName = String.Format(DEFAULT_TABLE_NAME, log.CreatedOn.ToString("yyyyMMdd"));
            if (!String.IsNullOrWhiteSpace(ComponentsService.Config.GetDataSourcePostfix()))
            {
                tableName = String.Format(POSTFIX_TABLE_NAME, ComponentsService.Config.GetDataSourcePostfix().ToLower(), log.CreatedOn.ToString("yyyyMMdd"));
            }
            return tableName;
        }

        private CloudStorageAccount GetStorageAccount()
        {
            var result = CloudStorageAccount.Parse(ComponentsService.Config.GetDataSource());
            ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(result.TableEndpoint);
            tableServicePoint.UseNagleAlgorithm = false;
            tableServicePoint.Expect100Continue = false;
            return result;
        }
    }
}
