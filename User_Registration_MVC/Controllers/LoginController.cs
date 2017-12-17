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
                    TempData["UserId"] = userId;
                    return RedirectToAction("Index", "Home", new {id = userId});
            }

            ViewBag.Message = message;
            //return RedirectToAction("Index", "Home");
            return View("../Home/Index", user); //idzie do /login/login
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

            var usernameTaken = db.Users.Any(u => u.Username == user.Username);
            var emailTaken = db.Users.Any(u => u.Email == user.Email);

            if (usernameTaken)
            {
                user.UserId = -1;
            }
            else if (emailTaken)
            {
                user.UserId = -2;
            }
            else
            {
                db.Users.Add(user);
                db.SaveChanges();
            }

            if (user.UserId>0)
            {
                var SleepList = SleepsInitializer.SleepsInitialize();
                foreach (Sleep sleep in SleepList)
                {
                    sleep.InitOtherData();
                    db.Users.First(x => x.UserId == user.UserId).Sleep.Add(sleep);
                }

                db.SaveChanges();
            }

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

            return PartialView("_HeaderNavBar", user);
        }
    }
}