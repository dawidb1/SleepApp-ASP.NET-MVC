using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using User_Registration_MVC.Models;

namespace User_Registration_MVC.Controllers
{
    public class HomeController : Controller
    {

        [HttpGet]
        public ActionResult Index(int? id)
        {
            User user;
            if (id != null)
            {
                var db = new SleepAppV2Entities();
                user = db.Users.First(u => u.UserId == id);
            }
            else
            {
                user = new User();
            }
            return View(user);
        }

    }
}