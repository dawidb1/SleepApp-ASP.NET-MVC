using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using User_Registration_MVC.Models;

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

        // GET: Sleeps/Details/5
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
        public ActionResult Create([Bind(Include = "SleepId,StartSleep,EndSleep,MorningRating,EveningRating,Note,AmountOfSleep,UserId,QuickSleep")] Sleep sleep)
        {
            if (ModelState.IsValid)
            {
                var username = HttpContext.User.Identity.Name;
                //var user = db.User .Select(x => x.Username == username);
                //var user = db.User.
                //sleep.UserId = ViewBag.UserId;
                var user = db.User.Where(x => x.Username == username).FirstOrDefault();
                sleep.User = user;
                sleep.UserId = user.UserId;

                //db.Sleep.Add(new Sleep(sleep)); //MUST CHANGE DB TO COLAPSE IN CREATE?????
                //db.Sleep.Add(sleep);
                db.User.First(x => x.UserId == sleep.UserId).Sleep.Add(new Sleep(sleep));
                //db.User.First(x => x.UserId == sleep.UserId).Sleep.Add(sleep);
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }

            ViewBag.UserId = new SelectList(db.User, "UserId", "Username", sleep.UserId);
            return View(sleep);
        }

        // GET: Sleeps/Edit/5
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
        public ActionResult Edit([Bind(Include = "SleepId,StartSleep,EndSleep,MorningRating,EveningRating,Note,QuickSleep")] Sleep sleep)
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
                //jakaś walidacja żeby maks 24 godziny
                dbSleep.SetAmountOfSleep();

                db.Entry(dbSleep).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }
            ViewBag.UserId = new SelectList(db.User, "UserId", "Username", sleep.UserId);
            return View(sleep);
        }

        // GET: Sleeps/Delete/5
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
