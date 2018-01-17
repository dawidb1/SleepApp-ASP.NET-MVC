using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using User_Registration_MVC.Models;
using User_Registration_MVC.Models.ViewModels;
using System.Security.Claims;

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
            //int? id = (int?)Session["userId"];
            //   int id = int.Parse(HttpContext.Session["userId"].ToString());
            string username = HttpContext.User.Identity.Name;
            //if (id != null)
            if(true)
            {
                var db = new SleepLogAppEntities();
                User user = db.User.First(u => u.Username == username);
                return View(user);
            }

            //return View();
        }
        [Authorize]
        [HttpGet]
        public ActionResult Stats()
        {
            var db = new SleepLogAppEntities();

            //int userId = (int)Session["userId"];
            string username = HttpContext.User.Identity.Name;
            int userId = db.User.Where(x => x.Username == username).FirstOrDefault().UserId;
            //if (userId != null)
            //if(true)
            //{
                var sleeps = db.Sleep.Where(sleep => sleep.UserId == userId).Take(7).ToList();

                List<ChartInfo> chartList = new List<ChartInfo>();
                TimeSpan mean = new TimeSpan();
                TimeSpan sum = new TimeSpan();
                foreach (Sleep item in sleeps)
                {
                    sum += (TimeSpan)item.AmountOfSleep;
                    chartList.Add(new ChartInfo(item.AmountOfSleep, item.StartSleep.Date));
                }

                var meanTicks = sum.Ticks / chartList.Count;
                mean = TimeSpan.FromTicks(meanTicks);
                string meanString = string.Format("{0:00}h {1:00}m", mean.Hours, mean.Minutes);

                ViewBag.Mean = meanString;
                return View(chartList);
            //}
            //return RedirectToAction("Home", "Index");
        }
    }
}