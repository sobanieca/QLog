using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using QLog.Components.Abstract;
using QLog.Exceptions;
using QLog.Models;

namespace QLog.Components.Repository
{
    internal class AzureTableRepository : IRepository
    {
        private const string DEFAULT_TABLE_NAME = "qlog{0}";
        private const string POSTFIX_TABLE_NAME = "qlog{0}{1}";

        private const int BATCH_INSERT_LIMIT = 100;
        public const string DATA_SOURCE_POSTFIX_ERROR_MESSAGE = "QLog data source postfix can contain only lowercase alpha numeric characters of length at most 40.";

        private Exception _validationException = null;

        public AzureTableRepository()
        {
            //Validating data source postfix if it contains chars valid for Azure Table name
            string postfix = ComponentsService.Config.GetDataSourcePostfix();
            if (!String.IsNullOrWhiteSpace(postfix))
            {
                postfix = postfix.ToLower();
                string validChars = "abcdefghijklmnopqrstuwvxyz0123456789";
                foreach (var c in postfix)
                {
                    if (validChars.IndexOf(c) == -1)
                    {
                        _validationException =  new QLogDataSourcePostfixException(DATA_SOURCE_POSTFIX_ERROR_MESSAGE);
                    }
                }
            }
        }

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
            log.RowKey = String.Format("{0}{1}", (DateTime.MaxValue - log.CreatedOn).Ticks.ToString("d19"), log.Guid.ToString("N"));
            log.Message = log.Message.Trim();
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

            List<AzureTableMapping> tablesMapping = new List<AzureTableMapping>();

            foreach (var log in logs)
            {
                string tableName = GetTableName(log);
                AzureTableMapping tableMapping = tablesMapping.FirstOrDefault(x => x.TableName == tableName);
                if (tableMapping == null)
                {
                    tableMapping = new AzureTableMapping() { TableName = tableName };
                    tablesMapping.Add(tableMapping);
                }
                string partitionKey = log.CreatedOn.ToString("HH");
                PartitionMapping partitionMapping = tableMapping.PartitionMappings.FirstOrDefault(x => x.PartitionKey == partitionKey);
                if (partitionMapping == null)
                {
                    partitionMapping = new PartitionMapping() { PartitionKey = partitionKey };
                    tableMapping.PartitionMappings.Add(partitionMapping);
                }
                partitionMapping.Logs.Add(log);
            }

            foreach (var tableMapping in tablesMapping)
            {
                CloudTable table = tableClient.GetTableReference(tableMapping.TableName);
                table.CreateIfNotExists();
                foreach (var partitionMapping in tableMapping.PartitionMappings)
                {
                    List<QLogEntry> partitionLogs = partitionMapping.Logs;
                    //For now (23.03.2013) single batch operation may consist of at most 100 entities 
                    //so there is need to perform "paging" in case when there are more than 100 logs for single partition
                    int noPages = (int)Math.Ceiling((double)partitionLogs.Count / (double)BATCH_INSERT_LIMIT);
                    for (int i = 0; i < noPages; i++)
                    {
                        var batchLogs = partitionLogs.Skip(BATCH_INSERT_LIMIT * i).Take(BATCH_INSERT_LIMIT);
                        TableBatchOperation batchOperation = new TableBatchOperation();
                        foreach (var log in batchLogs)
                        {
                            log.PartitionKey = partitionMapping.PartitionKey;
                            log.RowKey = String.Format("{0}{1}", (DateTime.MaxValue - log.CreatedOn).Ticks.ToString("d19"), log.Guid.ToString("N"));
                            log.Message = log.Message.Trim();
                            batchOperation.Insert(log);
                        }
                        table.ExecuteBatch(batchOperation);
                    }
                }
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
            string tablePrefix = "qlog";
            if (!String.IsNullOrWhiteSpace(ComponentsService.Config.GetDataSourcePostfix()))
            {
                tablePrefix = String.Format("qlog{0}", ComponentsService.Config.GetDataSourcePostfix().ToLower());
            }
            List<CloudTable> qLogTables = new List<CloudTable>(tableClient.ListTables(tablePrefix));
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
            if (_validationException != null)
            {
                Exception e = _validationException;
                _validationException = null;
                throw e;
            }
            var result = CloudStorageAccount.Parse(ComponentsService.Config.GetDataSource());
            ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(result.TableEndpoint);
            tableServicePoint.UseNagleAlgorithm = false;
            tableServicePoint.Expect100Continue = false;
            return result;
        }

        private class AzureTableMapping
        {
            public string TableName { get; set; }
            public List<PartitionMapping> PartitionMappings { get; set; }

            public AzureTableMapping()
            {
                PartitionMappings = new List<PartitionMapping>();
            }
        }

        private class PartitionMapping
        {
            public string PartitionKey { get; set; }
            public List<QLogEntry> Logs { get; set; }

            public PartitionMapping()
            {
                Logs = new List<QLogEntry>();
            }
        }

    }
}
