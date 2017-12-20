using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using User_Registration_MVC.Models;
using User_Registration_MVC.Models.ViewModels;

namespace User_Registration_MVC.Controllers
{
    public class HomeController : Controller
    {

        [HttpGet]
        public ActionResult Index()
        {
            int? id =(int?)Session["userId"];

            if (id != null)
            {
                var db = new SleepAppV2Entities();
                User user = db.Users.First(u => u.UserId == id);
                return View(user);
            }

            return View();
        }


        [HttpGet]
        public ActionResult Stats()
        {
            var db = new SleepAppV2Entities();

            int? userId = (int?)Session["userId"];
            if (userId != null)
            {
                var sleeps = db.Sleep.Where(sleep => sleep.UserId == userId).ToList();
      
                List<ChartInfo> chartList = new List<ChartInfo>();
                foreach (Sleep item in sleeps)
                {
                    chartList.Add(new ChartInfo(item.AmountOfSleep, item.StartSleep.Date));
                }
                return View(chartList);
            }
            return RedirectToAction("Login", "Login");
        }
    }
}