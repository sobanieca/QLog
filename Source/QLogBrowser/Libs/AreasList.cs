using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QLogBrowser.Models;

namespace QLogBrowser.Libs
{
    public class AreasList
    {
        private const string QTRACE = "QTrace";
        private const string QDEBUG = "QDebug";
        private const string QINFO = "QInfo";
        private const string QWARN = "QWarn";
        private const string QERROR = "QError";
        private const string QCRITICAL = "QCritical";

        private List<QArea> _areas;

        public AreasList(List<QArea> areas)
        {
            _areas = new List<QArea>();
            foreach (QArea area in areas)
            {
                _areas.Add(area);
            }
        }

        public List<QArea> GetOrderedAreas()
        {
            List<QArea> result = new List<QArea>();

            TryAddArea(result, QTRACE);
            TryAddArea(result, QDEBUG);
            TryAddArea(result, QINFO);
            TryAddArea(result, QWARN);
            TryAddArea(result, QERROR);
            TryAddArea(result, QCRITICAL);

            foreach (var area in _areas)
            {
                result.Add(area);
            }

            return result;
        }

        private void TryAddArea(List<QArea> result, string name)
        {
            QArea area = _areas.FirstOrDefault(x => x.Name == name);
            if (area != null)
            {
                result.Add(area);
                _areas.Remove(area);
            }
        }
    }
}
