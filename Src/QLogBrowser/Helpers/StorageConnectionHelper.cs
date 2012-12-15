using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using QLogBrowser.Models;

namespace QLogBrowser.Helpers
{
    public static class StorageConnectionHelper
    {
        public static string GetConnectionString(StorageConnection connection)
        {
            if (connection.IsDevelopmentStorage)
                return "UseDevelopmentStorage=true";
            else
            {
                string pattern = "DefaultEndpointsProtocol={0};AccountName={1};AccountKey={2}";
                return String.Format(pattern, connection.IsHttps ? "https" : "http", connection.AccountName, connection.AccountKey);
            }
        }

        public static string GetPostfix(QLogBrowserSettings settings)
        {
            StorageConnection currentConnection = settings.Connections.FirstOrDefault(x => x.IsSelected);
            if (!String.IsNullOrWhiteSpace(currentConnection.SourceDataPostfix))
                return currentConnection.SourceDataPostfix.ToLower();
            else
                return "";
        }
    }
}
