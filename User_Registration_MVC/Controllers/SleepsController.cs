﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using User_Registration_MVC.Models;
using User_Registration_MVC.Models.ViewModels;

namespace User_Registration_MVC.Controllers
{
    public class SleepsController : Controller
    {
        private SleepLogAppEntities db = new SleepLogAppEntities();

        // GET: Sleeps
        public ActionResult Index()
        {
            var sleep = db.Sleep.Include(s => s.User);
            return View(sleep.ToList());
        }

        [Authorize]
        public ActionResult GoSleep()
        {
            var username = HttpContext.User.Identity.Name;
            var user = db.User.Where(x => x.Username == username).FirstOrDefault();

            ReloadTimer reloadTimer;
            if (user.SleepTemporary.Any())
            {
                var sTemp = user.SleepTemporary.LastOrDefault();
                
                //if start sleep time remembered
                if (!sTemp.StartSleep.Equals(DateTime.MinValue)) //jeśli jest wypełnione
                {
                    var amountOfSLeep = DateTime.Now - sTemp.StartSleep;
                    reloadTimer = new ReloadTimer {
                        RememberTimer = true,
                        Hours = amountOfSLeep.Hours,
                        Minutes = amountOfSLeep.Minutes,
                        Seconds = amountOfSLeep.Seconds };
                }
                else
                {
                    throw new Exception();
                }
            }
            else reloadTimer = new ReloadTimer { RememberTimer = false };
            return View(reloadTimer);
        }
        public void GetTime()
        {
            var username = HttpContext.User.Identity.Name;
            var user = db.User.Where(x => x.Username == username).FirstOrDefault();

            if (user.SleepTemporary.Any())
            {
                var sTemp = user.SleepTemporary.LastOrDefault();
                //walidacja 24h
                TimeSpan HOURS_VALIDATE = new TimeSpan(23, 59, 59);
                TimeSpan timeSpanTemp = DateTime.Now - sTemp.StartSleep;

                if (timeSpanTemp<HOURS_VALIDATE)
                {
                    Sleep sleep = new Sleep(sTemp.StartSleep, DateTime.Now.ToLocalTime());
                    user.Sleep.Add(sleep);

                }
                db.SleepTemporary.Remove(sTemp);
            }
            else
            {
                SleepTemporary st = new SleepTemporary();
                st.StartSleep = DateTime.Now;
                db.SleepTemporary.Add(st);

                user.SleepTemporary.Add(st);
            }
            db.SaveChanges();
        }

        // GET: Sleeps/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sleep sleep = db.Sleep.Find(id);
            if (sleep == null)
            {
                return HttpNotFound();
            }
            return View(sleep);
        }

        // GET: Sleeps/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.User, "UserId", "Username");
            return View();
        }

        // POST: Sleeps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(
            Include = "SleepId,StartSleep,EndSleep,MorningRating," +
            "EveningRating,Note,AmountOfSleep,UserId,QuickSleep")] Sleep sleep)
        {
            if (ModelState.IsValid)
            {
                TimeSpan hoursValidate = new TimeSpan(23, 59, 59);
                TimeSpan timeSpanTemp = sleep.EndSleep - sleep.StartSleep;

                if (Math.Abs(timeSpanTemp.Ticks) > hoursValidate.Ticks)
                {
                    ViewBag.Message = "Sen nie może trwać więcej niż 24 godziny, " +
                        "ani być ujemny.";
                    return View();
                }

                var username = HttpContext.User.Identity.Name;
         
                var user = db.User.Where(x => x.Username == username).FirstOrDefault();
                user.Sleep.Add(new Sleep(sleep));

                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }

            ViewBag.UserId = new SelectList(db.User, "UserId", "Username", sleep.UserId);
            return View(sleep);
        }

        // GET: Sleeps/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sleep sleep = db.Sleep.Find(id);
            if (sleep == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.User, "UserId", "Username", sleep.UserId);
            return View(sleep);
        }

        // POST: Sleeps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SleepId,StartSleep,EndSleep," +
            "MorningRating,EveningRating,Note,QuickSleep")] Sleep sleep)
        {
            if (ModelState.IsValid)
            {
                var dbSleep = db.Sleep.Where(x => x.SleepId == sleep.SleepId).FirstOrDefault();

                dbSleep.StartSleep = sleep.StartSleep;
                dbSleep.EndSleep = sleep.EndSleep;
                dbSleep.MorningRating = sleep.MorningRating;
                dbSleep.EveningRating = sleep.EveningRating;
                dbSleep.Note = sleep.Note;
                dbSleep.QuickSleep = sleep.QuickSleep;
                dbSleep.SetAmountOfSleep();

                db.Entry(dbSleep).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }
            ViewBag.UserId = new SelectList(db.User, "UserId", "Username", sleep.UserId);
            return View(sleep);
        }

        // GET: Sleeps/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sleep sleep = db.Sleep.Find(id);
            if (sleep == null)
            {
                return HttpNotFound();
            }
            return View(sleep);
        }

        // POST: Sleeps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sleep sleep = db.Sleep.Find(id);
            db.Sleep.Remove(sleep);
            db.SaveChanges();
            return RedirectToAction("Index","Home");
        }


        //co to jest?
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
