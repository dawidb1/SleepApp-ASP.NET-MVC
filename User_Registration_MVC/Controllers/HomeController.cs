using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using User_Registration_MVC.Models;
using User_Registration_MVC.Models.ViewModels;
using System.Security.Claims;
using System.Collections.ObjectModel;

namespace User_Registration_MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult DatePicker()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            // Po nicku dużo wolniej niż po id, ale trzeba zastosować
            //mechanizm Sesji albo wystarczy Viebag?

            //int? id = (int?)Session["userId"];
            //int id = int.Parse(HttpContext.Session["userId"].ToString());
            string username = HttpContext.User.Identity.Name;

            var db = new SleepLogAppEntities();
            if (db.User.Any(u => u.Username == username))
            {
                User user = db.User.First(u => u.Username == username);
                return View(user);
            }
            else return RedirectToAction("Login", "User");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Stats()
        {
            var db = new SleepLogAppEntities();

            //int userId = (int)Session["userId"];
            string username = HttpContext.User.Identity.Name;
            
            int? userId = db.User.Where(x => x.Username == username).FirstOrDefault().UserId;
            if (userId != null)
            {
                int LOGS_TO_STATS = 7;
                List<Sleep> sleepList = new List<Sleep>();

                var sleeps = db.Sleep.Where(sleep => sleep.UserId == userId).ToList();

                if (sleeps.Count<LOGS_TO_STATS)
                {
                    LOGS_TO_STATS = sleeps.Count;
                }
                for (int i = 1; i <= LOGS_TO_STATS; i++)
                {
                    sleepList.Add(sleeps[sleeps.Count - i]);
                }

                sleepList.Reverse();

                List<ChartInfo> chartList = new List<ChartInfo>();
                TimeSpan mean = new TimeSpan();
                TimeSpan sum = new TimeSpan();
                foreach (Sleep item in sleepList)
                {
                    sum += (TimeSpan)item.AmountOfSleep;
                    chartList.Add(new ChartInfo(item.AmountOfSleep, item.StartSleep.Date));
                }

                var meanTicks = sum.Ticks / chartList.Count;
                mean = TimeSpan.FromTicks(meanTicks);
                string meanString = string.Format("{0:00}h {1:00}m", mean.Hours, mean.Minutes);

                ViewBag.Mean = meanString;
                return View(chartList);
        }
            return RedirectToAction("Login", "User");
        }
    }
}