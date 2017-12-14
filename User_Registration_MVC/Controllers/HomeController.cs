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
        // GET: Registration
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult Index()
        //{

        //    return View();
        //}

        
    }
}