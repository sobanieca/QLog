using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SampleCloudApp.LogAreas;

namespace SampleCloudApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            QLog.Logger.LogTrace("/Home/Index called");

            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            QLog.Logger.LogTrace("/Home/About called");

            ViewBag.Message = "Your app description page.";

            QLog.Logger.LogDebug("Cleaning logs older than 30 days...");
            QLog.Utils.CleanLogsOlderThan(30);
            QLog.Logger.LogDebug("Finished logs cleanup");

            return View();
        }

        public ActionResult Contact()
        {
            QLog.Logger.LogTrace("/Home/Contact called");
            QLog.Logger.Log<QSampleArea>("Sample area log.");

            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
