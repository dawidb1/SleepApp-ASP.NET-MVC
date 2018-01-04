using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using User_Registration_MVC.Models;

namespace User_Registration_MVC.Controllers
{
    public class UserController : Controller
    {
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
                #region //Email is taken
                bool isEmailTaken = IsEmailTaken(user.Email);
                if (isEmailTaken)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
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

                user.IsEmailVerified = false;
                user.CreatedDate = DateTime.Now;

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

            //Sleep initializing
            //Add user created date
            ViewBag.Message = Message;
            ViewBag.Status = Status;
            return View(user);
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