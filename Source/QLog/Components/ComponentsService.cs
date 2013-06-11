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

        public static IEnvironment Environment { get { DetermineInit(); return _environment; } set { _environment = value; } }
        public static IBuffer Buffer { get { DetermineInit(); return _buffer; } set { _buffer = value; } }
        public static IConfig Config { get { DetermineInit(); return _config; } set { _config = value; } }
        public static IRepository Repository { get { DetermineInit(); return _repository; } set { _repository = value; } }

        private static bool _isInitialized = false;
        private static object _locker = new object();

        /// <summary>
        /// Initializes QLog with components found within assembly
        /// </summary>
        private static void InitializeComponents()
        {
            try
            {
                _environment = new QLogEnvironment();
                _buffer = new QLogBuffer();
                _config = new QLogConfig();
                _repository = ResolveRepository();
            }
            catch (Exception e)
            {
                SilentModeHandle(e);
            }
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
            bool isSilentModeEnabled = true;
            if (_config != null)
            {
                try
                {
                    bool configSilentMode = _config.IsSilentModeEnabled();
                    isSilentModeEnabled = configSilentMode;
                }
                catch (Exception)
                { } //QLog is not going to raise any exception if it won't be able to determine if SilentMode is ON or OFF
            }
            if (!isSilentModeEnabled)
                throw exceptionToHandle;
        }

        /// <summary>
        /// Initializes ComponentService with component in case when there is need for this
        /// </summary>
        private static void DetermineInit()
        {
            if (!_isInitialized)
            {
                lock (_locker)
                {
                    if (!_isInitialized)
                    {
                        _isInitialized = true;
                        InitializeComponents();
                    }
                }
            }
        }
    }
}
