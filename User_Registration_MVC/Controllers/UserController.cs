using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using User_Registration_MVC.Models;

namespace User_Registration_MVC.Controllers
{
    public class UserController : Controller
    {
        //Login Action 
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin userLogin, string returnUrl="")
        {
            int REMEMBER_ME_TIME = 525600; //YEAR IN MINUTES
            int NOT_REMEMBER_ME_TIME = 20;
            
            string Message = string.Empty;
            using (var db = new SleepLogAppEntities())
            {
                var user = db.User.Where(x => x.Username == userLogin.Username).FirstOrDefault();
                if (user!=null)
                {
                    if (string.Compare(Crypto.Hash(userLogin.Password),user.Password)==0)
                    {
                        int timeout = userLogin.RememberMe ? REMEMBER_ME_TIME : NOT_REMEMBER_ME_TIME;
                        var ticket = new FormsAuthenticationTicket(userLogin.Username, userLogin.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);
                        Session["userId"] = user.UserId;

                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        Message = "Invalid credential provided";
                    }
                }
                else
                {
                    Message = "Invalid credential provided";
                }
            }

                ViewBag.Message = Message;
            return View();
        }

        //Logout
        [HttpGet]
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("Login");
        }
       
        // Registration Action
        public ActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified, LastLoginDate, ActivationCode, SleepTemporary, Sleep")] User user)
        {
            bool Status = false;
            string Message = string.Empty;

            //Model Validation
            if (ModelState.IsValid)
            {
                user.IsEmailVerified = false;
                user.CreatedDate = DateTime.Now;

                #region //Email is taken
                bool isEmailTaken = IsEmailTaken(user.Email);
                if (isEmailTaken)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(user);
                }
                #endregion
                #region //username is taken
                bool isUsernameTaken = IsUsernameTaken(user.Username);
                if (isUsernameTaken)
                {
                    ModelState.AddModelError("UsernameExist", "Username already exist");
                    return View(user);
                }
                #endregion

                #region //Generate activation code
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region //Password Hashing
                user.Password = Crypto.Hash(user.Password);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);
                #endregion

                #region //Save to db
                using (var db = new SleepLogAppEntities())
                {
                    db.User.Add(user);
                    db.SaveChanges();
                }
                #endregion

                #region //Send activation mail to user
                SendVeryficationLinkEmail(user.Email, user.ActivationCode.ToString());
                Message = "Registration succesfully done. Account activation link "
                    + "has been sent to your email adress:" + user.Email;
                Status = true;
                #endregion
            }
            else
            {
                Message = "Invalid Request";
            }

            ViewBag.Message = Message;
            ViewBag.Status = Status;
            return View(user);
        }

        //Verify account
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using(var db = new SleepLogAppEntities())
            {
                db.Configuration.ValidateOnSaveEnabled = false; //to avoid confirm password does not match on save in db
                User verifyUser = db.User.Where(x => x.ActivationCode == new Guid(id)).FirstOrDefault();
                if (verifyUser!=null)
                {
                    verifyUser.IsEmailVerified = true;
                    Status = true;

                    #region //sleep initializer
                    var SleepList = SleepsInitializer.SleepsInitialize();
                    foreach (Sleep sleep in SleepList)
                    {
                        sleep.SetAmountOfSleep(); //możnaby przerzucić do SleepInitializer
                        db.User.First(x => x.UserId == verifyUser.UserId).Sleep.Add(sleep);
                    }
                    #endregion

                    db.SaveChanges();
                }
                ViewBag.Status = Status;
            }
            return View();
        }

        [NonAction]
        public bool IsEmailTaken(string email)
        {
            using (var db = new SleepLogAppEntities())
            {
                return db.User.Any(x => x.Email == email);
            }
        }
        [NonAction]
        public bool IsUsernameTaken(string username)
        {
            using (var db = new SleepLogAppEntities())
            {
                return db.User.Any(x => x.Username == username);
            }
        }
        [NonAction]
        public void SendVeryficationLinkEmail(string email, string activationCode)
        {
            var verifyUrl = "/User/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("sleeplogapp@gmail.com", "Your Sleep Log App");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "yY6r46Kas68v";

            string subject = "Your account is succesfully created!";
            string body = "<br/><br/> We are excited to tell you that your Sleep Log account is succesfully created."
                + "Please click on the below link to verify your account"
                + "<br/><br/><a href='" + link + "'>" + link + "</a>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var mailMessage = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            smtp.Send(mailMessage);
        }
    }
}