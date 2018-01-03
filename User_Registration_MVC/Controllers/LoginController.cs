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
            var db = new SleepLogAppEntities();
            var dbUser = db.User.Where(x => x.Username == user.Username).FirstOrDefault();

            //int userId = ValidateUser(logUser,dbUser);
            string message = string.Empty;

            if (isPasswordMatch(user,dbUser))
            {
                if (dbUser.IsEmailVerified)
                {
                    Session["UserId"] = user.UserId;
                    Session["Username"] = user.Username;
                    ViewBag.Message = "Login succesful.";
                    return RedirectToAction("Index", "Home");
                }
                else ViewBag.Message = "Account is not activated.";
                return RedirectToAction("Index", "Home");

            }
            ViewBag.Message = "Login and Password not match.";
            return RedirectToAction("Index", "Home");

            //return View("../Home/Index", user); //idzie do /login/login
        }
        [NonAction]
        private bool isPasswordMatch(User logUser,User dbUser)
        {
            if (logUser.Password == dbUser.Password) return true;
            return false;
        }
        
        //private int ValidateUser(User thisUser)
        //{
        //    int sleepId;
        //    var db = new SleepLogAppEntities();
        //    var dbUser = db.User.Where(x => x.Username == thisUser.Username).FirstOrDefault();

        //    string message;

        //    if (dbUser != null)
        //    {
        //        if (thisUser.Password == dbUser.Password
        //        && dbUser.IsEmailVerified)
        //        {
        //            sleepId = dbUser.UserId;
        //            message = "Login succesful";
        //        }
        //        else if (thisUser.Password != dbUser.Password)
        //        {
        //            sleepId = -1;
        //            message = "Username and password not match";
        //        }
        //        else if (!dbUser.IsEmailVerified)
        //        {
        //            sleepId = -2;
        //            message = "Email is not verified. Chech your mailbox and activate account.";
        //        }
        //        else throw new Exception();
        //    }
        //    else
        //    {
        //        message = "Username is not exist. Sign up";
        //        RedirectToAction("Home", "Index");
        //    }
        //    return sleepId;
        //}
        
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.RemoveAll();
            return View();
        }


        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(User user)
        {
            var db = new SleepLogAppEntities();
            user.CreatedDate = DateTime.Now;

            var usernameTaken = db.User.Any(u => u.Username == user.Username);
            var emailTaken = db.User.Any(u => u.Email == user.Email);

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
                db.User.Add(user);
                db.SaveChanges();
            }

            if (user.UserId>0)
            {
                var SleepList = SleepsInitializer.SleepsInitialize();
                foreach (Sleep sleep in SleepList)
                {
                    sleep.SetAmountOfSleep(); //możnaby przerzucić do SleepInitializer
                    db.User.First(x => x.UserId == user.UserId).Sleep.Add(sleep);
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