using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QLog.Areas.Base;

namespace SampleCloudApp.LogAreas
{
    public class QSampleArea : QArea
    {
        public override uint Severity
        {
            get
            {
                return 3500;
            }
        }

        public override QAreaColor AreaColor
        {
            get
            {
                return new QAreaColor(170, 0, 211);
            }
        }
    }
}