using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using User_Registration_MVC.Models;
using  System.Web.Security;

namespace User_Registration_MVC.Controllers
{
    public class LoginController : Controller
    {
        //GET: Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [Authorize]
        public ActionResult MyProfile()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(User user)
        {
            var db = new SleepAppV2Entities();
            int? userId = db.ValidateUser(user.Username, user.Password).FirstOrDefault();

            string message = string.Empty;
            switch (userId.Value)
            {
                case -1:
                    message = "Username and/or password is incorrect.";
                    break;
                case -2:
                    message = "Account has not been activated.";
                    break;
                default:
                    FormsAuthentication.SetAuthCookie(user.Username, user.RememberMe);
                    return RedirectToAction("MyProfile");
            }

            ViewBag.Message = message;
            return View(user);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }


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

            var SleepList = SleepsInitializer.SleepsInitialize();
            foreach (Sleep sleep in SleepList)
            {
                sleep.UserId = user.UserId;
                db.Sleep.Add(sleep);
            }
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