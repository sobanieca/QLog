using System;
using System.Collections.Generic;
using QLog.Areas.Base;
using QLog.Areas.Default;

namespace QLog.Helpers
{
    /// <summary>
    /// This helper is responsible for obtaining the area information. It is used to prevent using reflection if only it is possible.
    /// </summary>
    internal static class AreaHelper
    {
        //Internal buffer for area colors to avoid applying repeatedly Activator.CreateInstance() instructions for the same area:
        private static Dictionary<string, string> _areaColors = new Dictionary<string, string>();
        //Lock for areas colors buffer:
        private static object _areaColorsLock = new object();
        //Internal buffer for area severity to avoid applying repeatedly Activator.CreateInstance() instructions for the same area:
        private static Dictionary<string, uint> _areaSeverities = new Dictionary<string, uint>();
        //Lock for areas severities buffer:
        private static object _areaSeveritiesLock = new object();

        //Predefined areas colors:
        private const string QTRACE_COLOR = "147,188,255";
        private const string QDEBUG_COLOR = "77,82,255";
        private const string QINFO_COLOR = "10,152,0";
        private const string QWARN_COLOR = "206,129,0";
        private const string QERROR_COLOR = "219,36,36";
        private const string QCRITICAL_COLOR = "255,0,0";

        //Predefined areas severities:
        private const uint QTRACE_SEVERITY = 0;
        private const uint QDEBUG_SEVERITY = 1000;
        private const uint QINFO_SEVERITY = 3000;
        private const uint QWARN_SEVERITY = 4000;
        private const uint QERROR_SEVERITY = 5000;
        private const uint QCRITICAL_SEVERITY = 6000;

        internal static string GetAreaColor(Type area)
        {
            if (area == typeof(QTrace))
                return QTRACE_COLOR;
            if (area == typeof(QDebug))
                return QDEBUG_COLOR;
            if (area == typeof(QInfo))
                return QINFO_COLOR;
            if (area == typeof(QWarn))
                return QWARN_COLOR;
            if (area == typeof(QError))
                return QERROR_COLOR;
            if (area == typeof(QCritical))
                return QCRITICAL_COLOR;

            lock (_areaColorsLock)
            {
                if (_areaColors.ContainsKey(area.Name))
                {
                    return _areaColors[area.Name];
                }
                else
                {
                    QArea areaInstance = (QArea)Activator.CreateInstance(area);
                    _areaColors.Add(area.Name, areaInstance.AreaColor.ToString());
                    return _areaColors[area.Name];
                }
            }
        }

        internal static uint GetAreaSeverity(Type area)
        {
            if (area == typeof(QTrace))
                return QTRACE_SEVERITY;
            if (area == typeof(QDebug))
                return QDEBUG_SEVERITY;
            if (area == typeof(QInfo))
                return QINFO_SEVERITY;
            if (area == typeof(QWarn))
                return QWARN_SEVERITY;
            if (area == typeof(QError))
                return QERROR_SEVERITY;
            if (area == typeof(QCritical))
                return QCRITICAL_SEVERITY;

            lock (_areaSeveritiesLock)
            {
                if (_areaSeverities.ContainsKey(area.Name))
                {
                    return _areaSeverities[area.Name];
                }
                else
                {
                    QArea areaInstance = (QArea)Activator.CreateInstance(area);
                    _areaSeverities.Add(area.Name, areaInstance.Severity);
                    return _areaSeverities[area.Name];
                }
            }
        }

        internal static uint GetAreaSeverity(string areaName)
        {
            switch (areaName)
            {
                case "qcritical":
                    return QCRITICAL_SEVERITY;
                case "qerror":
                    return QERROR_SEVERITY;
                case "qwarn":
                    return QWARN_SEVERITY;
                case "qinfo":
                    return QINFO_SEVERITY;
                case "qdebug":
                    return QDEBUG_SEVERITY;
                case "qtrace":
                    return QTRACE_SEVERITY;
                default:
                    return UInt32.MaxValue;
            }
        }
    }
}
