﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using QLog.Components.Abstract;
using QLog.Helpers;

namespace QLog.Components.Config
{
    /// <summary>
    /// Base configuration component that is being used by QLog forks.
    /// </summary>
    internal class QLogConfig : IConfig
    {
        //Configuration keys definitions:
        private const string QLOG_AREA_KEY = "QLogArea";
        private const string QLOG_STACKTRACE_KEY = "QLogStacktrace";
        private const string QLOG_ASYNC_KEY = "QLogAsync";
        private const string QLOG_SILENT_MODE_KEY = "QLogSilentMode";
        private const string QLOG_POSTFIX_KEY = "QLogPostfix";
        protected const string DATA_SOURCE_KEY = "QLogDataSource";

        //Default values definitons:
        private const bool QLOG_AREA_DEFAULT = false;
        private const bool QLOG_STACKTRACE_DEFAULT = true;
        private const bool QLOG_ASYNC_DEFAULT = false;
        private const bool QLOG_SILENT_MODE_DEFAULT = true;
        private const string QLOG_POSTFIX_DEFAULT = "";

        /// <summary>
        /// Reads the current setting basing on the available environment tools
        /// </summary>
        /// <returns></returns>
        private string GetSetting(string key)
        {
            string result = null;
            if (!TryReadFromAzureConfig(out result, key))
                result = ConfigurationManager.AppSettings[key];
            return result;
        }

        /// <summary>
        /// Tries to read specified configuration setting from the Azure service configuration
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool TryReadFromAzureConfig(out string value, string key)
        {
            value = null;
            if (RoleEnvironment.IsAvailable)
            {
                try
                {
                    value = RoleEnvironment.GetConfigurationSettingValue(key);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Validates current log area against the area policy specified in the configuration file
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public bool IsValidLogArea(Type area)
        {
            string cfgArea = GetSetting(QLOG_AREA_KEY);
            if (String.IsNullOrWhiteSpace(cfgArea))
                return QLOG_AREA_DEFAULT;
            cfgArea = cfgArea.Trim().ToLower();
            if (cfgArea == "none")
                return false;
            if (cfgArea == "all")
                return true;
            if (cfgArea.StartsWith("accept:"))
                return ValidateAreaFilter(cfgArea, area, true);
            if (cfgArea.StartsWith("ignore:"))
                return ValidateAreaFilter(cfgArea, area, false);
            return ValidateLogArea(cfgArea, area);
        }

        /// <summary>
        /// Specifies whether stacktrace usage is enabled
        /// </summary>
        /// <returns></returns>
        public bool IsStacktraceEnabled()
        {
            string stacktrace = GetSetting(QLOG_STACKTRACE_KEY);
            if (String.IsNullOrWhiteSpace(stacktrace))
                return QLOG_STACKTRACE_DEFAULT;
            if (stacktrace.ToLower() == "true")
                return true;
            if (stacktrace.ToLower() == "false")
                return false;
            return false;
        }

        /// <summary>
        /// Specifies whether asynchronous log writing for given log area is enabled. If it is enabled, then all log entries will be 
        /// stored in the buffer and later flushed to the database. Otherwise all messages will be logged directly to the database
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public bool IsAsyncLogEnabled(Type area)
        {
            string async = GetSetting(QLOG_ASYNC_KEY);
            if (String.IsNullOrWhiteSpace(async))
                return QLOG_ASYNC_DEFAULT;
            async = async.Trim().ToLower();
            if (async == "true")
                return true;
            if (async == "false")
                return false;
            if (async.StartsWith("accept:"))
                return ValidateAreaFilter(async, area, true);
            if (async.StartsWith("ignore:"))
                return ValidateAreaFilter(async, area, false);
            return QLOG_ASYNC_DEFAULT;
        }

        /// <summary>
        /// Specifies whether asynchronous log writing for any log area is enabled. If it is enabled, then all log entries will be 
        /// stored in the buffer and later flushed to the database. Otherwise all messages will be logged directly to the database
        /// </summary>
        /// <returns></returns>
        public bool IsAsyncLogEnabled()
        {
            string async = GetSetting(QLOG_ASYNC_KEY);
            if (String.IsNullOrWhiteSpace(async))
                return QLOG_ASYNC_DEFAULT;
            async = async.ToLower();
            if (async == "false")
                return false;
            //If async is not disabled explicitly logger will assume that user wanted to turn it on for some areas
            return true;
        }

        /// <summary>
        /// Specifies whether silent mode is enabled or explicitly disabled
        /// </summary>
        /// <returns></returns>
        public bool IsSilentModeEnabled()
        {
            string silentMode = GetSetting(QLOG_SILENT_MODE_KEY);
            if (String.IsNullOrWhiteSpace(silentMode))
                return QLOG_STACKTRACE_DEFAULT;
            if (silentMode.ToLower() == "true")
                return true;
            if (silentMode.ToLower() == "false")
                return false;
            return false;
        }

        /// <summary>
        /// Returns the QLog postfix that will be used when saving logs in data source. For instance it will be added to table name.
        /// </summary>
        /// <returns></returns>
        public string GetDataSourcePostfix()
        {
            string tableName = GetSetting(QLOG_POSTFIX_KEY);
            if (String.IsNullOrWhiteSpace(tableName))
                return QLOG_POSTFIX_DEFAULT;
            else
                return tableName;
        }

        /// <summary>
        /// Validates the specified area against against the current settings of severity level
        /// </summary>
        /// <param name="strCfgArea"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        private bool ValidateLogArea(string strCfgArea, Type area)
        {
            uint severity = AreaHelper.GetAreaSeverity(area);
            uint cfgSeverity = AreaHelper.GetAreaSeverity(strCfgArea);
            if (severity >= cfgSeverity)
                return true;
            else
                return false;
        }

        /// <summary>
        /// In case when user specifies directly which areas he wants to be logged, or
        /// which areas should be ignored this function validates the given area against such policy
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="area"></param>
        /// <param name="ifFoundReturn"></param>
        /// <returns></returns>
        private bool ValidateAreaFilter(string filter, Type area, bool ifFoundReturn)
        {
            filter = filter.Substring(7);
            filter = filter.Trim();
            string areaName = area.Name.ToLower();
            string[] cfgAreas = filter.Split(',');
            foreach (var cfgArea in cfgAreas)
            {
                if (cfgArea.Trim() == areaName)
                    return ifFoundReturn;
            }
            return !ifFoundReturn;
        }

        /// <summary>
        /// Returns data source - for instance connection string.
        /// </summary>
        /// <returns></returns>
        public string GetDataSource()
        {
            return ConfigurationManager.ConnectionStrings[DATA_SOURCE_KEY].ConnectionString;
        }
    }
}
