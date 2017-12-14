using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using User_Registration_MVC.Models;

namespace User_Registration_MVC.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        //public ActionResult Index()
        //{
        //    return View();
        //}
        [HttpGet]
        public ActionResult Rejestration()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Rejestration(User user)
        {
            var db = new SleepAppV2Entities();
            user.CreatedDate = DateTime.Now;
            db.Users.Add(user);
            db.SaveChanges();
            string message = string.Empty;
            switch (user.UserId)
            {
                case -1:
                    message = "Username already exists.\\nPlease choose a different username.";
                    break;
                case -2:
                    message = "Supplied email address has already been used.";
                    break;
                default:
                    message = "Registration successful.\\nUser Id: " + user.UserId.ToString();
                    break;
            }
            ViewBag.Message = message;

            return View(user);
        }
    }
}