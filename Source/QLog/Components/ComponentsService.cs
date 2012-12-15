using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using QLog.Exceptions;
using QLog.Components.Abstract;
using QLog.Components.Environment;
using QLog.Components.Buffer;
using QLog.Components.Config;
using QLog.Components.Repository;

namespace QLog.Components
{
    internal static class ComponentsService
    {
        private static IEnvironment _environment;
        private static IBuffer _buffer;
        private static IConfig _config;
        private static IRepository _repository;

        public const string SILENT_MODE_HANDLE_ERROR_MESSAGE = "QLog was unable to verify whether SilentMode is enabled or disabled. There appears to be a problem with IConfig component.";

        public static IEnvironment Environment { get { return _environment; } set { _environment = value; } }
        public static IBuffer Buffer { get { return _buffer; } set { _buffer = value; } }
        public static IConfig Config { get { return _config; } set { _config = value; } }
        public static IRepository Repository { get { return _repository; } set { _repository = value; } }

        static ComponentsService()
        {
            InitializeComponents();
        }

        /// <summary>
        /// Initializes QLog with components found within assembly
        /// </summary>
        private static void InitializeComponents()
        {
            _environment = new QLogEnvironment();
            _buffer = new QLogBuffer();
            _config = new QLogConfig();
            _repository = ResolveRepository();
        }

        /// <summary>
        /// Resolves correct repository basing on the data source info
        /// </summary>
        /// <returns></returns>
        private static IRepository ResolveRepository()
        {
            string dataSource = _config.GetDataSource();
            if(!String.IsNullOrWhiteSpace(dataSource))
            {
                dataSource = dataSource.ToLower().Trim();
                if (dataSource == "debug")
                    return new DebugRepository();
            }
            return new AzureTableRepository();
        }

        /// <summary>
        /// Handles an exception occured during logging. If silent mode is disabled an exception will be thrown further.
        /// </summary>
        /// <param name="exceptionToHandle"></param>
        public static void SilentModeHandle(Exception exceptionToHandle)
        {
            bool isSilentModeEnabled = false;
            try
            {
                isSilentModeEnabled = _config.IsSilentModeEnabled();
            }
            catch (Exception e)
            {
                throw new QLogSilentModeHandleException(SILENT_MODE_HANDLE_ERROR_MESSAGE, e);
            }
            if (!isSilentModeEnabled)
                throw exceptionToHandle;
        }
    }
}
